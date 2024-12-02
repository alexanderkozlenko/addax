// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal sealed partial class TabularHandlerEmitter
    {
        private static readonly (string Name, string Version) s_assemblyInfo = GetAssemblyInfo();

        public void EmitRecordMappings(SourceProductionContext context, ImmutableArray<TabularRecordMapping> recordMappings)
        {
            if (recordMappings.IsDefaultOrEmpty)
            {
                return;
            }

            var cancellationToken = context.CancellationToken;

            using (var writer = new SourceTextWriter(Encoding.UTF8, ' ', 4))
            {
                for (var i = 0; i < recordMappings.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    writer.Reset();

                    BuildHandlerSource(writer, $"Handler{i}", recordMappings[i]);

                    context.AddSource($"Handler{i}.g.cs", writer.ToSourceText());
                }

                cancellationToken.ThrowIfCancellationRequested();
                writer.Reset();

                BuildRegistryInitializerSource(writer, "RegistryInitializer", recordMappings.Length);

                context.AddSource("RegistryInitializer.g.cs", writer.ToSourceText());
            }
        }

        private static bool MappingHasConverters(in TabularRecordMapping mapping)
        {
            foreach (var kvp in mapping.FieldMappings)
            {
                if (kvp.Value.ConverterTypeName != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MappingSupportsHeader(in TabularRecordMapping mapping)
        {
            foreach (var kvp in mapping.FieldMappings)
            {
                if (kvp.Value.FieldNameLiteral.HasValue)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MappingSupportsReading(in TabularRecordMapping mapping)
        {
            foreach (var kvp in mapping.FieldMappings)
            {
                if (kvp.Value.SupportsWriting)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool MappingSupportsWriting(in TabularRecordMapping mapping)
        {
            foreach (var kvp in mapping.FieldMappings)
            {
                if (kvp.Value.SupportsReading)
                {
                    return true;
                }
            }

            return false;
        }

        private static TKey[] GetDictionaryKeysOrdered<TKey, TValue>(ImmutableDictionary<TKey, TValue> dictionary)
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
                case "global::System.Numerics.BigInteger":
                    {
                        return "BigInteger";
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
