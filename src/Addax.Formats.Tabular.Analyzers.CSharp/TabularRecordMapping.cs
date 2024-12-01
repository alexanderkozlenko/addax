// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularRecordMapping
    {
        public readonly string TypeName;
        public readonly ImmutableDictionary<int, TabularFieldMapping> FieldMappings;

        public TabularRecordMapping(string typeName, ImmutableDictionary<int, TabularFieldMapping> fieldMappings)
        {
            Debug.Assert(typeName != null);
            Debug.Assert(fieldMappings != null);

            TypeName = typeName;
            FieldMappings = fieldMappings;
        }
    }
}
