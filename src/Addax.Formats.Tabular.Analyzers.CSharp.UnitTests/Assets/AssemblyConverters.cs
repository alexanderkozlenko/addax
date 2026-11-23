using System;
using System.Collections.Generic;
using Addax.Formats.Tabular;
using Addax.Formats.Tabular.Converters;

[assembly: TabularConverter<TabularBase16ArrayConverter>]
[assembly: TabularConverter<TabularBase16MemoryConverter>]
[assembly: TabularConverter<MyNamespace.CompactDateOnlyConverter>]
[assembly: TabularConverter<MyNamespace.ObjectConverter>]
[assembly: TabularConverter<MyNamespace.ObjectListConverter>]

namespace MyNamespace;

public sealed class CompactDateOnlyConverter : TabularDateOnlyConverter
{
    public CompactDateOnlyConverter()
        : base("yyyyMMdd")
    {
    }
}

public sealed class ObjectConverter : TabularConverter<object>
{
    public override bool TryFormat(object? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        var text = value?.ToString() ?? string.Empty;

        if (!text.AsSpan().TryCopyTo(destination))
        {
            charsWritten = 0;

            return false;
        }

        charsWritten = text.Length;

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out object? value)
    {
        value = source.ToString();

        return true;
    }
}

public sealed class ObjectListConverter : TabularConverter<List<object>>
{
    public override bool TryFormat(List<object>? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        var text = string.Join('|', value ?? []);

        if (!text.AsSpan().TryCopyTo(destination))
        {
            charsWritten = 0;

            return false;
        }

        charsWritten = text.Length;

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out List<object>? value)
    {
        value = [.. source.ToString().Split('|')];

        return true;
    }
}

[TabularRecord]
public struct AssemblyConvertersRecord
{
    [TabularFieldOrder(0)]
    public byte[]? MyValue0;

    [TabularFieldOrder(1)]
    public Memory<byte>? MyValue1;
}

[TabularRecord]
public struct BuiltInTypeAssemblyConverterRecord
{
    [TabularFieldOrder(0)]
    public DateOnly MyValue0;

    [TabularFieldOrder(1)]
    public DateOnly? MyValue1;
}

[TabularRecord]
public struct DynamicAssemblyConvertersRecord
{
    [TabularFieldOrder(0)]
    public dynamic? MyValue0;

    [TabularFieldOrder(1)]
    public List<dynamic>? MyValue1;
}

[TabularRecord]
public struct MemberConvertersRecord
{
    [TabularFieldOrder(0)]
    [TabularConverter<TabularDateOnlyConverter>]
    public DateOnly MyValue0;

    [TabularFieldOrder(1)]
    [TabularConverter<TabularBase64ArrayConverter>]
    public byte[]? MyValue1;
}
