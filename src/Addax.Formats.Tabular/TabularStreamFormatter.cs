// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;

namespace Addax.Formats.Tabular;

internal sealed class TabularStreamFormatter
{
    private readonly char _recordSeparationSymbol1;
    private readonly char _recordSeparationSymbol2;
    private readonly char _fieldSeparationSymbol;
    private readonly char _fieldQuoteSymbol;
    private readonly char _fieldEscapeSymbol;
    private readonly char? _commentPrefix;

    public TabularStreamFormatter(TabularDataDialect dialect)
    {
        _recordSeparationSymbol1 = dialect.LineTerminator[0];
        _recordSeparationSymbol2 = dialect.LineTerminator[^1];
        _fieldSeparationSymbol = dialect.Delimiter;
        _fieldQuoteSymbol = dialect.QuoteChar;
        _fieldEscapeSymbol = dialect.EscapeChar;
        _commentPrefix = dialect.CommentPrefix;
    }

    public void WriteRecordSeparator(IBufferWriter<char> writer, out long committed)
    {
        if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
        {
            var buffer = writer.GetSpan(1);

            buffer[0] = _recordSeparationSymbol1;
            writer.Advance(1);
            committed = 1;
        }
        else
        {
            var buffer = writer.GetSpan(2);

            buffer[0] = _recordSeparationSymbol1;
            buffer[1] = _recordSeparationSymbol2;
            writer.Advance(2);
            committed = 2;
        }
    }

    public void WriteFieldSeparator(IBufferWriter<char> writer, out long committed)
    {
        var buffer = writer.GetSpan(1);

        buffer[0] = _fieldSeparationSymbol;
        writer.Advance(1);
        committed = 1;
    }

    public void WriteValue(ReadOnlySpan<char> value, IBufferWriter<char> writer, out long committed)
    {
        committed = 0;

        if (value.IsEmpty)
        {
            return;
        }

        if (!RequiresEscaping(value))
        {
            var bufferSize = value.Length;
            var buffer = writer.GetSpan(bufferSize);

            value.CopyTo(buffer);
            writer.Advance(bufferSize);
            committed += bufferSize;
        }
        else
        {
            {
                var buffer = writer.GetSpan(1);

                buffer[0] = _fieldQuoteSymbol;
                writer.Advance(1);
                committed++;
            }

            var currentSegment = value;

            while (!currentSegment.IsEmpty)
            {
                var symbolIndex = _fieldEscapeSymbol != _fieldQuoteSymbol ?
                    currentSegment.IndexOfAny(_fieldQuoteSymbol, _fieldEscapeSymbol) :
                    currentSegment.IndexOf(_fieldQuoteSymbol);

                if (symbolIndex >= 0)
                {
                    if (symbolIndex > 0)
                    {
                        var sourceFragment = currentSegment[..symbolIndex];
                        var bufferSize = symbolIndex;
                        var buffer = writer.GetSpan(bufferSize);

                        sourceFragment.CopyTo(buffer);
                        currentSegment = currentSegment[symbolIndex..];
                        writer.Advance(bufferSize);
                        committed += bufferSize;
                    }

                    {
                        var buffer = writer.GetSpan(2);

                        buffer[0] = _fieldEscapeSymbol;
                        buffer[1] = currentSegment[0];
                        currentSegment = currentSegment[1..];
                        writer.Advance(2);
                        committed += 2;
                    }
                }
                else
                {
                    var bufferSize = currentSegment.Length;
                    var buffer = writer.GetSpan(bufferSize);

                    currentSegment.CopyTo(buffer);
                    writer.Advance(bufferSize);
                    committed += bufferSize;

                    currentSegment = ReadOnlySpan<char>.Empty;
                }
            }

            {
                var buffer = writer.GetSpan(1);

                buffer[0] = _fieldQuoteSymbol;
                writer.Advance(1);
                committed++;
            }
        }
    }

