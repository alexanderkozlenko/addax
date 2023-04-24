// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers;

internal sealed partial class TabularConverterEmitter
{
    private static readonly AssemblyName _assemblyName = typeof(TabularConverterEmitter).Assembly.GetName();

    public void EmitImplementationSourceOutput(SourceProductionContext context, TabularConverterSourceSpec sourceSpec)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        if (sourceSpec.RecordSpecs.IsDefaultOrEmpty)
        {
            return;
        }

        var builder = new StringBuilder(1024);

        for (var i = 0; i < sourceSpec.RecordSpecs.Length; i++)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            BuildRecordConverterSource(builder, sourceSpec.RecordSpecs[i], i, context.CancellationToken);

            context.AddSource($"Addax.Formats.Tabular.Converters.GeneratedRecordConverter{i}.g.cs", builder.ToString());
            builder.Clear();
        }

        BuildModuleInitializerSource(builder, sourceSpec.RecordSpecs.Length, context.CancellationToken);

        context.AddSource("Addax.Formats.Tabular.Converters.ModuleInitializer.g.cs", builder.ToString());
        builder.Clear();
    }

    private static string? GetTypeCode(string typeName)
    {
        return typeName switch
        {
            "global::System.Boolean" => "Boolean",
            "global::System.Byte" => "Byte",
            "global::System.Char" => "Char",
            "global::System.DateOnly" => "DateOnly",
            "global::System.DateTime" => "DateTime",
            "global::System.DateTimeOffset" => "DateTimeOffset",
            "global::System.Decimal" => "Decimal",
            "global::System.Double" => "Double",
            "global::System.Guid" => "Guid",
            "global::System.Half" => "Half",
            "global::System.Int16" => "Int16",
            "global::System.Int32" => "Int32",
            "global::System.Int64" => "Int64",
            "global::System.Int128" => "Int128",
            "global::System.SByte" => "SByte",
            "global::System.Single" => "Single",
            "global::System.String" => "String",
            "global::System.TimeOnly" => "TimeOnly",
            "global::System.TimeSpan" => "TimeSpan",
            "global::System.UInt16" => "UInt16",
            "global::System.UInt32" => "UInt32",
            "global::System.UInt64" => "UInt64",
            "global::System.UInt128" => "UInt128",
            "global::System.Text.Rune" => "Rune",
            "global::System.Numerics.Complex" => "Complex",
            "global::System.Numerics.BigInteger" => "BigInteger",
            _ => null,
        };
    }

    private static bool TypeHasFieldConverters(in TabularRecordSpec recordSpec)
    {
        foreach (var kvp in recordSpec.FieldSpecs)
        {
            if (kvp.Value.MemberHasConverter)
            {
                return true;
            }
        }

        return false;
    }

    private static int GetReadingFieldsCount(in TabularRecordSpec recordSpec)
    {
        var count = 0;

        foreach (var kvp in recordSpec.FieldSpecs)
        {
            if (kvp.Value.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write))
            {
                count += 1;
            }
        }

        return count;
    }

    private static int GetWritingFieldsCount(in TabularRecordSpec recordSpec)
    {
        for (var i = 0; i < recordSpec.FieldSpecs.Count; i++)
        {
            if (!recordSpec.FieldSpecs.ContainsKey(i))
            {
                return 0;
            }
        }

        var count = 0;

        foreach (var kvp in recordSpec.FieldSpecs)
        {
            if (kvp.Value.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Read))
            {
                count += 1;
            }
        }

        return count;
    }

    private static TKey[] GetSortedKeys<TKey, TValue>(ImmutableDictionary<TKey, TValue> dictionary)
        where TKey : notnull
    {
        var keys = new TKey[dictionary.Count];
        var index = 0;

        foreach (var kvp in dictionary)
        {
            keys[index++] = kvp.Key;
        }

        Array.Sort(keys);

        return keys;
    }
}
