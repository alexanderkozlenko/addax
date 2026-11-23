using System;
using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular;
using Addax.Formats.Tabular.Converters;

namespace MyNamespace;

[TabularRecord]
public record PositionalRecord(
    [property: TabularFieldOrder(0)] int MyValue0,
    [property: TabularFieldOrder(1)] string? MyValue1);

[TabularRecord]
public record PositionalWithReversedOrdersRecord(
    [property: TabularFieldOrder(1)] int MyValue1,
    [property: TabularFieldOrder(0)] string? MyValue0);

[TabularRecord]
public sealed record PositionalWithConverterAndKeywordsRecord(
    [property: TabularFieldOrder(0)][property: TabularConverter<TabularInt32Converter>] int? @class,
    [property: TabularFieldOrder(1)] Guid @event);

[TabularRecord]
public readonly record struct ReadOnlyPositionalStructRecord(
    [property: TabularFieldOrder(0)] int MyValue0);

[TabularRecord]
public record struct PositionalStructWithExtraGetOnlyMemberRecord(
    [property: TabularFieldOrder(0)] int MyValue0)
{
    public int MyValue1
    {
        get;
    } = MyValue0 * 2;
}

[TabularRecord]
public record PositionalWithCaseDistinctParametersRecord(
    [property: TabularFieldOrder(0)] int MyValue0,
    [property: TabularFieldOrder(1)] int myValue0);

[TabularRecord]
public record PositionalWithExtraConstructorRecord(
    [property: TabularFieldOrder(0)] int MyValue0)
{
    public PositionalWithExtraConstructorRecord(int myValue0, int myValue1)
        : this(myValue0 + myValue1)
    {
    }
}

[TabularRecord]
public record PositionalWithExtraSettableMemberRecord(
    [property: TabularFieldOrder(0)] string MyValue0)
{
    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        init;
    }
}

[TabularRecord]
public record PositionalWithUnmappedParameterRecord(
    [property: TabularFieldOrder(0)] int MyValue0,
    int MyValue1);

[TabularRecord]
public record PositionalWithOptionalParameterRecord(
    [property: TabularFieldOrder(0)] int MyValue0 = 5);

[TabularRecord]
public record PositionalWithInParameterRecord(
    [property: TabularFieldOrder(0)] in int MyValue0);

[TabularRecord]
public record PositionalWithParamsParameterRecord(
    [property: TabularFieldOrder(0)][property: TabularConverter<TabularBase16ArrayConverter>] params byte[] MyValue0);

[TabularRecord]
public record struct PositionalStructWithExtraSettableMemberRecord(
    [property: TabularFieldOrder(0)] int MyValue0)
{
    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public readonly struct ReadOnlyStructWithPrimaryConstructorRecord(int myValue0)
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
    } = myValue0;
}

[TabularRecord]
public class ClassWithPrimaryConstructorRecord(int myValue0, long? myValue1)
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
    } = myValue0;

    [TabularFieldOrder(1)]
    public long? MyValue1
    {
        get;
    } = myValue1;
}

[TabularRecord]
public class ClassWithSeveralConstructorsRecord
{
    public ClassWithSeveralConstructorsRecord()
    {
    }

    public ClassWithSeveralConstructorsRecord(int myValue0)
    {
        MyValue0 = myValue0;
    }

    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class ClassWithOptionalConstructorRecord
{
    public ClassWithOptionalConstructorRecord(int myValue1 = 0)
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
public class ClassWithTwoOptionalConstructorsRecord
{
    public ClassWithTwoOptionalConstructorsRecord(int myValue1 = 0)
    {
    }

    public ClassWithTwoOptionalConstructorsRecord(string? myValue1 = null)
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
public class ClassWithPrivateConstructorRecord
{
    private ClassWithPrivateConstructorRecord()
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
public abstract class AbstractClassRecord
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public readonly struct ReadOnlyStructWithConstructorRecord
{
    public ReadOnlyStructWithConstructorRecord(int myValue0)
    {
        MyValue0 = myValue0;
    }

    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
    }
}

[TabularRecord]
public struct StructWithParameterizedConstructorRecord
{
    public StructWithParameterizedConstructorRecord(int myValue0)
    {
        MyValue0 = myValue0;
    }

    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public struct StructWithParameterlessAndParameterizedConstructorsRecord
{
    public StructWithParameterlessAndParameterizedConstructorsRecord()
    {
    }

    public StructWithParameterlessAndParameterizedConstructorsRecord(int myValue0)
    {
        MyValue0 = myValue0;
    }

    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class ClassWithVarargConstructorRecord
{
    public ClassWithVarargConstructorRecord(__arglist)
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
public struct StructWithVarargConstructorRecord
{
    public StructWithVarargConstructorRecord(__arglist)
    {
    }

    [TabularFieldOrder(0)]
    public int MyValue0;
}

[TabularRecord]
public class ClassWithParameterlessAndVarargConstructorsRecord
{
    public ClassWithParameterlessAndVarargConstructorsRecord()
    {
    }

    public ClassWithParameterlessAndVarargConstructorsRecord(__arglist)
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
public class RequiredMappedMemberRecord
{
    [TabularFieldOrder(0)]
    public required int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class RequiredUnmappedMemberRecord
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }

    public required string MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public class RequiredUnmappedMemberWithSetsRequiredMembersRecord
{
    [SetsRequiredMembers]
    public RequiredUnmappedMemberWithSetsRequiredMembersRecord()
    {
        MyValue1 = string.Empty;
    }

    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }

    public required string MyValue1
    {
        get;
        set;
    }
}

public class RequiredUnmappedMemberBase
{
    public required string MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public class RequiredUnmappedMemberInBaseRecord : RequiredUnmappedMemberBase
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

public class RequiredMappedMemberBase
{
    [TabularFieldOrder(0)]
    public required int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class RequiredMappedMemberInBaseRecord : RequiredMappedMemberBase
{
    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public record PositionalWithRequiredMemberRecord(
    [property: TabularFieldOrder(0)] int MyValue0)
{
    public required int MyValue1
    {
        get;
        init;
    }
}

[TabularRecord]
public class ClassPrimaryConstructorWithRequiredMemberRecord(int myValue0)
{
    [TabularFieldOrder(0)]
    public required int MyValue0
    {
        get;
        set;
    } = myValue0;
}
