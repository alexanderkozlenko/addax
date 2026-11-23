// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

internal sealed class TabularParser
{
    private const int s_blockLength = 32;

    private readonly char _lineTerminatorStart;
    private readonly char _lineTerminatorEnd;
    private readonly int _lineTerminatorLength;
    private readonly char _delimiter;
    private readonly char _quoteChar;
    private readonly char _escapeChar;
    private readonly char? _commentPrefix;

    private readonly SearchValues<char> _valueStopChars;
    private readonly SearchValues<char> _quotedValueStopChars;

    public TabularParser(TabularDialect dialect)
    {
        _lineTerminatorStart = dialect.LineTerminator[0];
        _lineTerminatorEnd = dialect.LineTerminator[^1];
        _lineTerminatorLength = dialect.LineTerminator.Length;
        _delimiter = dialect.Delimiter;
        _quoteChar = dialect.QuoteChar;
        _escapeChar = dialect.EscapeChar;
        _commentPrefix = dialect.CommentPrefix;

        _valueStopChars = dialect.SearchValues.ValueStopChars;
        _quotedValueStopChars = dialect.SearchValues.EscapableChars;
    }

    public OperationStatus Parse(ReadOnlySpan<char> source, bool isEndOfStream, ref TabularParserState state, Span<int> fieldEnds, out int fieldCount)
    {
        Debug.Assert(fieldEnds.Length >= s_blockLength);

        var current = state;
        var count = 0;
        var scanEnd = 0;
        var status = OperationStatus.Done;

        while (true)
        {
            var position = current.Position;

            if (position == source.Length)
            {
                if (!isEndOfStream)
                {
                    status = OperationStatus.NeedMoreData;

                    break;
                }

                if (current.Area == TabularParsingArea.EndOfStream)
                {
                    break;
                }

                if (count == fieldEnds.Length)
                {
                    status = OperationStatus.DestinationTooSmall;

                    break;
                }

                if (current.Area is TabularParsingArea.QuotedValue or TabularParsingArea.QuotedValueEscape or TabularParsingArea.QuotedValueLineTerminator)
                {
                    status = OperationStatus.NeedMoreData;

                    break;
                }

                fieldEnds[count++] = current.Area is TabularParsingArea.Comment or TabularParsingArea.CommentLineTerminator ? ~position : position;
                current.Area = TabularParsingArea.EndOfStream;

                break;
            }

            if (count == fieldEnds.Length)
            {
                status = OperationStatus.DestinationTooSmall;

                break;
            }

            if ((current.Area is TabularParsingArea.StartOfRecord or TabularParsingArea.StartOfField) &&
                (position >= scanEnd) &&
                (position < source.Length - s_blockLength) &&
                Vector128.IsHardwareAccelerated)
            {
                if (ScanFields(source, ref current, fieldEnds, ref count, out scanEnd) == OperationStatus.DestinationTooSmall)
                {
                    status = OperationStatus.DestinationTooSmall;

                    break;
                }

                continue;
            }

            switch (current.Area)
            {
                case TabularParsingArea.StartOfRecord:
                case TabularParsingArea.StartOfField:
                    {
                        var symbol = source[position];

                        if (symbol == _quoteChar)
                        {
                            current.Area = TabularParsingArea.QuotedValue;
                            current.Position = position + 1;
                        }
                        else if ((current.Area == TabularParsingArea.StartOfRecord) && (symbol == _commentPrefix))
                        {
                            current.Area = TabularParsingArea.Comment;
                            current.Position = position + 1;
                        }
                        else
                        {
                            current.Area = TabularParsingArea.Value;
                        }
                    }
                    break;
                case TabularParsingArea.Value:
                    {
                        var offset = source.Slice(position).IndexOfAny(_valueStopChars);

                        if (offset < 0)
                        {
                            current.Position = source.Length;

                            break;
                        }

                        position += offset;

                        var symbol = source[position];

                        if (symbol == _delimiter)
                        {
                            fieldEnds[count++] = position;
                            current.Area = TabularParsingArea.StartOfField;
                            current.Position = position + 1;
                        }
                        else if (symbol == _lineTerminatorStart)
                        {
                            if (_lineTerminatorLength == 1)
                            {
                                fieldEnds[count++] = position;
                                current.Area = TabularParsingArea.StartOfRecord;
                            }
                            else
                            {
                                current.Area = TabularParsingArea.ValueLineTerminator;
                            }

                            current.Position = position + 1;
                        }
                        else
                        {
                            current.Position = position;
                            state = current;
                            fieldCount = count;

                            return OperationStatus.InvalidData;
                        }
                    }
                    break;
                case TabularParsingArea.ValueLineTerminator:
                    {
                        if (source[position] == _lineTerminatorEnd)
                        {
                            fieldEnds[count++] = position - 1;
                            current.Area = TabularParsingArea.StartOfRecord;
                            current.Position = position + 1;
                        }
                        else
                        {
                            current.Area = TabularParsingArea.Value;
                        }
                    }
                    break;
                case TabularParsingArea.QuotedValue:
                    {
                        var offset = source.Slice(position).IndexOfAny(_quotedValueStopChars);

                        if (offset < 0)
                        {
                            current.Position = source.Length;

                            break;
                        }

                        position += offset;
                        current.Area = source[position] == _quoteChar ? TabularParsingArea.QuotedValueTail : TabularParsingArea.QuotedValueEscape;
                        current.Position = position + 1;
                    }
                    break;
                case TabularParsingArea.QuotedValueEscape:
                    {
                        var symbol = source[position];

                        if ((symbol != _quoteChar) && (symbol != _escapeChar))
                        {
                            state = current;
                            fieldCount = count;

                            return OperationStatus.InvalidData;
                        }

                        current.Area = TabularParsingArea.QuotedValue;
                        current.Position = position + 1;
                    }
                    break;
                case TabularParsingArea.QuotedValueTail:
                    {
                        var symbol = source[position];

                        if ((symbol == _quoteChar) && (_escapeChar == _quoteChar))
                        {
                            current.Area = TabularParsingArea.QuotedValue;
                            current.Position = position + 1;
                        }
                        else if (symbol == _delimiter)
                        {
                            fieldEnds[count++] = position;
                            current.Area = TabularParsingArea.StartOfField;
                            current.Position = position + 1;
                        }
                        else if (symbol == _lineTerminatorStart)
                        {
                            if (_lineTerminatorLength == 1)
                            {
                                fieldEnds[count++] = position;
                                current.Area = TabularParsingArea.StartOfRecord;
                            }
                            else
                            {
                                current.Area = TabularParsingArea.QuotedValueLineTerminator;
                            }

                            current.Position = position + 1;
                        }
                        else
                        {
                            state = current;
                            fieldCount = count;

                            return OperationStatus.InvalidData;
                        }
                    }
                    break;
                case TabularParsingArea.QuotedValueLineTerminator:
                    {
                        if (source[position] != _lineTerminatorEnd)
                        {
                            state = current;
                            fieldCount = count;

                            return OperationStatus.InvalidData;
                        }

                        fieldEnds[count++] = position - 1;
                        current.Area = TabularParsingArea.StartOfRecord;
                        current.Position = position + 1;
                    }
                    break;
                case TabularParsingArea.Comment:
                    {
                        var offset = source.Slice(position).IndexOf(_lineTerminatorStart);

                        if (offset < 0)
                        {
                            current.Position = source.Length;

                            break;
                        }

                        position += offset;

                        if (_lineTerminatorLength == 1)
                        {
                            fieldEnds[count++] = ~position;
                            current.Area = TabularParsingArea.StartOfRecord;
                        }
                        else
                        {
                            current.Area = TabularParsingArea.CommentLineTerminator;
                        }

                        current.Position = position + 1;
                    }
                    break;
                case TabularParsingArea.CommentLineTerminator:
                    {
                        if (source[position] == _lineTerminatorEnd)
                        {
                            fieldEnds[count++] = ~(position - 1);
                            current.Area = TabularParsingArea.StartOfRecord;
                            current.Position = position + 1;
                        }
                        else
                        {
                            current.Area = TabularParsingArea.Comment;
                        }
                    }
                    break;
            }
        }

        state = current;
        fieldCount = count;

        return status;
    }