    public void WriteValue(in ReadOnlySequence<char> value, IBufferWriter<char> writer, out long committed)
    {
        committed = 0;

        if (value.IsEmpty)
        {
            return;
        }

        var reader = new SequenceReader<char>(value);

        if (!RequiresEscaping(value))
        {
            while (!reader.End)
            {
                var bufferSize = reader.UnreadSpan.Length;
                var buffer = writer.GetSpan(bufferSize);

                reader.UnreadSpan.CopyTo(buffer);
                reader.Advance(bufferSize);
                writer.Advance(bufferSize);
                committed += bufferSize;
            }
        }
        else
        {
            {
                var buffer = writer.GetSpan(1);

                buffer[0] = _fieldQuoteSymbol;
                writer.Advance(1);
                committed++;
            }

            while (!reader.End)
            {
                var currentSegment = reader.UnreadSpan;

                var symbolIndex = _fieldEscapeSymbol != _fieldQuoteSymbol ?
                    currentSegment.IndexOfAny(_fieldQuoteSymbol, _fieldEscapeSymbol) :
                    currentSegment.IndexOf(_fieldQuoteSymbol);

                if (symbolIndex >= 0)
                {
                    if (symbolIndex > 0)
                    {
                        var sourceFragment = currentSegment[..symbolIndex];
                        var bufferSize = symbolIndex;
                        var buffer = writer.GetSpan(bufferSize);

                        sourceFragment.CopyTo(buffer);
                        reader.Advance(bufferSize);
                        writer.Advance(bufferSize);
                        committed += bufferSize;
                    }

                    {
                        var buffer = writer.GetSpan(2);

                        buffer[0] = _fieldEscapeSymbol;
                        buffer[1] = currentSegment[symbolIndex];
                        reader.Advance(1);
                        writer.Advance(2);
                        committed += 2;
                    }
                }
                else
                {
                    var bufferSize = currentSegment.Length;
                    var buffer = writer.GetSpan(bufferSize);

                    currentSegment.CopyTo(buffer);
                    reader.Advance(currentSegment.Length);
                    writer.Advance(bufferSize);
                    committed += bufferSize;
                }
            }

            {
                var buffer = writer.GetSpan(1);

                buffer[0] = _fieldQuoteSymbol;
                writer.Advance(1);
                committed++;
            }
        }
    }

    public void WriteComment(ReadOnlySpan<char> value, IBufferWriter<char> writer, out long committed)
    {
        Debug.Assert(_commentPrefix is not null);

        committed = 0;

        {
            var buffer = writer.GetSpan(1);

            buffer[0] = _commentPrefix.Value;
            writer.Advance(1);
        }

        if (!value.IsEmpty)
        {
            var bufferSize = value.Length;
            var buffer = writer.GetSpan(bufferSize);

            value.CopyTo(buffer);
            writer.Advance(bufferSize);
            committed += bufferSize;
        }
    }

    public void WriteComment(in ReadOnlySequence<char> value, IBufferWriter<char> writer, out long committed)
    {
        Debug.Assert(_commentPrefix is not null);

        committed = 0;

        {
            var buffer = writer.GetSpan(1);

            buffer[0] = _commentPrefix.Value;
            writer.Advance(1);
        }

        var reader = new SequenceReader<char>(value);

        while (!reader.End)
        {
            var bufferSize = reader.UnreadSpan.Length;
            var buffer = writer.GetSpan(bufferSize);

            reader.UnreadSpan.CopyTo(buffer);
            reader.Advance(bufferSize);
            writer.Advance(bufferSize);
            committed += bufferSize;
        }
    }

    public bool CanWriteComment(ReadOnlySpan<char> value)
    {
        if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
        {
            return value.IndexOf(_recordSeparationSymbol1) < 0;
        }
        else
        {
            var separator = (ReadOnlySpan<char>)stackalloc char[]
            {
                _recordSeparationSymbol1,
                _recordSeparationSymbol2,
            };

            return value.IndexOf(separator) < 0;
        }
    }

