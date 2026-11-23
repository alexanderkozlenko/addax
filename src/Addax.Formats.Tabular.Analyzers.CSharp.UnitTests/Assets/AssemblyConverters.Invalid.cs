using Addax.Formats.Tabular;
using Addax.Formats.Tabular.Converters;

[assembly: TabularConverter<TabularBase16ArrayConverter>]
[assembly: TabularConverter<TabularBase64ArrayConverter>]
[assembly: TabularConverter<MyNamespace.NullableInt32Converter>]

namespace MyNamespace;

public sealed class NullableInt32Converter : TabularConverter<int?>
{
}

[TabularRecord]
public struct MyRecord
{
    [TabularFieldOrder(0)]
    public byte[]? MyValue0;

    [TabularFieldOrder(1)]
    public int MyValue1;
}