    public int DecodeField(Span<char> source, int fieldStart, int fieldEnd, out int valueStart)
    {
        if (fieldEnd < 0)
        {
            valueStart = fieldStart + 1;

            return ~fieldEnd - fieldStart - 1;
        }

        if ((fieldEnd == fieldStart) || (source[fieldStart] != _quoteChar))
        {
            valueStart = fieldStart;

            return fieldEnd - fieldStart;
        }

        valueStart = fieldStart + 1;

        var value = source.Slice(valueStart, fieldEnd - fieldStart - 2);
        var escapeIndex = value.IndexOf(_escapeChar);

        if (escapeIndex < 0)
        {
            return value.Length;
        }

        return UnescapeValue(value, escapeIndex);
    }

    private int UnescapeValue(Span<char> value, int escapeIndex)
    {
        var length = escapeIndex;
        var index = escapeIndex;

        while (true)
        {
            value[length++] = value[index + 1];
            index += 2;
            escapeIndex = value.Slice(index).IndexOf(_escapeChar);

            if (escapeIndex < 0)
            {
                value.Slice(index).CopyTo(value.Slice(length));

                return length + value.Length - index;
            }

            value.Slice(index, escapeIndex).CopyTo(value.Slice(length));
            length += escapeIndex;
            index += escapeIndex;
        }
    }

