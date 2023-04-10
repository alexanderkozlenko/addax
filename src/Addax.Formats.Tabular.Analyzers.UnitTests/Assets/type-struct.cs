using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord(strict: false)]
internal struct MyType
{
    [TabularField(index: 0)]
    public string? MyProperty0 { get; init; }
}
