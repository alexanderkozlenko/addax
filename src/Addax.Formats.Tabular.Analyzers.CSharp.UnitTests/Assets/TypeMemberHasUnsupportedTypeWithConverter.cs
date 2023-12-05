using Addax.Formats.Tabular;
using Addax.Formats.Tabular.Converters;

namespace MyNamespace;

[TabularRecord]
public struct MyType
{
    [TabularFieldOrder(0)]
    [TabularConverter<TabularBase16ArrayConverter>]
    public byte[]? MyValue0;

    [TabularFieldOrder(1)]
    [TabularConverter<TabularBase64ArrayConverter>]
    public byte[]? MyValue1;
}
