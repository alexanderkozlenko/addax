// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

internal sealed class TabularStreamParser
{
    private readonly char _recordSeparationSymbol1;
    private readonly char _recordSeparationSymbol2;
    private readonly char _fieldSeparationSymbol;
    private readonly char _fieldQuoteSymbol;
    private readonly char _fieldEscapeSymbol;
    private readonly char? _commentPrefix;

    public TabularStreamParser(TabularDataDialect dialect)
    {
        _recordSeparationSymbol1 = dialect.LineTerminator[0];
        _recordSeparationSymbol2 = dialect.LineTerminator[^1];
        _fieldSeparationSymbol = dialect.Delimiter;
        _fieldQuoteSymbol = dialect.QuoteChar;
        _fieldEscapeSymbol = dialect.EscapeChar;
        _commentPrefix = dialect.CommentPrefix;
    }

    public TabularStreamParsingStatus Parse(ReadOnlySpan<char> buffer, ref TabularStreamParserState state, out long parsed)
    {
        parsed = 0;

        while (!buffer.IsEmpty)
        {
            switch (state.LastTokenType)
            {
                case TabularStreamParsingTokenType.None:
                    {
                        var symbol = buffer[0];

                        parsed += 1;
                        buffer = buffer[1..];

                        if (symbol == _fieldSeparationSymbol)
                        {
                            return TabularStreamParsingStatus.FoundFieldSeparation;
                        }
                        else if (symbol == _recordSeparationSymbol1)
                        {
                            if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
                            {
                                return TabularStreamParsingStatus.FoundRecordSeparation;
                            }

                            state.LastTokenType = TabularStreamParsingTokenType.RecordSeparation;
                        }
                        else if (symbol == _fieldQuoteSymbol)
                        {
                            state.LastTokenType = TabularStreamParsingTokenType.FieldQuoteBegin;
                            state.IsFieldQuoted = true;
                        }
                        else
                        {
                            if (state.IsBeginningOfLine && _commentPrefix.HasValue && (symbol == _commentPrefix.Value))
                            {
                                state.LastTokenType = TabularStreamParsingTokenType.CommentPrefix;
                                state.IsCommentPrefixFound = true;
                            }
                            else
                            {
                                state.LastTokenType = TabularStreamParsingTokenType.FieldValue;
                            }
                        }
                    }
                    break;
                case TabularStreamParsingTokenType.FieldValue:
                    {
                        var symbolIndex = buffer.IndexOfAny(_fieldSeparationSymbol, _recordSeparationSymbol1, _fieldQuoteSymbol);

                        if (symbolIndex < 0)
                        {
                            parsed += buffer.Length;
                            buffer = ReadOnlySpan<char>.Empty;

                            continue;
                        }

                        var symbol = buffer[symbolIndex];

                        parsed += symbolIndex + 1;
                        buffer = buffer[(symbolIndex + 1)..];

                        if (symbol == _fieldSeparationSymbol)
                        {
                            return TabularStreamParsingStatus.FoundFieldSeparation;
                        }
                        else if (symbol == _recordSeparationSymbol1)
                        {
                            if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
                            {
                                return TabularStreamParsingStatus.FoundRecordSeparation;
                            }

                            state.LastTokenType = TabularStreamParsingTokenType.RecordSeparation;
                        }
                        else
                        {
                            return TabularStreamParsingStatus.FoundInvalidData;
                        }
                    }
                    break;
                case TabularStreamParsingTokenType.FieldQuoteBegin:
                    {
                        if (_fieldEscapeSymbol != _fieldQuoteSymbol)
                        {
                            var symbolIndex = buffer.IndexOfAny(_fieldEscapeSymbol, _fieldQuoteSymbol);

                            if (symbolIndex < 0)
                            {
                                parsed += buffer.Length;
                                buffer = ReadOnlySpan<char>.Empty;

                                continue;
                            }

                            var symbol = buffer[symbolIndex];

                            parsed += symbolIndex + 1;
                            buffer = buffer[(symbolIndex + 1)..];

                            if (symbol == _fieldEscapeSymbol)
                            {
                                state.LastTokenType = TabularStreamParsingTokenType.FieldEscape;
                                state.IsFieldEscapeFound = true;
                            }
                            else
                            {
                                state.LastTokenType = TabularStreamParsingTokenType.FieldQuoteEnd;
                            }
                        }
                        else
                        {
                            var symbolIndex = buffer.IndexOf(_fieldQuoteSymbol);

                            if (symbolIndex < 0)
                            {
                                parsed += buffer.Length;
                                buffer = ReadOnlySpan<char>.Empty;

                                continue;
                            }

                            parsed += symbolIndex + 1;
                            buffer = buffer[(symbolIndex + 1)..];
                            state.LastTokenType = TabularStreamParsingTokenType.FieldQuoteEnd;
                        }
                    }
                    break;
                case TabularStreamParsingTokenType.FieldQuoteEnd:
                    {
                        var symbol = buffer[0];

                        parsed += 1;
                        buffer = buffer[1..];

                        if ((_fieldEscapeSymbol == _fieldQuoteSymbol) && (symbol == _fieldQuoteSymbol))
                        {
                            state.LastTokenType = TabularStreamParsingTokenType.FieldQuoteBegin;
                            state.IsFieldEscapeFound = true;

                            continue;
                        }

                        if (symbol == _fieldSeparationSymbol)
                        {
                            return TabularStreamParsingStatus.FoundFieldSeparation;
                        }
                        else if (symbol == _recordSeparationSymbol1)
                        {
                            if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
                            {
                                return TabularStreamParsingStatus.FoundRecordSeparation;
                            }

                            state.LastTokenType = TabularStreamParsingTokenType.RecordSeparation;
                        }
                        else
                        {
                            return TabularStreamParsingStatus.FoundInvalidData;
                        }
                    }
                    break;
                case TabularStreamParsingTokenType.FieldEscape:
                    {
                        var symbol = buffer[0];

                        parsed += 1;
                        buffer = buffer[1..];

                        if ((symbol != _fieldQuoteSymbol) && (symbol != _fieldEscapeSymbol))
                        {
                            return TabularStreamParsingStatus.FoundInvalidData;
                        }

                        state.LastTokenType = TabularStreamParsingTokenType.FieldQuoteBegin;
                    }
                    break;
                case TabularStreamParsingTokenType.RecordSeparation:
                    {
                        var symbol = buffer[0];

                        parsed += 1;
                        buffer = buffer[1..];

                        if (symbol == _recordSeparationSymbol2)
                        {
                            return TabularStreamParsingStatus.FoundRecordSeparation;
                        }
                        else if (!state.IsFieldQuoted)
                        {
                            state.LastTokenType = TabularStreamParsingTokenType.FieldValue;
                        }
                        else
                        {
                            return TabularStreamParsingStatus.FoundInvalidData;
                        }
                    }
                    break;
                case TabularStreamParsingTokenType.CommentPrefix:
                    {
                        var symbolIndex = buffer.IndexOf(_recordSeparationSymbol1);

                        if (symbolIndex < 0)
                        {
                            parsed += buffer.Length;
                            buffer = ReadOnlySpan<char>.Empty;

                            continue;
                        }

                        parsed += symbolIndex + 1;
                        buffer = buffer[(symbolIndex + 1)..];

                        if (_recordSeparationSymbol1 == _recordSeparationSymbol2)
                        {
                            return TabularStreamParsingStatus.FoundRecordSeparation;
                        }

                        state.LastTokenType = TabularStreamParsingTokenType.RecordSeparation;
                    }
                    break;
            }
        }

        return TabularStreamParsingStatus.NeedMoreData;
    }

