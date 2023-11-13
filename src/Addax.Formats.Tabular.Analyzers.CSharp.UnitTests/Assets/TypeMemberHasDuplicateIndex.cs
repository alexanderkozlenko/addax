using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public struct MyType
{
    [TabularFieldOrder(0)]
    public string? MyValue0;

    [TabularFieldOrder(0)]
    public string? MyValue1;
}
