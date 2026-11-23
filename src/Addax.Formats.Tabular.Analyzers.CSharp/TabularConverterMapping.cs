// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Diagnostics;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularConverterMapping : IEquatable<TabularConverterMapping>
    {
        public readonly string ValueTypeKey;
        public readonly string ConverterTypeName;
        public readonly EquatableArray<string> DiagnosticIdentifiers;

        public TabularConverterMapping(
            string valueTypeKey,
            string converterTypeName,
            EquatableArray<string> diagnosticIdentifiers)
        {
            Debug.Assert(valueTypeKey != null);
            Debug.Assert(converterTypeName != null);

            ValueTypeKey = valueTypeKey;
            ConverterTypeName = converterTypeName;
            DiagnosticIdentifiers = diagnosticIdentifiers;
        }

        public bool Equals(TabularConverterMapping other)
        {
            return
                (ValueTypeKey == other.ValueTypeKey) &&
                (ConverterTypeName == other.ConverterTypeName) &&
                (DiagnosticIdentifiers == other.DiagnosticIdentifiers);
        }

        public override bool Equals(object obj)
        {
            return (obj is TabularConverterMapping other) && Equals(other);
        }

        public override int GetHashCode()
        {
            var builder = HashCodeBuilder.Create();

            builder.Add(ValueTypeKey);
            builder.Add(ConverterTypeName);
            builder.Add(DiagnosticIdentifiers);

            return builder.ToHashCode();
        }

        public static bool operator ==(TabularConverterMapping left, TabularConverterMapping right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TabularConverterMapping left, TabularConverterMapping right)
        {
            return !left.Equals(right);
        }
    }
}
