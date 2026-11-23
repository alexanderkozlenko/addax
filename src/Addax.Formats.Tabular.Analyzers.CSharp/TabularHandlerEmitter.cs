// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Reflection;
using System.Text;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal static partial class TabularHandlerEmitter
    {
        private static readonly (string Name, string Version) s_assemblyInfo = GetAssemblyInfo();

        public static void EmitRecordMappings(SourceProductionContext context, EquatableArray<TabularRecordMapping> recordMappings)
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

        private static (string Name, string Version) GetAssemblyInfo()
        {
            var assembly = typeof(TabularHandlerGenerator).Assembly;
            var assemblyName = assembly.GetName();
            var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            return (assemblyName.Name, assemblyVersionAttribute.InformationalVersion);
        }
    }
}
