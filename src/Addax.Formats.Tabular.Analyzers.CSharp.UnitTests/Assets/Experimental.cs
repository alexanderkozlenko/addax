#pragma warning disable EXP0001
#pragma warning disable EXP0002
#pragma warning disable EXP0003
#pragma warning disable EXP0004
#pragma warning disable EXP0005
#pragma warning disable EXP0006
#pragma warning disable EXP0007
#pragma warning disable EXP0008
#pragma warning disable EXP0009
#pragma warning disable EXP0010
#pragma warning disable EXP0011
#pragma warning disable EXP0012
#pragma warning disable EXP0013
#pragma warning disable EXP0014

using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
[Experimental("EXP0001")]
public class ExperimentalRecord
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[Experimental("EXP0002")]
public static class ExperimentalOuterType
{
    [TabularRecord]
    public class NestedInExperimentalRecord
    {
        [TabularFieldOrder(0)]
        public int MyValue0
        {
            get;
            set;
        }
    }
}

[TabularRecord]
public class ExperimentalConstructorRecord
{
    [Experimental("EXP0003")]
    public ExperimentalConstructorRecord()
    {
    }

    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
[method: Experimental("EXP0004")]
public record ExperimentalPrimaryConstructorRecord(
    [property: TabularFieldOrder(0)] int MyValue0);

[TabularRecord]
public class ExperimentalMembersRecord
{
    [TabularFieldOrder(0)]
    [Experimental("EXP0005")]
    public int MyValue0
    {
        get;
        set;
    }

    [TabularFieldOrder(1)]
    [Experimental("EXP0006")]
    public int MyValue1;
}

[TabularRecord]
public class ExperimentalAccessorsRecord
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        [Experimental("EXP0007")]
        get;
        set;
    }

    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        [Experimental("EXP0008")]
        set;
    }
}

[Experimental("EXP0009")]
public sealed class ExperimentalConverter : TabularConverter<int>
{
}

public sealed class ExperimentalConverterConstructor : TabularConverter<int>
{
    [Experimental("EXP0010")]
    public ExperimentalConverterConstructor()
    {
    }
}

[TabularRecord]
public class ExperimentalConvertersRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<ExperimentalConverter>]
    public int MyValue0
    {
        get;
        set;
    }

    [TabularFieldOrder(1)]
    [TabularConverter(typeof(ExperimentalConverterConstructor))]
    public int MyValue1
    {
        get;
        set;
    }
}

[Experimental("EXP0013")]
public sealed class ExperimentalTypeArgument
{
}

public sealed class GenericConverter<T> : TabularConverter<int>
{
}

[Experimental("EXP0014")]
public static class ExperimentalConverters
{
    public sealed class NestedConverter : TabularConverter<int>
    {
    }
}

[TabularRecord]
public class ExperimentalConverterTypesRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<GenericConverter<ExperimentalTypeArgument>>]
    public int MyValue0
    {
        get;
        set;
    }

    [TabularFieldOrder(1)]
    [TabularConverter<ExperimentalConverters.NestedConverter>]
    public int MyValue1
    {
        get;
        set;
    }
}

[Experimental("EXP0011")]
public enum ExperimentalEnum
{
    MyValue,
}

public sealed class ExperimentalEnumConverter : TabularConverter<ExperimentalEnum>
{
}

[TabularRecord]
public class ExperimentalValueTypeRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<ExperimentalEnumConverter>]
    public ExperimentalEnum? MyValue0
    {
        get;
        set;
    }
}

public class ExperimentalBase
{
    [Experimental("EXP0012")]
    public virtual int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class ExperimentalOverriddenMemberRecord : ExperimentalBase
{
    [TabularFieldOrder(0)]
    public override int MyValue0
    {
        get;
        set;
    }
}
