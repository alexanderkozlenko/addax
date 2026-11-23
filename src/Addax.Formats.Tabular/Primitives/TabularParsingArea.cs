// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Primitives;

internal enum TabularParsingArea
{
    StartOfRecord,
    StartOfField,
    Value,
    ValueLineTerminator,
    QuotedValue,
    QuotedValueEscape,
    QuotedValueTail,
    QuotedValueLineTerminator,
    Comment,
    CommentLineTerminator,
    EndOfStream,
}