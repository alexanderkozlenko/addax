using Addax.Formats.Tabular;
using Addax.Formats.Tabular.Converters;

namespace MyNamespace;

[TabularRecord]
public struct MyType
{
    [TabularFieldOrder(0)]
    [TabularConverter<TabularBooleanConverter>]
    public string? MyValue0;
}
