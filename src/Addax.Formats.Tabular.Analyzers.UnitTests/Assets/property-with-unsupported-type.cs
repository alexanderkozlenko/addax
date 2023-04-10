using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord(strict: false)]
internal sealed class MyType
{
    [TabularField(index: 0)]
    public object? MyProperty0 { get; set; }
}
