using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public struct MyRecord
{
    [TabularFieldName("my-value")]
    [TabularFieldOrder(0)]
    public string? MyValue0;
}
