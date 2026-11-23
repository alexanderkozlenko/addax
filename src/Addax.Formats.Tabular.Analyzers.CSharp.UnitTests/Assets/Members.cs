using System;
using Addax.Formats.Tabular;
using Addax.Formats.Tabular.Converters;

namespace MyNamespace;

[TabularRecord]
public struct KeywordNamesRecord
{
    [TabularFieldOrder(0)]
    public int @class;

    [TabularFieldOrder(1)]
    public string? @await
    {
        get;
        set;
    }

    [TabularFieldOrder(2)]
    public long? @event;

    [TabularFieldOrder(3)]
    [TabularConverter<TabularInt32Converter>]
    public int? @operator
    {
        get;
        set;
    }

    [TabularFieldOrder(4)]
    public double @union;
}

[TabularRecord]
public class InaccessibleAccessorsRecord
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        private set;
    }

    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        protected init;
    }

    [TabularFieldOrder(2)]
    public int MyValue2
    {
        private get;
        set;
    }

    [TabularFieldOrder(3)]
    public int MyValue3
    {
        get;
        protected internal set;
    }
}

[TabularRecord]
public readonly struct ReadOnlyFieldRecord
{
    [TabularFieldOrder(0)]
    public readonly string MyValue0;
}

[TabularRecord]
public class SetOnlyPropertyRecord
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        set
        {
        }
    }
}

[TabularRecord]
public class NeitherDirectionRecord
{
    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
    }
}

[TabularRecord]
public class IndexerRecord
{
    [TabularFieldOrder(0)]
    public int this[int index]
    {
        get
        {
            return index;
        }
        set
        {
        }
    }

    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public class PrivateMemberRecord
{
    [TabularFieldOrder(0)]
    private int MyValue0
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
public class ProtectedMemberRecord
{
    [TabularFieldOrder(0)]
    protected int MyValue0
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
public class PrivateUnsupportedMemberRecord
{
    [TabularFieldOrder(0)]
    private object? MyValue0
    {
        get;
        set;
    }
}

public interface IValueSource
{
    int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class ExplicitInterfaceMemberRecord : IValueSource
{
    [TabularFieldOrder(0)]
    int IValueSource.MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class StaticMemberRecord
{
    [TabularFieldOrder(0)]
    public static int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class ConstantMemberRecord
{
    [TabularFieldOrder(0)]
    public const int MyValue0 = 0;
}

[TabularRecord]
public class RefStructMemberRecord
{
    [TabularFieldOrder(0)]
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

public class HiddenMemberBase
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class HiddenMemberRecord : HiddenMemberBase
{
    public new string MyValue0
    {
        get;
        set;
    } = string.Empty;
}

public class MethodHidingBase
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class InaccessibleMethodHidingRecord : MethodHidingBase
{
    private new int MyValue0()
    {
        return 0;
    }
}

public class ConstructedGenericBase<T>
{
    [TabularFieldOrder(0)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class ConstructedGenericBaseRecord : ConstructedGenericBase<long>
{
    [TabularFieldOrder(1)]
    public int MyValue1
    {
        get;
        set;
    }
}

public class VirtualBase
{
    [TabularFieldOrder(0)]
    public virtual int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class UnmappedOverrideRecord : VirtualBase
{
    public override int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class OverrideAtSameOrderRecord : VirtualBase
{
    [TabularFieldOrder(0)]
    public override int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class OverrideAtOtherOrderRecord : VirtualBase
{
    [TabularFieldOrder(1)]
    public override int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class GetterOnlyOverrideRecord : VirtualBase
{
    [TabularFieldOrder(0)]
    public override int MyValue0
    {
        get
        {
            return base.MyValue0;
        }
    }
}

public abstract class AbstractBase
{
    [TabularFieldOrder(0)]
    public abstract int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class OverrideOfAbstractRecord : AbstractBase
{
    [TabularFieldName("my-value")]
    [TabularFieldOrder(0)]
    public override int MyValue0
    {
        get;
        set;
    }
}

public class IntermediateBase : VirtualBase
{
    [TabularFieldOrder(1)]
    public override int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class OverrideChainRecord : IntermediateBase
{
    [TabularFieldOrder(0)]
    public sealed override int MyValue0
    {
        get;
        set;
    }
}

public class GenericVirtualBase<T>
{
    [TabularFieldOrder(0)]
    public virtual T? MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class OverrideOfGenericBaseRecord : GenericVirtualBase<int>
{
    [TabularFieldOrder(0)]
    public override int MyValue0
    {
        get;
        set;
    }
}
