// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

internal struct TabularStreamParserState
{
    public TabularStreamParsingTokenType LastTokenType;
    public bool IsBeginningOfLine;
    public bool IsFieldQuoted;
    public bool IsFieldEscapeFound;
    public bool IsCommentPrefixFound;

    public bool IsIncomplete
    {
        get
        {
            return
                IsFieldQuoted &&
                LastTokenType is
                    TabularStreamParsingTokenType.FieldQuoteBegin or
                    TabularStreamParsingTokenType.FieldEscape or
                    TabularStreamParsingTokenType.RecordSeparation;
        }
    }
}
