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
public record PositionalRecordWithReversedOrders(
    [property: TabularFieldOrder(1)] int MyValue1,
    [property: TabularFieldOrder(0)] string? MyValue0);

[TabularRecord]
public sealed record PositionalRecordWithConverterAndKeywords(
    [property: TabularFieldOrder(0)][property: TabularConverter<TabularInt32Converter>] int? @class,
    [property: TabularFieldOrder(1)] Guid @event);

[TabularRecord]
public readonly record struct ReadOnlyRecordStruct(
    [property: TabularFieldOrder(0)] int MyValue0);

[TabularRecord]
public record struct RecordStructWithExtraGetOnlyMember(
    [property: TabularFieldOrder(0)] int MyValue0)
{
    public int MyValue1
    {
        get;
    } = MyValue0 * 2;
}

[TabularRecord]
public record PositionalRecordWithCaseDistinctParameters(
    [property: TabularFieldOrder(0)] int MyValue0,
    [property: TabularFieldOrder(1)] int myValue0);

[TabularRecord]
public record PositionalRecordWithExtraConstructor(
    [property: TabularFieldOrder(0)] int MyValue0)
{
    public PositionalRecordWithExtraConstructor(int myValue0, int myValue1)
        : this(myValue0 + myValue1)
    {
    }
}

[TabularRecord]
public record PositionalRecordWithExtraSettableMember(
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
public record PositionalRecordWithUnmappedParameter(
    [property: TabularFieldOrder(0)] int MyValue0,
    int MyValue1);

[TabularRecord]
public record PositionalRecordWithOptionalParameter(
    [property: TabularFieldOrder(0)] int MyValue0 = 5);

[TabularRecord]
public record PositionalRecordWithInParameter(
    [property: TabularFieldOrder(0)] in int MyValue0);

[TabularRecord]
public record PositionalRecordWithParamsParameter(
    [property: TabularFieldOrder(0)][property: TabularConverter<TabularBase16ArrayConverter>] params byte[] MyValue0);

[TabularRecord]
public record struct RecordStructWithExtraSettableMember(
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
public readonly struct ReadOnlyStructWithPrimaryConstructor(int myValue0)
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
    } = myValue0;
}

[TabularRecord]
public class ClassWithPrimaryConstructor(int myValue0, long? myValue1)
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
public class ClassWithSeveralConstructors
{
    public ClassWithSeveralConstructors()
    {
    }

    public ClassWithSeveralConstructors(int myValue0)
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
public class ClassWithOptionalConstructor
{
    public ClassWithOptionalConstructor(int myValue1 = 0)
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
public class ClassWithTwoOptionalConstructors
{
    public ClassWithTwoOptionalConstructors(int myValue1 = 0)
    {
    }

    public ClassWithTwoOptionalConstructors(string? myValue1 = null)
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
public class ClassWithPrivateConstructor
{
    private ClassWithPrivateConstructor()
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
public abstract class AbstractClass
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public readonly struct ReadOnlyStructWithConstructor
{
    public ReadOnlyStructWithConstructor(int myValue0)
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
public struct StructWithParameterizedConstructor
{
    public StructWithParameterizedConstructor(int myValue0)
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
public struct StructWithParameterlessAndParameterizedConstructors
{
    public StructWithParameterlessAndParameterizedConstructors()
    {
    }

    public StructWithParameterlessAndParameterizedConstructors(int myValue0)
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
public class ClassWithVarargConstructor
{
    public ClassWithVarargConstructor(__arglist)
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
public struct StructWithVarargConstructor
{
    public StructWithVarargConstructor(__arglist)
    {
    }

    [TabularFieldOrder(0)]
    public int MyValue0;
}

[TabularRecord]
public class ClassWithParameterlessAndVarargConstructors
{
    public ClassWithParameterlessAndVarargConstructors()
    {
    }

    public ClassWithParameterlessAndVarargConstructors(__arglist)
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
public class RequiredMappedMember
{
    [TabularFieldOrder(0)]
    public required int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class RequiredUnmappedMember
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
public class RequiredUnmappedMemberWithSetsRequiredMembers
{
    [SetsRequiredMembers]
    public RequiredUnmappedMemberWithSetsRequiredMembers()
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
public class RequiredUnmappedMemberInBase : RequiredUnmappedMemberBase
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
public class RequiredMappedMemberInBase : RequiredMappedMemberBase
{
    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public record PositionalRecordWithRequiredMember(
    [property: TabularFieldOrder(0)] int MyValue0)
{
    public required int MyValue1
    {
        get;
        init;
    }
}

[TabularRecord]
public class ClassPrimaryConstructorWithRequiredMember(int myValue0)
{
    [TabularFieldOrder(0)]
    public required int MyValue0
    {
        get;
        set;
    } = myValue0;
}
