#pragma warning disable EXP0101

using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public class ExperimentalAssemblyRecord : MyLibrary.MyBase
{
    [TabularFieldOrder(1)]
    [TabularConverter<MyLibrary.MyConverter>]
    public int MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public class HiddenReferencedMemberRecord : MyLibrary.MyBase
{
    public new long MyValue0
    {
        get;
        set;
    }
}
