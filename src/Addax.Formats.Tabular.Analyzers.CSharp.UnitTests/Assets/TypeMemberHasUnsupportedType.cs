using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public struct MyType
{
    [TabularFieldOrder(0)]
    public object? MyValue0;
}
