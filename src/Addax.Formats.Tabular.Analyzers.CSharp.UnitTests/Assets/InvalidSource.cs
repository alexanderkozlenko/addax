using System;
using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public class PointerArrayMemberRecord
{
    [TabularFieldOrder(0)]
    public int*[] MyValue0;
}

[TabularRecord]
public class PointerMemberRecord
{
    [TabularFieldOrder(0)]
    public int* MyValue0;
}

public sealed class GenericConverter<T> : TabularConverter<int>
{
}

[TabularRecord]
public class PointerTypeArgumentRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<GenericConverter<int*[]>>]
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
