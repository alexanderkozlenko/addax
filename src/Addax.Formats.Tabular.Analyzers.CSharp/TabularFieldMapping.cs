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
        public readonly string MemberNameLiteral;
        public readonly string ValueTypeName;
        public readonly string ValueTypeKey;
        public readonly string ValueTypeCode;
        public readonly string ConverterTypeName;
        public readonly TabularFieldAccess FieldAccess;
        public readonly TabularTypeModifier ValueTypeModifier;

        public TabularFieldMapping(
            int fieldOrder,
            string memberIdentifier,
            string memberNameLiteral,
            TabularFieldAccess fieldAccess,
            TabularTypeModifier valueTypeModifier,
            string valueTypeName,
            string valueTypeKey,
            string valueTypeCode,
            string converterTypeName,
            string fieldNameLiteral)
        {
            Debug.Assert(memberIdentifier != null);
            Debug.Assert(memberNameLiteral != null);
            Debug.Assert(valueTypeName != null);
            Debug.Assert(valueTypeKey != null);
            Debug.Assert((valueTypeCode == null) || (converterTypeName == null));

            FieldOrder = fieldOrder;
            MemberIdentifier = memberIdentifier;
            MemberNameLiteral = memberNameLiteral;
            FieldAccess = fieldAccess;
            ValueTypeModifier = valueTypeModifier;
            ValueTypeName = valueTypeName;
            ValueTypeKey = valueTypeKey;
            ValueTypeCode = valueTypeCode;
            ConverterTypeName = converterTypeName;
            FieldNameLiteral = fieldNameLiteral;
        }

        public bool Equals(TabularFieldMapping other)
        {
            return
                (FieldOrder == other.FieldOrder) &&
                (FieldNameLiteral == other.FieldNameLiteral) &&
                (MemberIdentifier == other.MemberIdentifier) &&
                (MemberNameLiteral == other.MemberNameLiteral) &&
                (ValueTypeName == other.ValueTypeName) &&
                (ValueTypeKey == other.ValueTypeKey) &&
                (ValueTypeCode == other.ValueTypeCode) &&
                (ConverterTypeName == other.ConverterTypeName) &&
                (FieldAccess == other.FieldAccess) &&
                (ValueTypeModifier == other.ValueTypeModifier);
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
            builder.Add(MemberNameLiteral);
            builder.Add(ValueTypeName);
            builder.Add(ValueTypeKey);
            builder.Add(ValueTypeCode);
            builder.Add(ConverterTypeName);
            builder.Add(FieldAccess);
            builder.Add(ValueTypeModifier);

            return builder.ToHashCode();
        }

        public static bool operator ==(TabularFieldMapping left, TabularFieldMapping right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TabularFieldMapping left, TabularFieldMapping right)
        {
            return !left.Equals(right);
        }
    }
}
