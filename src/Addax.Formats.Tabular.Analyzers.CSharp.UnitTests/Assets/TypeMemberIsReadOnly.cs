using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public readonly struct MyType(string myValue)
{
    [TabularFieldOrder(0)]
    public readonly string MyValue = myValue;
}
