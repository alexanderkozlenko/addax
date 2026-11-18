// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Diagnostics;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularRecordMapping : IEquatable<TabularRecordMapping>
    {
        public readonly string TypeName;
        public readonly EquatableArray<TabularFieldMapping> FieldMappings;
        public readonly EquatableArray<int> ConstructorArguments;
        public readonly EquatableArray<string> ExperimentalDiagnosticIds;

        public TabularRecordMapping(string typeName, EquatableArray<TabularFieldMapping> fieldMappings, EquatableArray<int> constructorArguments, EquatableArray<string> experimentalDiagnosticIds)
        {
            Debug.Assert(typeName != null);

            TypeName = typeName;
            FieldMappings = fieldMappings;
            ConstructorArguments = constructorArguments;
            ExperimentalDiagnosticIds = experimentalDiagnosticIds;
        }

        public bool Equals(TabularRecordMapping other)
        {
            return
                (TypeName == other.TypeName) &&
                FieldMappings.Equals(other.FieldMappings) &&
                ConstructorArguments.Equals(other.ConstructorArguments) &&
                ExperimentalDiagnosticIds.Equals(other.ExperimentalDiagnosticIds);
        }

        public override bool Equals(object obj)
        {
            return (obj is TabularRecordMapping other) && Equals(other);
        }

        public override int GetHashCode()
        {
            var builder = HashCodeBuilder.Create();

            builder.Add(TypeName);
            builder.Add(FieldMappings);
            builder.Add(ConstructorArguments);
            builder.Add(ExperimentalDiagnosticIds);

            return builder.ToHashCode();
        }
    }
}
