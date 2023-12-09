// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using System.Diagnostics;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularRecordMapping
    {
        public readonly ImmutableDictionary<int, TabularFieldMapping> FieldMappings;
        public readonly string TypeName;

        public TabularRecordMapping(string typeName, ImmutableDictionary<int, TabularFieldMapping> fieldMappings)
        {
            Debug.Assert(typeName != null);

            TypeName = typeName;
            FieldMappings = fieldMappings;
        }
    }
}
