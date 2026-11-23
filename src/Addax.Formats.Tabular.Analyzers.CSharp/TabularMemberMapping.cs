// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularMemberMapping
    {
        public readonly ISymbol Member;
        public readonly ITypeSymbol ValueType;
        public readonly string FieldNameLiteral;
        public readonly string ValueTypeName;
        public readonly string ValueTypeKey;
        public readonly string ValueTypeCode;
        public readonly TabularConverterMapping? ConverterMapping;
        public readonly TabularFieldAccess FieldAccess;
        public readonly TabularTypeModifier ValueTypeModifier;

        public TabularMemberMapping(
            ISymbol member,
            TabularFieldAccess fieldAccess,
            ITypeSymbol valueType,
            TabularTypeModifier valueTypeModifier,
            string valueTypeName,
            string valueTypeKey,
            string valueTypeCode,
            TabularConverterMapping? converterMapping,
            string fieldNameLiteral)
        {
            Debug.Assert(member != null);
            Debug.Assert(valueType != null);
            Debug.Assert(valueTypeName != null);
            Debug.Assert(valueTypeKey != null);
            Debug.Assert((valueTypeCode == null) || !converterMapping.HasValue);

            Member = member;
            FieldAccess = fieldAccess;
            ValueType = valueType;
            ValueTypeModifier = valueTypeModifier;
            ValueTypeName = valueTypeName;
            ValueTypeKey = valueTypeKey;
            ValueTypeCode = valueTypeCode;
            ConverterMapping = converterMapping;
            FieldNameLiteral = fieldNameLiteral;
        }
    }
}
