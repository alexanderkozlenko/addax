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
        public readonly EquatableArray<string> DiagnosticIdentifiers;
        public readonly TabularRecordAccess RecordAccess;

        public TabularRecordMapping(
            string typeName,
            EquatableArray<TabularFieldMapping> fieldMappings,
            EquatableArray<int> constructorArguments,
            EquatableArray<string> diagnosticIdentifiers,
            TabularRecordAccess recordAccess)
        {
            Debug.Assert(typeName != null);

            TypeName = typeName;
            FieldMappings = fieldMappings;
            ConstructorArguments = constructorArguments;
            DiagnosticIdentifiers = diagnosticIdentifiers;
            RecordAccess = recordAccess;
        }

        public bool Equals(TabularRecordMapping other)
        {
            return
                (TypeName == other.TypeName) &&
                (FieldMappings == other.FieldMappings) &&
                (ConstructorArguments == other.ConstructorArguments) &&
                (DiagnosticIdentifiers == other.DiagnosticIdentifiers) &&
                (RecordAccess == other.RecordAccess);
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
            builder.Add(DiagnosticIdentifiers);
            builder.Add(RecordAccess);

            return builder.ToHashCode();
        }

        public static bool operator ==(TabularRecordMapping left, TabularRecordMapping right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TabularRecordMapping left, TabularRecordMapping right)
        {
            return !left.Equals(right);
        }
    }
}
