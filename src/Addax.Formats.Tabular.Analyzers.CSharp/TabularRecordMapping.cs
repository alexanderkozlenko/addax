// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularRecordMapping
    {
        public readonly ImmutableDictionary<int, TabularFieldMapping> FieldMappings;
        public readonly string TypeName;

        public TabularRecordMapping(string typeName, ImmutableDictionary<int, TabularFieldMapping> fieldMappings)
        {
            TypeName = typeName;
            FieldMappings = fieldMappings;
        }
    }
}
