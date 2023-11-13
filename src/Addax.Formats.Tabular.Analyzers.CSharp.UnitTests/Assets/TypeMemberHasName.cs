using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public struct MyType
{
    [TabularFieldName("my-value")]
    [TabularFieldOrder(0)]
    public string MyValue;
}