    public ReadOnlySequence<char> Extract(in ReadOnlySequence<char> buffer, long bufferLength, TabularStreamParsingStatus status, in TabularStreamParserState state, SequenceSource<char> bufferSource, out BufferKind bufferKind)
    {
        if (!state.IsCommentPrefixFound)
        {
            var valueMargin = state.IsFieldQuoted ? 1 : 0;
            var valueLength = bufferLength - (2 * valueMargin);

            switch (status)
            {
                case TabularStreamParsingStatus.FoundFieldSeparation:
                    {
                        valueLength -= 1;
                    }
                    break;
                case TabularStreamParsingStatus.FoundRecordSeparation:
                    {
                        valueLength -= _recordSeparationSymbol1 == _recordSeparationSymbol2 ? 1 : 2;
                    }
                    break;
            }

            if (valueLength == 0)
            {
                bufferKind = BufferKind.None;

                return ReadOnlySequence<char>.Empty;
            }

            var value = buffer.Slice(valueMargin, valueLength);

            if (!state.IsFieldEscapeFound)
            {
                bufferKind = BufferKind.Shared;

                return value;
            }
            else
            {
                bufferKind = BufferKind.Private;

                Unescape(value, bufferSource, _fieldEscapeSymbol);

                return bufferSource.ToSequence();
            }
        }
        else
        {
            Debug.Assert(status is not TabularStreamParsingStatus.FoundFieldSeparation);

            bufferKind = BufferKind.Shared;

            var valueLength = bufferLength - 1;

            if (status is TabularStreamParsingStatus.FoundRecordSeparation)
            {
                valueLength -= _recordSeparationSymbol1 == _recordSeparationSymbol2 ? 1 : 2;
            }

            var value = buffer.Slice(1, valueLength);

            return value;
        }
    }

    private static void Unescape(in ReadOnlySequence<char> value, IBufferWriter<char> writer, char escapeSymbol)
    {
        var reader = new SequenceReader<char>(value);

        while (!reader.End)
        {
            var currentSegment = reader.UnreadSpan;
            var escapeIndex = currentSegment.IndexOf(escapeSymbol);

            if (escapeIndex >= 0)
            {
                var sourceFragment = currentSegment[..escapeIndex];
                var bufferLength = escapeIndex + 1;
                var buffer = writer.GetSpan(bufferLength);

                sourceFragment.CopyTo(buffer);
                buffer[bufferLength - 1] = currentSegment[escapeIndex + 1];
                reader.Advance(bufferLength + 1);
                writer.Advance(bufferLength);
            }
            else
            {
                var bufferLength = currentSegment.Length;
                var buffer = writer.GetSpan(bufferLength);

                currentSegment.CopyTo(buffer);
                reader.Advance(bufferLength);
                writer.Advance(bufferLength);
            }
        }
    }
}
