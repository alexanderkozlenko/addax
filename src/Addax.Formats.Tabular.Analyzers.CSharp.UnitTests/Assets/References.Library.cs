using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular;

[assembly: Experimental("EXP0101")]

namespace MyLibrary;

public class MyBase
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

public sealed class MyConverter : TabularConverter<int>
{
}
