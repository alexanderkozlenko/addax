using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public readonly struct MyType
{
    public MyType(string myValue)
    {
        MyValue = myValue;
    }

    [TabularFieldOrder(0)]
    public readonly string MyValue;
}
