// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Addax.Formats.Tabular.Buffers;
using Addax.Formats.Tabular.Collections;

namespace Addax.Formats.Tabular;

internal sealed class TabularParser
{
    private readonly string _tokenT;
    private readonly char _tokenD;
    private readonly char _tokenQ;
    private readonly char _tokenE;
    private readonly char? _tokenA;

    private readonly TabularSearchValues _searchValues;

    public TabularParser(TabularDialect dialect)
    {
        Debug.Assert(dialect is not null);

        _tokenT = dialect.LineTerminator;
        _tokenD = dialect.Delimiter;
        _tokenQ = dialect.QuoteSymbol;
        _tokenE = dialect.EscapeSymbol;
        _tokenA = dialect.AnnotationPrefix;

        _searchValues = dialect.SearchValues;
    }

    public bool TryParse(ReadOnlySpan<char> source, TabularParsingMode mode, ref TabularParserState state, PooledQueue<TabularFieldInfo> fields, ref int consumed)
    {
        Debug.Assert(fields is not null);
        Debug.Assert(consumed >= 0);

        var length = source.Length;

        while (!source.IsEmpty)
        {
            switch (state.CurrentArea)
            {
                case TabularParsingArea.None:
                    {
                        var symbol = source[0];

                        state.CharsParsed++;
                        source = source.Slice(1);

                        if (symbol == _tokenD)
                        {
                            StoreField(fields, ref state, TabularSeparator.Delimiter);

                            mode &= ~TabularParsingMode.StartOfRecord;
                        }
                        else if (symbol == _tokenT[0])
                        {
                            if (_tokenT.Length == 1)
                            {
                                StoreField(fields, ref state, TabularSeparator.LineTerminator);

                                mode |= TabularParsingMode.StartOfRecord;

                                continue;
                            }

                            state.CurrentArea = TabularParsingArea.LineTerminator;
                        }
                        else if (symbol == _tokenQ)
                        {
                            state.CurrentArea = TabularParsingArea.QuotedValue;
                            state.FoundQuoting = true;
                        }
                        else
                        {
                            if (((mode & TabularParsingMode.StartOfRecord) != 0) && _tokenA.HasValue && (symbol == _tokenA.Value))
                            {
                                state.CurrentArea = TabularParsingArea.Annotation;
                                state.FoundAnnotation = true;
                            }
                            else
                            {
                                state.CurrentArea = TabularParsingArea.Value;
                            }
                        }
                    }
                    break;
                case TabularParsingArea.Value:
                    {
                        var index = source.IndexOfAny(_searchValues.ValuesT0DQ);

                        if (index < 0)
                        {
                            state.CharsParsed += source.Length;
                            source = default;

                            continue;
                        }

                        var symbol = source[index];

                        state.CharsParsed += index + 1;
                        source = source.Slice(index + 1);

                        if (symbol == _tokenD)
                        {
                            StoreField(fields, ref state, TabularSeparator.Delimiter);

                            mode &= ~TabularParsingMode.StartOfRecord;
                        }
                        else if (symbol == _tokenT[0])
                        {
                            if (_tokenT.Length == 1)
                            {
                                StoreField(fields, ref state, TabularSeparator.LineTerminator);

                                mode |= TabularParsingMode.StartOfRecord;

                                continue;
                            }

                            state.CurrentArea = TabularParsingArea.LineTerminator;
                        }
                        else
                        {
                            consumed += length - source.Length;

                            return false;
                        }
                    }
                    break;
                case TabularParsingArea.QuotedValue:
                    {
                        if (_tokenE != _tokenQ)
                        {
                            var index = source.IndexOfAny(_searchValues.ValuesQE);

                            if (index < 0)
                            {
                                state.CharsParsed += source.Length;
                                source = default;

                                continue;
                            }

                            var symbol = source[index];

                            state.CharsParsed += index + 1;
                            source = source.Slice(index + 1);

                            if (symbol == _tokenE)
                            {
                                state.CurrentArea = TabularParsingArea.EscapedSymbol;
                                state.CharsEscaped++;
                            }
                            else
                            {
                                state.CurrentArea = TabularParsingArea.QuotedValueTail;
                            }
                        }
                        else
                        {
                            var index = source.IndexOfAny(_searchValues.ValuesQ);

                            if (index < 0)
                            {
                                state.CharsParsed += source.Length;
                                source = default;

                                continue;
                            }

                            state.CharsParsed += index + 1;
                            source = source.Slice(index + 1);
                            state.CurrentArea = TabularParsingArea.QuotedValueTail;
                        }
                    }
                    break;
                case TabularParsingArea.QuotedValueTail:
                    {
                        var symbol = source[0];

                        state.CharsParsed++;
                        source = source.Slice(1);

                        if ((_tokenE == _tokenQ) && (symbol == _tokenQ))
                        {
                            state.CurrentArea = TabularParsingArea.QuotedValue;
                            state.CharsEscaped++;

                            continue;
                        }

                        if (symbol == _tokenD)
                        {
                            StoreField(fields, ref state, TabularSeparator.Delimiter);

                            mode &= ~TabularParsingMode.StartOfRecord;
                        }
                        else if (symbol == _tokenT[0])
                        {
                            if (_tokenT.Length == 1)
                            {
                                StoreField(fields, ref state, TabularSeparator.LineTerminator);

                                mode |= TabularParsingMode.StartOfRecord;

                                continue;
                            }

                            state.CurrentArea = TabularParsingArea.LineTerminator;
                        }
                        else
                        {
                            consumed += length - source.Length;

                            return false;
                        }
                    }
                    break;
                case TabularParsingArea.EscapedSymbol:
                    {
                        var symbol = source[0];

                        state.CharsParsed++;
                        source = source.Slice(1);

                        if ((symbol == _tokenQ) || (symbol == _tokenE))
                        {
                            state.CurrentArea = TabularParsingArea.QuotedValue;
                        }
                        else
                        {
                            consumed += length - source.Length;

                            return false;
                        }
                    }
                    break;
                case TabularParsingArea.LineTerminator:
                    {
                        var symbol = source[0];

                        state.CharsParsed++;
                        source = source.Slice(1);

                        if (symbol == _tokenT[1])
                        {
                            StoreField(fields, ref state, TabularSeparator.LineTerminator);

                            mode |= TabularParsingMode.StartOfRecord;
                        }
                        else if (!state.FoundQuoting)
                        {
                            if (symbol == _tokenD)
                            {
                                StoreField(fields, ref state, TabularSeparator.Delimiter);

                                mode &= ~TabularParsingMode.StartOfRecord;
                            }
                            else if (symbol == _tokenT[0])
                            {
                                state.CurrentArea = TabularParsingArea.LineTerminator;
                            }
                            else if (symbol == _tokenQ)
                            {
                                consumed += length - source.Length;

                                return false;
                            }
                            else
                            {
                                state.CurrentArea = TabularParsingArea.Value;
                            }
                        }
                        else
                        {
                            consumed += length - source.Length;

                            return false;
                        }
                    }
                    break;
                case TabularParsingArea.Annotation:
                    {
                        var index = source.IndexOfAny(_searchValues.ValuesT0);

                        if (index < 0)
                        {
                            state.CharsParsed += source.Length;
                            source = default;

                            continue;
                        }

                        state.CharsParsed += index + 1;
                        source = source.Slice(index + 1);

                        if (_tokenT.Length == 1)
                        {
                            StoreField(fields, ref state, TabularSeparator.LineTerminator);

                            mode |= TabularParsingMode.StartOfRecord;

                            continue;
                        }

                        state.CurrentArea = TabularParsingArea.LineTerminator;
                    }
                    break;
            }
        }

        consumed += length;

        if ((mode & TabularParsingMode.EndOfStream) != 0)
        {
            if (state.FoundQuoting)
            {
                if ((state.CurrentArea == TabularParsingArea.QuotedValue) ||
                    (state.CurrentArea == TabularParsingArea.EscapedSymbol) ||
                    (state.CurrentArea == TabularParsingArea.LineTerminator))
                {
                    return false;
                }
            }

            StoreField(fields, ref state, TabularSeparator.None);
        }

        return true;
    }

