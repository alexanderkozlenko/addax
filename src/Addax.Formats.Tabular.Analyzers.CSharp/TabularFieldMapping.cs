// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularFieldMapping
    {
        public readonly SyntaxToken? FieldNameLiteral;
        public readonly string MemberName;
        public readonly string ValueTypeName;
        public readonly string ConverterTypeName;
        public readonly bool SupportsWriting;
        public readonly bool SupportsReading;
        public readonly bool IsNullableT;

        public TabularFieldMapping(
            string memberName,
            bool supportsReading,
            bool supportsWriting,
            bool isNullableT,
            string valueTypeName,
            string converterTypeName,
            SyntaxToken? fieldNameLiteral)
        {
            Debug.Assert(memberName != null);
            Debug.Assert(valueTypeName != null);

            MemberName = memberName;
            SupportsReading = supportsReading;
            SupportsWriting = supportsWriting;
            IsNullableT = isNullableT;
            ValueTypeName = valueTypeName;
            ConverterTypeName = converterTypeName;
            FieldNameLiteral = fieldNameLiteral;
        }
    }
}
