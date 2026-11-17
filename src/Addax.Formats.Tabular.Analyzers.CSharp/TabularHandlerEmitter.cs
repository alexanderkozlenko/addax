// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Reflection;
using System.Text;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal sealed partial class TabularHandlerEmitter
    {
        private static readonly (string Name, string Version) s_assemblyInfo = GetAssemblyInfo();

        public void EmitRecordMappings(SourceProductionContext context, EquatableArray<TabularRecordMapping> recordMappings)
        {
            if (recordMappings.IsEmpty)
            {
                return;
            }

            var cancellationToken = context.CancellationToken;
            var writer = new SourceTextWriter(Encoding.UTF8, ' ', 4);

            for (var i = 0; i < recordMappings.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                writer.Reset();

                BuildHandlerSource(writer, $"Handler{i}", recordMappings[i]);

                context.AddSource($"Handler{i}.g.cs", writer.ToSourceText());
            }
        }

        private static bool MappingHasConverters(in TabularRecordMapping mapping)
        {
            foreach (var fieldMapping in mapping.FieldMappings)
            {
                if (fieldMapping.ConverterTypeName != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MappingSupportsHeader(in TabularRecordMapping mapping)
        {
            foreach (var fieldMapping in mapping.FieldMappings)
            {
                if (fieldMapping.FieldNameLiteral != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MappingSupportsReading(in TabularRecordMapping mapping)
        {
            foreach (var fieldMapping in mapping.FieldMappings)
            {
                if (fieldMapping.SupportsWriting)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MappingSupportsWriting(in TabularRecordMapping mapping)
        {
            var fieldCount = 0;
            var maxFieldOrder = -1;

            foreach (var fieldMapping in mapping.FieldMappings)
            {
                if (fieldMapping.SupportsReading)
                {
                    fieldCount++;

                    if (fieldMapping.FieldOrder > maxFieldOrder)
                    {
                        maxFieldOrder = fieldMapping.FieldOrder;
                    }
                }
            }

            return (fieldCount != 0) && (maxFieldOrder == fieldCount - 1);
        }

        private static int[] GetFieldOrders(in TabularRecordMapping mapping)
        {
            var fieldOrders = new int[mapping.FieldMappings.Count];

            for (var i = 0; i < fieldOrders.Length; i++)
            {
                fieldOrders[i] = mapping.FieldMappings[i].FieldOrder;
            }

            return fieldOrders;
        }

        private static (string Name, string Version) GetAssemblyInfo()
        {
            var assembly = typeof(TabularHandlerGenerator).Assembly;

            var assemblyName = assembly.GetName();
            var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            return (assemblyName.Name, assemblyVersionAttribute.InformationalVersion);
        }

        private static string GetValueTypeCode(string valueTypeName)
        {
            switch (valueTypeName)
            {
                case "global::System.Boolean":
                    {
                        return "Boolean";
                    }
                case "global::System.Byte":
                    {
                        return "Byte";
                    }
                case "global::System.Char":
                    {
                        return "Char";
                    }
                case "global::System.DateOnly":
                    {
                        return "DateOnly";
                    }
                case "global::System.DateTime":
                    {
                        return "DateTime";
                    }
                case "global::System.DateTimeOffset":
                    {
                        return "DateTimeOffset";
                    }
                case "global::System.Decimal":
                    {
                        return "Decimal";
                    }
                case "global::System.Double":
                    {
                        return "Double";
                    }
                case "global::System.Guid":
                    {
                        return "Guid";
                    }
                case "global::System.Half":
                    {
                        return "Half";
                    }
                case "global::System.Int16":
                    {
                        return "Int16";
                    }
                case "global::System.Int32":
                    {
                        return "Int32";
                    }
                case "global::System.Int64":
                    {
                        return "Int64";
                    }
                case "global::System.Int128":
                    {
                        return "Int128";
                    }
                case "global::System.SByte":
                    {
                        return "SByte";
                    }
                case "global::System.Single":
                    {
                        return "Single";
                    }
                case "global::System.String":
                    {
                        return "String";
                    }
                case "global::System.TimeOnly":
                    {
                        return "TimeOnly";
                    }
                case "global::System.TimeSpan":
                    {
                        return "TimeSpan";
                    }
                case "global::System.UInt16":
                    {
                        return "UInt16";
                    }
                case "global::System.UInt32":
                    {
                        return "UInt32";
                    }
                case "global::System.UInt64":
                    {
                        return "UInt64";
                    }
                case "global::System.UInt128":
                    {
                        return "UInt128";
                    }
                case "global::System.Uri":
                    {
                        return "Uri";
                    }
                case "global::System.Numerics.BFloat16":
                    {
                        return "BFloat16";
                    }
                case "global::System.Numerics.BigInteger":
                    {
                        return "BigInteger";
                    }
                case "global::System.Numerics.Decimal32":
                    {
                        return "Decimal32";
                    }
                case "global::System.Numerics.Decimal64":
                    {
                        return "Decimal64";
                    }
                case "global::System.Numerics.Decimal128":
                    {
                        return "Decimal128";
                    }
                case "global::System.Text.Rune":
                    {
                        return "Rune";
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
    }
}
