using Addax.Formats.Tabular;

[assembly: TabularConverter<MyNamespace.MyConverter>]

namespace MyNamespace;

public sealed class MyConverter : TabularConverter<string>
{
}

public sealed class MyOtherConverter : TabularConverter<string>
{
}
