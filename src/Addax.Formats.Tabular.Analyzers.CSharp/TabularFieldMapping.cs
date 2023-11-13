// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularFieldMapping
    {
        public readonly SyntaxToken? FieldNameLiteral;
        public readonly string MemberName;
        public readonly string ValueTypeName;
        public readonly string ConverterTypeName;
        public readonly TypeMemberAccess MemberAccess;
        public readonly bool AsNullableT;

        public TabularFieldMapping(
            string memberName,
            TypeMemberAccess memberAccess,
            bool asNullableT,
            string valueTypeName,
            string converterTypeName,
            SyntaxToken? fieldNameLiteral)
        {
            MemberName = memberName;
            MemberAccess = memberAccess;
            AsNullableT = asNullableT;
            ValueTypeName = valueTypeName;
            ConverterTypeName = converterTypeName;
            FieldNameLiteral = fieldNameLiteral;
        }
    }
}
