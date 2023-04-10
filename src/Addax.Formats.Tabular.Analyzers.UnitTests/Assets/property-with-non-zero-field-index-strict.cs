using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord(strict: true)]
internal sealed class MyType
{
    [TabularField(index: 1)]
    public string? MyProperty1 { get; set; }

    [TabularField(index: 3)]
    public string? MyProperty3 { get; set; }
}