    private OperationStatus ScanFields(ReadOnlySpan<char> source, ref TabularParserState state, Span<int> fieldEnds, ref int fieldCount, out int scanEnd)
    {
        var fieldStart = state.Position;
        var isStartOfRecord = state.Area == TabularParsingArea.StartOfRecord;

        if (isStartOfRecord && (source[fieldStart] == _commentPrefix))
        {
            scanEnd = fieldStart + 1;

            return OperationStatus.Done;
        }

        var chars = MemoryMarshal.Cast<char, ushort>(source);
        var runCount = fieldCount;
        var index = fieldStart;
        var insideMask = 0u;
        var closingMask = 0u;
        var previousStops = 0u;
        var status = OperationStatus.Done;

        scanEnd = source.Length;

        while (index < source.Length - s_blockLength)
        {
            if (fieldCount > fieldEnds.Length - s_blockLength)
            {
                status = OperationStatus.DestinationTooSmall;

                break;
            }

            var stops = GetStopMask(chars, index, out var hasSpecialChars);

            if (hasSpecialChars || ((insideMask | closingMask) != 0))
            {
                var quotes = GetCharMask(chars, index, _quoteChar);
                var delimiters = GetCharMask(chars, index, _delimiter);
                var lineTerminators = stops & ~delimiters;
                var inside = quotes;

                inside ^= inside << 1;
                inside ^= inside << 2;
                inside ^= inside << 4;
                inside ^= inside << 8;
                inside ^= inside << 16;
                inside ^= insideMask;

                var openings = quotes & inside;
                var closings = quotes & ~inside;

                delimiters &= ~inside;
                lineTerminators &= ~inside;
                stops = delimiters | lineTerminators;

                var fieldStarts = (delimiters << 1) | (lineTerminators << _lineTerminatorLength);
                var recordStarts = lineTerminators << _lineTerminatorLength;

                if (index == state.Position)
                {
                    fieldStarts |= 1;
                }
                else if (_lineTerminatorLength == 1)
                {
                    if ((previousStops >> 31) != 0)
                    {
                        fieldStarts |= 1;
                        recordStarts |= source[index - 1] != _delimiter ? 1u : 0u;
                    }
                }
                else
                {
                    if ((previousStops >> 31) != 0)
                    {
                        if (source[index - 1] == _delimiter)
                        {
                            fieldStarts |= 1;
                        }
                        else
                        {
                            fieldStarts |= 2;
                            recordStarts |= 2;
                        }
                    }

                    if ((((previousStops >> 30) & 1) != 0) && (source[index - 2] != _delimiter))
                    {
                        fieldStarts |= 1;
                        recordStarts |= 1;
                    }
                }

                var afterClosings = (closings << 1) | closingMask;
                var invalid = afterClosings & ~stops;

                if (_escapeChar == _quoteChar)
                {
                    invalid &= ~openings;
                    fieldStarts |= afterClosings;
                }
                else
                {
                    invalid |= GetCharMask(chars, index, _escapeChar) & inside;
                }

                invalid |= openings & ~fieldStarts;

                if (_commentPrefix.HasValue)
                {
                    invalid |= GetCharMask(chars, index, _commentPrefix.Value) & recordStarts;
                }

                if (invalid != 0)
                {
                    scanEnd = index + s_blockLength;

                    break;
                }

                insideMask = (uint)((int)inside >> 31);
                closingMask = closings >> 31;
            }

            var target = fieldEnds.Slice(fieldCount);
            var stopCount = BitOperations.PopCount(stops);

            previousStops = stops;

            target[0] = index + BitOperations.TrailingZeroCount(stops);
            stops &= stops - 1;
            target[1] = index + BitOperations.TrailingZeroCount(stops);
            stops &= stops - 1;
            target[2] = index + BitOperations.TrailingZeroCount(stops);
            stops &= stops - 1;
            target[3] = index + BitOperations.TrailingZeroCount(stops);
            stops &= stops - 1;

            for (var i = 4; i < stopCount; i += 4)
            {
                target[i + 0] = index + BitOperations.TrailingZeroCount(stops);
                stops &= stops - 1;
                target[i + 1] = index + BitOperations.TrailingZeroCount(stops);
                stops &= stops - 1;
                target[i + 2] = index + BitOperations.TrailingZeroCount(stops);
                stops &= stops - 1;
                target[i + 3] = index + BitOperations.TrailingZeroCount(stops);
                stops &= stops - 1;
            }

            fieldCount += stopCount;
            index += s_blockLength;
        }

        if (fieldCount != runCount)
        {
            var fieldEnd = fieldEnds[fieldCount - 1];

            isStartOfRecord = source[fieldEnd] != _delimiter;
            fieldStart = fieldEnd + (isStartOfRecord ? _lineTerminatorLength : 1);
        }

        state.Position = fieldStart;
        state.Area = isStartOfRecord ? TabularParsingArea.StartOfRecord : TabularParsingArea.StartOfField;

        return status;
    }

