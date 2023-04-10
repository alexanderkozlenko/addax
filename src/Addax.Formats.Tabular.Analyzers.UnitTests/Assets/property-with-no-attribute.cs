using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord(strict: false)]
internal sealed class MyType
{
    [TabularField(index: 0)]
    public string? MyProperty0 { get; set; }

    public string? MyProperty1 { get; set; }
}
