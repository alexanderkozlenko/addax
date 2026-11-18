using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular;
using Addax.Formats.Tabular.Converters;

namespace MyNamespace;

[TabularRecord]
public struct BuiltInArrayConvertersRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<TabularBase16ArrayConverter>]
    public byte[]? MyValue0;

    [TabularFieldOrder(1)]
    [TabularConverter<TabularBase64ArrayConverter>]
    public byte[]? MyValue1;
}

[TabularRecord]
public struct NullableValueConvertersRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<TabularInt32Converter>]
    public int? MyValue0;

    [TabularFieldOrder(1)]
    [TabularConverter(typeof(TabularGuidConverter))]
    public Guid? MyValue1
    {
        get;
        set;
    }
}

public sealed class UpperCaseConverter : TabularConverter<string>
{
    public override bool TryFormat(string? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        var text = (value ?? string.Empty).ToUpperInvariant();

        if (!text.AsSpan().TryCopyTo(destination))
        {
            charsWritten = 0;

            return false;
        }

        charsWritten = text.Length;

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out string? value)
    {
        value = source.ToString().ToLowerInvariant();

        return true;
    }
}

[TabularRecord]
public class CustomConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<UpperCaseConverter>]
    public string? MyValue0
    {
        get;
        set;
    }

    [TabularFieldOrder(1)]
    [TabularConverter<TabularInt32Converter>]
    public int? MyValue1
    {
        get;
        set;
    }
}

[TabularRecord]
public struct ValueTypeMismatchRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<TabularBooleanConverter>]
    public string? MyValue0;
}

public sealed class IdentityConvertibleConverter : TabularConverter<(int, int)>
{
}

[TabularRecord]
public class IdentityConvertibleValueTypeRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<IdentityConvertibleConverter>]
    public (int X, int Y) MyValue0
    {
        get;
        set;
    }
}

public sealed class DefaultAndOptionalConstructorsConverter : TabularConverter<int>
{
    public DefaultAndOptionalConstructorsConverter()
    {
    }

    public DefaultAndOptionalConstructorsConverter(int myOption = 0)
    {
    }
}

[TabularRecord]
public class DefaultAndOptionalConstructorsRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<DefaultAndOptionalConstructorsConverter>]
    public int MyValue0
    {
        get;
        set;
    }
}

public sealed class OptionalConstructorConverter : TabularConverter<int>
{
    private OptionalConstructorConverter()
    {
    }

    public OptionalConstructorConverter(int myOption = 0)
    {
    }
}

[TabularRecord]
public class OptionalConstructorRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<OptionalConstructorConverter>]
    public int MyValue0
    {
        get;
        set;
    }
}

public sealed class AmbiguousConstructorsConverter : TabularConverter<int>
{
    public AmbiguousConstructorsConverter(int myOption1 = 0)
    {
    }

    public AmbiguousConstructorsConverter(string? myOption2 = null)
    {
    }
}

[TabularRecord]
public class AmbiguousConstructorsRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<AmbiguousConstructorsConverter>]
    public int MyValue0
    {
        get;
        set;
    }
}

public sealed class InaccessibleConstructorConverter : TabularConverter<int>
{
    private InaccessibleConstructorConverter()
    {
    }
}

[TabularRecord]
public class InaccessibleConstructorRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<InaccessibleConstructorConverter>]
    public int MyValue0
    {
        get;
        set;
    }
}

public sealed class RequiredMembersConverter : TabularConverter<int>
{
    public required int MyOption
    {
        get;
        init;
    }
}

[TabularRecord]
public class RequiredMembersRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<RequiredMembersConverter>]
    public int MyValue0
    {
        get;
        set;
    }
}

public sealed class SetsRequiredMembersConverter : TabularConverter<int>
{
    [SetsRequiredMembers]
    public SetsRequiredMembersConverter()
    {
        MyOption = 1;
    }

    public required int MyOption
    {
        get;
        init;
    }
}

[TabularRecord]
public class SetsRequiredMembersRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<SetsRequiredMembersConverter>]
    public int MyValue0
    {
        get;
        set;
    }
}

public abstract class AbstractConverter : TabularConverter<int>
{
}

[TabularRecord]
public class AbstractConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<AbstractConverter>]
    public int MyValue0
    {
        get;
        set;
    }
}

file sealed class FileLocalConverter : TabularConverter<int>
{
}

[TabularRecord]
public class FileLocalConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<FileLocalConverter>]
    public int MyValue0
    {
        get;
        set;
    }
}

public sealed class GenericConverter<T> : TabularConverter<int>
    where T : allows ref struct
{
}

file sealed class FileLocalTypeArgument
{
}

[TabularRecord]
public class DynamicTypeArgumentRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter(typeof(GenericConverter<List<dynamic>>))]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class RefStructTypeArgumentRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<GenericConverter<Span<int>>>]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class FileLocalTypeArgumentRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<GenericConverter<FileLocalTypeArgument[]>>]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class NotConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter(typeof(string))]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class ArrayConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter(typeof(TabularInt32Converter[]))]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class NullConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter(null!)]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class OpenGenericConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter(typeof(GenericConverter<>))]
    public int MyValue0
    {
        get;
        set;
    }
}

[TabularRecord]
public class PrivateConverterRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<PrivateConverter>]
    public int MyValue0
    {
        get;
        set;
    }

    private sealed class PrivateConverter : TabularConverter<int>
    {
    }
}

[TabularRecord]
public class PointerTypeArgumentRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter(typeof(GenericConverter<int*[]>))]
    public int MyValue0
    {
        get;
        set;
    }
}
