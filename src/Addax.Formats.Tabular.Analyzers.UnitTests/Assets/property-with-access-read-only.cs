using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord(strict: false)]
internal sealed class MyType
{
    private string? _myProperty0 = "value0";

    [TabularField(index: 0)]
    public string? MyProperty0
    {
        get
        {
            return _myProperty0;
        }
    }

    [TabularField(index: 1)]
    public string? MyProperty1 { get; set; }
}