    public void ReadField(ReadOnlyMemory<char> source, ref readonly TabularFieldInfo fieldInfo, ref ArrayBuffer<char> field)
    {
        if (!fieldInfo.IsAnnotation)
        {
            var offset = fieldInfo.HasQuoting ? 1 : 0;
            var length = fieldInfo.CharsUsed - (2 * offset);

            switch (fieldInfo.Separator)
            {
                case TabularSeparator.Delimiter:
                    {
                        length--;
                    }
                    break;
                case TabularSeparator.LineTerminator:
                    {
                        length -= _tokenT.Length;
                    }
                    break;
            }

            if (length > 0)
            {
                source = source.Slice(offset, length);

                if (fieldInfo.CharsEscaped == 0)
                {
                    MemoryMarshal.TryGetArray(source, out var segment);

                    field = new(segment);

                    return;
                }
                else
                {
                    field = new(length - fieldInfo.CharsEscaped);

                    UnescapeField(source.Span, field.AsSpan(), _tokenE);
                }
            }
        }
        else
        {
            var length = fieldInfo.CharsUsed - 1;

            switch (fieldInfo.Separator)
            {
                case TabularSeparator.LineTerminator:
                    {
                        length -= _tokenT.Length;
                    }
                    break;
            }

            if (length > 0)
            {
                field = new(length);
                source = source.Slice(1, length);
                source.Span.CopyTo(field.AsSpan());
            }
        }
    }

    private static void UnescapeField(ReadOnlySpan<char> source, Span<char> target, char token)
    {
        var escape = false;
        var j = 0;

        for (var i = 0; i < source.Length; i++)
        {
            var value = source[i];

            escape = (value == token) && !escape;

            if (!escape)
            {
                target[j++] = value;
            }
        }
    }

    private static void StoreField(PooledQueue<TabularFieldInfo> fields, ref TabularParserState state, TabularSeparator separator)
    {
        var fieldInfo = new TabularFieldInfo(
            state.CharsParsed,
            state.CharsEscaped,
            separator,
            state.FoundQuoting,
            state.FoundAnnotation);

        fields.Enqueue(fieldInfo);
        state = default;
    }
}
