// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

internal enum TabularStreamParsingTokenType
{
    None,
    FieldValue,
    FieldQuoteBegin,
    FieldQuoteEnd,
    FieldEscape,
    RecordSeparation,
    CommentPrefix,
}
