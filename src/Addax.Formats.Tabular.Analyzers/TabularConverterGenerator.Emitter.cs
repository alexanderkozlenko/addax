// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers;

public partial class TabularConverterGenerator
{
    private sealed partial class Emitter
    {
        private static readonly AssemblyName _assemblyName = typeof(Emitter).Assembly.GetName();

        public void EmitImplementationSourceOutput(SourceProductionContext context, TabularConverterGeneratorSpec generatorSpec)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var builder = new StringBuilder(1024);

            for (var i = 0; i < generatorSpec.RecordSpecs.Length; i++)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                BuildRecordConverterSource(builder, generatorSpec.RecordSpecs[i], i, context.CancellationToken);

                context.AddSource($"Addax.Formats.Tabular.Converters.GeneratedRecordConverter{i}.g.cs", builder.ToString());
                builder.Clear();
            }

            BuildModuleInitializerSource(builder, generatorSpec.RecordSpecs.Length, context.CancellationToken);

            context.AddSource("Addax.Formats.Tabular.Converters.ModuleInitializer.g.cs", builder.ToString());
            builder.Clear();
        }

        private static bool TypeSupportsReadingFields(in TabularRecordSpec recordSpec)
        {
            foreach (var fieldSpec in recordSpec.FieldSpecs.Values)
            {
                if (fieldSpec.TypeMemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TypeSupportsWritingFields(in TabularRecordSpec recordSpec)
        {
            for (var i = 0; i < recordSpec.FieldSpecs.Count; i++)
            {
                if (!recordSpec.FieldSpecs.ContainsKey(i))
                {
                    return false;
                }
            }

            foreach (var fieldSpec in recordSpec.FieldSpecs.Values)
            {
                if (fieldSpec.TypeMemberAccessTypes.HasFlag(TypeMemberAccessTypes.Read))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetTypeMemberTypeCode(TabularFieldType fieldType)
        {
            return fieldType switch
            {
                TabularFieldType.Char => "Char",
                TabularFieldType.Rune => "Rune",
                TabularFieldType.Boolean => "Boolean",
                TabularFieldType.SByte => "SByte",
                TabularFieldType.Byte => "Byte",
                TabularFieldType.Int16 => "Int16",
                TabularFieldType.UInt16 => "UInt16",
                TabularFieldType.Int32 => "Int32",
                TabularFieldType.UInt32 => "UInt32",
                TabularFieldType.Int64 => "Int64",
                TabularFieldType.UInt64 => "UInt64",
                TabularFieldType.Int128 => "Int128",
                TabularFieldType.UInt128 => "UInt128",
                TabularFieldType.BigInteger => "BigInteger",
                TabularFieldType.Half => "Half",
                TabularFieldType.Single => "Single",
                TabularFieldType.Double => "Double",
                TabularFieldType.Decimal => "Decimal",
                TabularFieldType.Complex => "Complex",
                TabularFieldType.TimeSpan => "TimeSpan",
                TabularFieldType.TimeOnly => "TimeOnly",
                TabularFieldType.DateOnly => "DateOnly",
                TabularFieldType.DateTime => "DateTime",
                TabularFieldType.DateTimeOffset => "DateTimeOffset",
                TabularFieldType.Guid => "Guid",
                _ => "String",
            };
        }

        private static string GetTypeMemberTypeString(TabularFieldType fieldType)
        {
            return fieldType switch
            {
                TabularFieldType.Char => "global::System.Char",
                TabularFieldType.Rune => "global::System.Text.Rune",
                TabularFieldType.Boolean => "global::System.Boolean",
                TabularFieldType.SByte => "global::System.SByte",
                TabularFieldType.Byte => "global::System.Byte",
                TabularFieldType.Int16 => "global::System.Int16",
                TabularFieldType.UInt16 => "global::System.UInt16",
                TabularFieldType.Int32 => "global::System.Int32",
                TabularFieldType.UInt32 => "global::System.UInt32",
                TabularFieldType.Int64 => "global::System.Int64",
                TabularFieldType.UInt64 => "global::System.UInt64",
                TabularFieldType.Int128 => "global::System.Int128",
                TabularFieldType.UInt128 => "global::System.UInt128",
                TabularFieldType.BigInteger => "global::System.Numerics.BigInteger",
                TabularFieldType.Half => "global::System.Half",
                TabularFieldType.Single => "global::System.Single",
                TabularFieldType.Double => "global::System.Double",
                TabularFieldType.Decimal => "global::System.Decimal",
                TabularFieldType.Complex => "global::System.Numerics.Complex",
                TabularFieldType.TimeSpan => "global::System.TimeSpan",
                TabularFieldType.TimeOnly => "global::System.TimeOnly",
                TabularFieldType.DateOnly => "global::System.DateOnly",
                TabularFieldType.DateTime => "global::System.DateTime",
                TabularFieldType.DateTimeOffset => "global::System.DateTimeOffset",
                TabularFieldType.Guid => "global::System.Guid",
                _ => "global::System.String",
            };
        }
    }
}
