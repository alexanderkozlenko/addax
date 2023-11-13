using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public struct MyType
{
    [TabularFieldOrder(1)]
    public string MyValue;
}
