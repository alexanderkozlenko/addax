using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord(strict: false)]
internal sealed class MyType
{
    public MyType(string? myProperty0)
    {
        MyProperty0 = myProperty0;
    }

    [TabularField(index: 0)]
    public string? MyProperty0 { get; set; }
}