    private uint GetStopMask(ReadOnlySpan<ushort> chars, int index, out bool hasSpecialChars)
    {
        var mask = 0u;

        if (Vector256.IsHardwareAccelerated)
        {
            var delimiters = Vector256.Create((ushort)_delimiter);
            var lineTerminatorStarts = Vector256.Create((ushort)_lineTerminatorStart);
            var lineTerminatorEnds = Vector256.Create((ushort)_lineTerminatorEnd);
            var quoteChars = Vector256.Create((ushort)_quoteChar);
            var specialChars = Vector256<ushort>.Zero;

            for (var offset = 0; offset < s_blockLength; offset += Vector256<ushort>.Count)
            {
                var block = Vector256.Create(chars.Slice(index + offset, Vector256<ushort>.Count));
                var lineTerminators = Vector256.Equals(block, lineTerminatorStarts);

                if (_lineTerminatorLength != 1)
                {
                    lineTerminators &= Vector256.Equals(Vector256.Create(chars.Slice(index + offset + 1, Vector256<ushort>.Count)), lineTerminatorEnds);
                }

                mask |= (Vector256.Equals(block, delimiters) | lineTerminators).ExtractMostSignificantBits() << offset;
                specialChars |= Vector256.Equals(block, quoteChars);

                if (_escapeChar != _quoteChar)
                {
                    specialChars |= Vector256.Equals(block, Vector256.Create((ushort)_escapeChar));
                }

                if (_commentPrefix.HasValue)
                {
                    specialChars |= Vector256.Equals(block, Vector256.Create((ushort)_commentPrefix.Value));
                }
            }

            hasSpecialChars = specialChars != Vector256<ushort>.Zero;
        }
        else
        {
            var delimiters = Vector128.Create((ushort)_delimiter);
            var lineTerminatorStarts = Vector128.Create((ushort)_lineTerminatorStart);
            var lineTerminatorEnds = Vector128.Create((ushort)_lineTerminatorEnd);
            var quoteChars = Vector128.Create((ushort)_quoteChar);
            var specialChars = Vector128<ushort>.Zero;

            for (var offset = 0; offset < s_blockLength; offset += Vector128<ushort>.Count)
            {
                var block = Vector128.Create(chars.Slice(index + offset, Vector128<ushort>.Count));
                var lineTerminators = Vector128.Equals(block, lineTerminatorStarts);

                if (_lineTerminatorLength != 1)
                {
                    lineTerminators &= Vector128.Equals(Vector128.Create(chars.Slice(index + offset + 1, Vector128<ushort>.Count)), lineTerminatorEnds);
                }

                mask |= (Vector128.Equals(block, delimiters) | lineTerminators).ExtractMostSignificantBits() << offset;
                specialChars |= Vector128.Equals(block, quoteChars);

                if (_escapeChar != _quoteChar)
                {
                    specialChars |= Vector128.Equals(block, Vector128.Create((ushort)_escapeChar));
                }

                if (_commentPrefix.HasValue)
                {
                    specialChars |= Vector128.Equals(block, Vector128.Create((ushort)_commentPrefix.Value));
                }
            }

            hasSpecialChars = specialChars != Vector128<ushort>.Zero;
        }

        return mask;
    }

    private static uint GetCharMask(ReadOnlySpan<ushort> chars, int index, char value)
    {
        var mask = 0u;

        if (Vector256.IsHardwareAccelerated)
        {
            var values = Vector256.Create((ushort)value);

            for (var offset = 0; offset < s_blockLength; offset += Vector256<ushort>.Count)
            {
                mask |= Vector256.Equals(Vector256.Create(chars.Slice(index + offset, Vector256<ushort>.Count)), values).ExtractMostSignificantBits() << offset;
            }
        }
        else
        {
            var values = Vector128.Create((ushort)value);

            for (var offset = 0; offset < s_blockLength; offset += Vector128<ushort>.Count)
            {
                mask |= Vector128.Equals(Vector128.Create(chars.Slice(index + offset, Vector128<ushort>.Count)), values).ExtractMostSignificantBits() << offset;
            }
        }

        return mask;
    }
}
