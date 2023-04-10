using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord(strict: false)]
internal sealed class MyType
{
    private string? _myProperty0;

    [TabularField(index: 0)]
    public string? MyProperty0
    {
        set
        {
            _myProperty0 = value;
        }
    }

    [TabularField(index: 1)]
    public string? MyProperty1 { get; set; }
}
