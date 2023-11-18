﻿// (c) Oleksandr Kozlenko. Licensed under the MIT license.

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

            for (var i = 0; i < recordMappings.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var sourceTextBuilder = new SourceTextBuilder(' ', 4, Encoding.UTF8))
                {
                    BuildHandlerSource(sourceTextBuilder, $"Handler{i}", recordMappings[i]);

                    context.AddSource($"Handler{i}.g.cs", sourceTextBuilder.ToSourceText());
                }
            }

            using (var sourceTextBuilder = new SourceTextBuilder(' ', 4, Encoding.UTF8))
            {
                BuildHandlerRegistratorSource(sourceTextBuilder, "HandlerRegistrator", recordMappings.Length);

                context.AddSource("HandlerRegistrator.g.cs", sourceTextBuilder.ToSourceText());
            }
        }

        private static bool RecordTypeHasConverters(in TabularRecordMapping mapping)
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

        private static bool RecordTypeHasHeader(in TabularRecordMapping mapping)
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

        private static int GetFieldCountForReading(in TabularRecordMapping mapping)
        {
            var count = 0;

            foreach (var kvp in mapping.FieldMappings)
            {
                if ((kvp.Value.MemberAccess & TypeMemberAccess.Write) != 0)
                {
                    count += 1;
                }
            }

            return count;
        }

        private static int GetFieldCountForWriting(in TabularRecordMapping mapping)
        {
            for (var i = 0; i < mapping.FieldMappings.Count; i++)
            {
                if (!mapping.FieldMappings.ContainsKey(i))
                {
                    return 0;
                }
            }

            var count = 0;

            foreach (var kvp in mapping.FieldMappings)
            {
                if ((kvp.Value.MemberAccess & TypeMemberAccess.Read) != 0)
                {
                    count += 1;
                }
            }

            return count;
        }

        private static TKey[] SortDictionaryKeys<TKey, TValue>(ImmutableDictionary<TKey, TValue> dictionary)
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
            var assembly = typeof(TabularHandlerEmitter).Assembly;

            var assemblyName = assembly.GetName().Name;
            var assemblyVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            return (assemblyName, assemblyVersion);
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
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
    }
}
