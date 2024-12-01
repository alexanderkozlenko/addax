// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Primitives;

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
