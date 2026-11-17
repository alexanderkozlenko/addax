using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public class MyRecord
{
    [TabularFieldOrder(0)]
    private int MyValue0
    {
        get;
        set;
    }
}