    public bool CanWriteComment(in ReadOnlySequence<char> value)
    {
        var reader = new SequenceReader<char>(value);

        if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
        {
            while (!reader.End)
            {
                if (reader.UnreadSpan.IndexOf(_recordSeparationSymbol1) >= 0)
                {
                    return false;
                }

                reader.Advance(reader.UnreadSpan.Length);
            }

            return true;
        }
        else
        {
            var foundLineTerminator = false;

            while (!reader.End)
            {
                var currentSegment = reader.UnreadSpan;

                if (!foundLineTerminator)
                {
                    var symbolIndex = currentSegment.IndexOf(_recordSeparationSymbol1);

                    if (symbolIndex < 0)
                    {
                        reader.Advance(currentSegment.Length);

                        continue;
                    }

                    foundLineTerminator = true;
                    reader.Advance(symbolIndex + 1);
                }
                else
                {
                    var result = reader.TryRead(out var symbol);

                    Debug.Assert(result);

                    if (symbol == _recordSeparationSymbol2)
                    {
                        return false;
                    }

                    foundLineTerminator = false;
                }
            }

            return true;
        }
    }

    private bool RequiresEscaping(ReadOnlySpan<char> value)
    {
        if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
        {
            return value.IndexOfAny(_recordSeparationSymbol1, _fieldSeparationSymbol, _fieldQuoteSymbol) >= 0;
        }
        else
        {
            var foundRecordSeparator = false;

            while (!value.IsEmpty)
            {
                if (!foundRecordSeparator)
                {
                    var symbolIndex = value.IndexOfAny(_recordSeparationSymbol1, _fieldSeparationSymbol, _fieldQuoteSymbol);

                    if (symbolIndex < 0)
                    {
                        break;
                    }

                    var symbol = value[symbolIndex];

                    if (symbol != _recordSeparationSymbol1)
                    {
                        return true;
                    }

                    foundRecordSeparator = true;
                    value = value[(symbolIndex + 1)..];
                }
                else
                {
                    var symbol = value[0];

                    if ((symbol == _recordSeparationSymbol2) ||
                        (symbol == _fieldSeparationSymbol) ||
                        (symbol == _fieldQuoteSymbol))
                    {
                        return true;
                    }

                    foundRecordSeparator = false;
                    value = value[1..];
                }
            }

            return false;
        }
    }

    private bool RequiresEscaping(in ReadOnlySequence<char> value)
    {
        var reader = new SequenceReader<char>(value);

        if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
        {
            while (!reader.End)
            {
                if (reader.UnreadSpan.IndexOfAny(_recordSeparationSymbol1, _fieldSeparationSymbol, _fieldQuoteSymbol) >= 0)
                {
                    return true;
                }

                reader.Advance(reader.UnreadSpan.Length);
            }

            return false;
        }
        else
        {
            var foundLineTerminator = false;

            while (!reader.End)
            {
                var currentSegment = reader.UnreadSpan;

                if (!foundLineTerminator)
                {
                    var symbolIndex = currentSegment.IndexOfAny(_recordSeparationSymbol1, _fieldSeparationSymbol, _fieldQuoteSymbol);

                    if (symbolIndex < 0)
                    {
                        reader.Advance(currentSegment.Length);

                        continue;
                    }

                    var symbol = currentSegment[symbolIndex];

                    if (symbol != _recordSeparationSymbol1)
                    {
                        return true;
                    }

                    foundLineTerminator = true;
                    reader.Advance(symbolIndex + 1);
                }
                else
                {
                    var result = reader.TryRead(out var symbol);

                    Debug.Assert(result);

                    if ((symbol == _recordSeparationSymbol2) ||
                        (symbol == _fieldSeparationSymbol) ||
                        (symbol == _fieldQuoteSymbol))
                    {
                        return true;
                    }

                    foundLineTerminator = false;
                }
            }

            return false;
        }
    }

    public bool SupportsComments
    {
        get
        {
            return _commentPrefix is not null;
        }
    }
}
