// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

internal enum TabularParsingArea
{
    None,
    Value,
    QuotedValue,
    QuotedValueTail,
    EscapedSymbol,
    LineTerminator,
    Annotation,
}
