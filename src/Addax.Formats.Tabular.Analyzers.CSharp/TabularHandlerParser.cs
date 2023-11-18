// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal sealed class TabularHandlerParser
    {
        private static readonly SymbolDisplayFormat s_displayFormat = SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions & ~SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        private static readonly DiagnosticDescriptor s_diagnostic0000 = new DiagnosticDescriptor(
            "TAB0000",
            "The current language version is not supported by the source generator",
            "The current language version is not supported by the source generator",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0001 = new DiagnosticDescriptor(
            "TAB0001",
            "The type cannot be used for tabular record mapping",
            "The type cannot be used for tabular record mapping",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0002 = new DiagnosticDescriptor(
            "TAB0002",
            "The type member cannot be used for tabular field mapping",
            "The type member cannot be used for tabular field mapping",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0003 = new DiagnosticDescriptor(
            "TAB0003",
            "The type member requires a converter for tabular field mapping",
            "The type member requires a converter for tabular field mapping",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0004 = new DiagnosticDescriptor(
            "TAB0004",
            "The specified converter cannot be used for tabular field mapping",
            "The specified converter cannot be used for tabular field mapping",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0005 = new DiagnosticDescriptor(
            "TAB0005",
            "The type member has an invalid tabular field mapping",
            "The type member has an invalid tabular field mapping",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        public ImmutableArray<TabularRecordMapping> GetRecordMappings(CSharpCompilation compilation, SourceProductionContext context, ImmutableArray<INamedTypeSymbol> recordTypes)
        {
            if (recordTypes.IsDefaultOrEmpty)
            {
                return ImmutableArray<TabularRecordMapping>.Empty;
            }

            var cancellationToken = context.CancellationToken;

            if (!TryGetReferencedAssembly(compilation, "Addax.Formats.Tabular", out var featureAssembly, cancellationToken))
            {
                return ImmutableArray<TabularRecordMapping>.Empty;
            }

            var converterBaseType = featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularConverter`1");
            var converterAttributeType = featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularConverterAttribute");
            var fieldOrderAttributeType = featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularFieldOrderAttribute");
            var fieldNameAttributeType = featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularFieldNameAttribute");

            var recordMappingsBuilder = ImmutableArray.CreateBuilder<TabularRecordMapping>();

            foreach (var recordType in recordTypes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (compilation.LanguageVersion < LanguageVersion.CSharp11)
                {
                    context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0000, recordType.Locations.FirstOrDefault()));

                    continue;
                }

                if (recordType.IsStatic || (recordType.IsValueType && recordType.IsRefLikeType))
                {
                    context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0001, recordType.Locations.FirstOrDefault()));

                    continue;
                }

                var recordTypeHasDefaultConstructor = TryGetDefaultConstructor(recordType, out var recordTypeConstructor);
                var recordTypeHasErrors = false;
                var recordMembers = GetMappedRecordMembers(recordType, fieldOrderAttributeType, cancellationToken);
                var fieldMappingsBuilder = ImmutableDictionary.CreateBuilder<int, TabularFieldMapping>();

                foreach (var recordMemberInfo in recordMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (recordMemberInfo.Member.IsStatic)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0002, recordMemberInfo.Member.Locations.FirstOrDefault()));
                        recordTypeHasErrors = true;

                        continue;
                    }

                    var valueTypeInfo = GetValueTypeInfo(recordMemberInfo.Member);

                    if (valueTypeInfo.Type.IsStatic || (valueTypeInfo.Type.IsValueType && valueTypeInfo.Type.IsRefLikeType))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0002, recordMemberInfo.Member.Locations.FirstOrDefault()));

                        continue;
                    }

                    var converterTypeName = default(string);

                    if (!TryGetAttribute(recordMemberInfo.Member, converterAttributeType, out var converterAttribute, cancellationToken))
                    {
                        if (!valueTypeInfo.IsSupported)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0003, recordMemberInfo.Member.Locations.FirstOrDefault()));
                            recordTypeHasErrors = true;

                            continue;
                        }
                    }
                    else
                    {
                        if (!TryGetConverterType(converterAttribute, out var converterType))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0004, recordMemberInfo.Member.Locations.FirstOrDefault()));
                            recordTypeHasErrors = true;

                            continue;
                        }

                        if (!TryGetConverterValueType(converterType, converterBaseType, out var converterValueType))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0004, recordMemberInfo.Member.Locations.FirstOrDefault()));
                            recordTypeHasErrors = true;

                            continue;
                        }

                        if (!SymbolEqualityComparer.Default.Equals(valueTypeInfo.Type, converterValueType))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0004, recordMemberInfo.Member.Locations.FirstOrDefault()));
                            recordTypeHasErrors = true;

                            continue;
                        }

                        if (!TryGetDefaultConstructor(converterType, out var converterConstructor))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0004, recordMemberInfo.Member.Locations.FirstOrDefault()));
                            recordTypeHasErrors = true;

                            continue;
                        }

                        if (!compilation.IsSymbolAccessibleWithin(converterConstructor, compilation.Assembly))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0004, recordMemberInfo.Member.Locations.FirstOrDefault()));
                            recordTypeHasErrors = true;

                            continue;
                        }

                        converterTypeName = converterType.ToDisplayString(s_displayFormat);
                    }

                    if (!TryGetFieldOrder(recordMemberInfo.FieldOrderAttribute, out var fieldOrder))
                    {
                        recordTypeHasErrors = true;

                        continue;
                    }

                    if (fieldOrder < 0)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0005, recordMemberInfo.Member.Locations.FirstOrDefault()));
                        recordTypeHasErrors = true;

                        continue;
                    }

                    if (fieldMappingsBuilder.ContainsKey(fieldOrder))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(s_diagnostic0005, recordMemberInfo.Member.Locations.FirstOrDefault()));
                        recordTypeHasErrors = true;

                        continue;
                    }

                    var mappedMemberAccess = GetTypeMemberAccess(recordMemberInfo.Member);

                    if (!recordTypeHasDefaultConstructor || !compilation.IsSymbolAccessibleWithin(recordTypeConstructor, compilation.Assembly))
                    {
                        mappedMemberAccess &= ~TypeMemberAccess.Write;

                        if (mappedMemberAccess == TypeMemberAccess.None)
                        {
                            continue;
                        }
                    }

                    var fieldNameLiteral = default(SyntaxToken?);

                    if (TryGetAttribute(recordMemberInfo.Member, fieldNameAttributeType, out var fieldNameAttribute, cancellationToken))
                    {
                        if (TryGetFieldName(fieldNameAttribute, out var fieldName))
                        {
                            fieldNameLiteral = SyntaxFactory.Literal(fieldName);
                        }
                    }

                    var fieldMapping = new TabularFieldMapping(
                        recordMemberInfo.Member.Name,
                        mappedMemberAccess,
                        valueTypeInfo.AsNullableT,
                        valueTypeInfo.Name,
                        converterTypeName,
                        fieldNameLiteral);

                    fieldMappingsBuilder[fieldOrder] = fieldMapping;
                }

                if (!recordTypeHasErrors)
                {
                    var recordTypeName = recordType.ToDisplayString(s_displayFormat);
                    var fieldMappings = fieldMappingsBuilder.ToImmutable();

                    recordMappingsBuilder.Add(new TabularRecordMapping(recordTypeName, fieldMappings));
                }
            }

            return recordMappingsBuilder.ToImmutable();
        }

        private static ImmutableArray<(ISymbol Member, AttributeData FieldOrderAttribute)> GetMappedRecordMembers(ITypeSymbol recordType, INamedTypeSymbol fieldOrderAttributeType, CancellationToken cancellationToken)
        {
            var membersBuilder = ImmutableArray.CreateBuilder<(ISymbol, AttributeData)>();

            while (recordType != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var members = recordType.GetMembers();

                foreach (var member in members)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!(member is IFieldSymbol) && !(member is IPropertySymbol))
                    {
                        continue;
                    }

                    var attributes = member.GetAttributes();

                    foreach (var attribute in attributes)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, fieldOrderAttributeType))
                        {
                            membersBuilder.Add((member, attribute));

                            break;
                        }
                    }
                }

                recordType = recordType.BaseType;
            }

            return membersBuilder.ToImmutable();
        }

        private static (ITypeSymbol Type, string Name, bool IsSupported, bool AsNullableT) GetValueTypeInfo(ISymbol member)
        {
            var valueType = default(ITypeSymbol);

            if (member is IFieldSymbol typeField)
            {
                valueType = typeField.Type;
            }
            else if (member is IPropertySymbol typeProperty)
            {
                valueType = typeProperty.Type;
            }
            else
            {
                throw new NotSupportedException();
            }

            var memberTypeIsNullableT = false;

            if (valueType is INamedTypeSymbol namedType)
            {
                if (namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                {
                    memberTypeIsNullableT = true;
                    valueType = namedType.TypeArguments[0];
                }
            }

            var valueTypeName = default(string);
            var valueTypeIsSupported = true;

            switch (valueType.SpecialType)
            {
                case SpecialType.System_Boolean:
                    {
                        valueTypeName = "global::System.Boolean";
                    }
                    break;
                case SpecialType.System_Byte:
                    {
                        valueTypeName = "global::System.Byte";
                    }
                    break;
                case SpecialType.System_Char:
                    {
                        valueTypeName = "global::System.Char";
                    }
                    break;
                case SpecialType.System_DateTime:
                    {
                        valueTypeName = "global::System.DateTime";
                    }
                    break;
                case SpecialType.System_Decimal:
                    {
                        valueTypeName = "global::System.Decimal";
                    }
                    break;
                case SpecialType.System_Double:
                    {
                        valueTypeName = "global::System.Double";
                    }
                    break;
                case SpecialType.System_Int16:
                    {
                        valueTypeName = "global::System.Int16";
                    }
                    break;
                case SpecialType.System_Int32:
                    {
                        valueTypeName = "global::System.Int32";
                    }
                    break;
                case SpecialType.System_Int64:
                    {
                        valueTypeName = "global::System.Int64";
                    }
                    break;
                case SpecialType.System_SByte:
                    {
                        valueTypeName = "global::System.SByte";
                    }
                    break;
                case SpecialType.System_Single:
                    {
                        valueTypeName = "global::System.Single";
                    }
                    break;
                case SpecialType.System_String:
                    {
                        valueTypeName = "global::System.String";
                    }
                    break;
                case SpecialType.System_UInt16:
                    {
                        valueTypeName = "global::System.UInt16";
                    }
                    break;
                case SpecialType.System_UInt32:
                    {
                        valueTypeName = "global::System.UInt32";
                    }
                    break;
                case SpecialType.System_UInt64:
                    {
                        valueTypeName = "global::System.UInt64";
                    }
                    break;
                default:
                    {
                        valueTypeName = valueType.ToDisplayString(s_displayFormat);

                        valueTypeIsSupported =
                            (valueTypeName == "global::System.DateOnly") ||
                            (valueTypeName == "global::System.DateTimeOffset") ||
                            (valueTypeName == "global::System.Guid") ||
                            (valueTypeName == "global::System.Half") ||
                            (valueTypeName == "global::System.Int128") ||
                            (valueTypeName == "global::System.TimeOnly") ||
                            (valueTypeName == "global::System.TimeSpan") ||
                            (valueTypeName == "global::System.UInt128") ||
                            (valueTypeName == "global::System.Uri") ||
                            (valueTypeName == "global::System.Numerics.BigInteger");
                    }
                    break;
            }

            return (valueType, valueTypeName, valueTypeIsSupported, memberTypeIsNullableT);
        }

        private static TypeMemberAccess GetTypeMemberAccess(ISymbol member)
        {
            if (member is IFieldSymbol field)
            {
                var access = TypeMemberAccess.Read;

                if (!field.IsReadOnly && !field.IsConst)
                {
                    access |= TypeMemberAccess.Write;
                }

                return access;
            }
            else if (member is IPropertySymbol property)
            {
                var access = TypeMemberAccess.None;

                if (property.GetMethod != null)
                {
                    access |= TypeMemberAccess.Read;
                }
                if (property.SetMethod != null)
                {
                    access |= TypeMemberAccess.Write;
                }

                return access;
            }
            else
            {
                return TypeMemberAccess.None;
            }
        }

        private static bool TryGetFieldOrder(AttributeData attribute, out int fieldOrder)
        {
            if ((attribute != null) &&
                (attribute.ConstructorArguments.Length == 1) &&
                (attribute.ConstructorArguments[0].Type != null) &&
                (attribute.ConstructorArguments[0].Type.SpecialType == SpecialType.System_Int32))
            {
                fieldOrder = (int)attribute.ConstructorArguments[0].Value;

                return true;
            }

            fieldOrder = default;

            return false;
        }

        private static bool TryGetFieldName(AttributeData attribute, out string fieldName)
        {
            if ((attribute != null) &&
                (attribute.ConstructorArguments.Length == 1) &&
                (attribute.ConstructorArguments[0].Type != null) &&
                (attribute.ConstructorArguments[0].Type.SpecialType == SpecialType.System_String))
            {
                fieldName = (string)attribute.ConstructorArguments[0].Value;

                return true;
            }

            fieldName = default;

            return false;
        }

        private static bool TryGetConverterType(AttributeData attribute, out INamedTypeSymbol converterType)
        {
            if ((attribute != null) &&
                (attribute.ConstructorArguments.Length == 1) &&
                (attribute.ConstructorArguments[0].Kind == TypedConstantKind.Type) &&
                (attribute.ConstructorArguments[0].Value is INamedTypeSymbol namedType))
            {
                converterType = namedType;

                return true;
            }

            converterType = default;

            return false;
        }

        public static bool TryGetConverterValueType(INamedTypeSymbol converterType, INamedTypeSymbol converterBaseType, out ITypeSymbol converterValueType)
        {
            while (converterType != null)
            {
                if (SymbolEqualityComparer.Default.Equals(converterType.ConstructedFrom, converterBaseType))
                {
                    converterValueType = converterType.TypeArguments[0];

                    return true;
                }

                converterType = converterType.BaseType;
            }

            converterValueType = null;

            return false;
        }

        private static bool TryGetDefaultConstructor(INamedTypeSymbol type, out IMethodSymbol constructor)
        {
            foreach (var instanceConstructor in type.InstanceConstructors)
            {
                if (instanceConstructor.Parameters.IsDefaultOrEmpty)
                {
                    constructor = instanceConstructor;

                    return true;
                }
            }

            constructor = null;

            return false;
        }

        private static bool TryGetAttribute(ISymbol member, INamedTypeSymbol attributeType, out AttributeData attribute, CancellationToken cancellationToken)
        {
            var definedAttributes = member.GetAttributes();

            foreach (var definedAttribute in definedAttributes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (SymbolEqualityComparer.Default.Equals(definedAttribute.AttributeClass, attributeType))
                {
                    attribute = definedAttribute;

                    return true;
                }
            }

            attribute = null;

            return false;
        }

        private static bool TryGetReferencedAssembly(Compilation compilation, string assemblyName, out IAssemblySymbol assembly, CancellationToken cancellationToken)
        {
            foreach (var reference in compilation.References)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol referencedAssembly)
                {
                    if (referencedAssembly.Name == assemblyName)
                    {
                        assembly = referencedAssembly;

                        return true;
                    }
                }
            }

            assembly = null;

            return false;
        }
    }
}
