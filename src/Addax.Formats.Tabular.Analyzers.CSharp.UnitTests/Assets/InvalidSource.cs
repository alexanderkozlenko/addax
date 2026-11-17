using System;
using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public class NonConstantOrderRecord
{
    [TabularFieldOrder(int.Parse("0"))]
    public int MyValue0
    {
        get;
        set;
    }
}

public sealed class RefStructConverter : TabularConverter<Span<int>>
{
}

[TabularRecord]
public class RefStructMemberWithConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<RefStructConverter>]
    public Span<int> MyValue0
    {
        get
        {
            return default;
        }
        set
        {
        }
    }
}
