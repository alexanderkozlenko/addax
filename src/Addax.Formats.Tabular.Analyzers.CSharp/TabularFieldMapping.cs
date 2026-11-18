// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Diagnostics;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularFieldMapping : IEquatable<TabularFieldMapping>
    {
        public readonly int FieldOrder;
        public readonly string FieldNameLiteral;
        public readonly string MemberIdentifier;
        public readonly string ValueTypeName;
        public readonly string ConverterTypeName;
        public readonly bool SupportsWriting;
        public readonly bool SupportsReading;
        public readonly bool IsNullableT;

        public TabularFieldMapping(
            int fieldOrder,
            string memberIdentifier,
            bool supportsReading,
            bool supportsWriting,
            bool isNullableT,
            string valueTypeName,
            string converterTypeName,
            string fieldNameLiteral)
        {
            Debug.Assert(memberIdentifier != null);
            Debug.Assert(valueTypeName != null);

            FieldOrder = fieldOrder;
            MemberIdentifier = memberIdentifier;
            SupportsReading = supportsReading;
            SupportsWriting = supportsWriting;
            IsNullableT = isNullableT;
            ValueTypeName = valueTypeName;
            ConverterTypeName = converterTypeName;
            FieldNameLiteral = fieldNameLiteral;
        }

        public bool Equals(TabularFieldMapping other)
        {
            return
                (FieldOrder == other.FieldOrder) &&
                (FieldNameLiteral == other.FieldNameLiteral) &&
                (MemberIdentifier == other.MemberIdentifier) &&
                (ValueTypeName == other.ValueTypeName) &&
                (ConverterTypeName == other.ConverterTypeName) &&
                (SupportsWriting == other.SupportsWriting) &&
                (SupportsReading == other.SupportsReading) &&
                (IsNullableT == other.IsNullableT);
        }

        public override bool Equals(object obj)
        {
            return (obj is TabularFieldMapping other) && Equals(other);
        }

        public override int GetHashCode()
        {
            var builder = HashCodeBuilder.Create();

            builder.Add(FieldOrder);
            builder.Add(FieldNameLiteral);
            builder.Add(MemberIdentifier);
            builder.Add(ValueTypeName);
            builder.Add(ConverterTypeName);
            builder.Add(SupportsWriting);
            builder.Add(SupportsReading);
            builder.Add(IsNullableT);

            return builder.ToHashCode();
        }
    }
}
