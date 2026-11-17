using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public class PositionsRecord
{
    [TabularFieldOrder(2)]
    public double MyValue2
    {
        get;
        set;
    }

    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }

    [TabularFieldOrder(1)]
    public string? MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public class SkippedFieldsRecord
{
    [TabularFieldOrder(0)]
    public string MyValue0
    {
        get
        {
            return "-";
        }
    }

    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        set;
    }

    [TabularFieldOrder(2)]
    public string MyValue2
    {
        get
        {
            return "-";
        }
    }

    [TabularFieldOrder(3)]
    public string MyValue3
    {
        get
        {
            return "-";
        }
    }

    [TabularFieldOrder(4)]
    public int MyValue4
    {
        get;
        set;
    }
}

[TabularRecord]
public struct LeadingGapRecord
{
    [TabularFieldOrder(1)]
    public string? MyValue1;
}

[TabularRecord]
public class HeaderRecord
{
    [TabularFieldOrder(0)]
    [TabularFieldName("a,\"b\"")]
    public int MyValue0
    {
        get;
        set;
    }

    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public struct DuplicateOrderRecord
{
    [TabularFieldOrder(0)]
    public string? MyValue0;

    [TabularFieldOrder(0)]
    public string? MyValue1;
}

[TabularRecord]
public struct NegativeOrderRecord
{
    [TabularFieldOrder(-1)]
    public string? MyValue0;
}

[TabularRecord]
public struct NullNameRecord
{
    [TabularFieldName(null!)]
    [TabularFieldOrder(0)]
    public string? MyValue0;
}
