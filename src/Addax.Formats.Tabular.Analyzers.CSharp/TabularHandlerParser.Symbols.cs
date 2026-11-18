// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal partial class TabularHandlerParser
    {
        private static readonly SymbolDisplayFormat s_displayFormat = SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions & ~SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        private static void AddExperimentalDiagnosticIds(ISymbol symbol, HashSet<string> experimentalDiagnosticIds)
        {
            if (symbol == null)
            {
                return;
            }

            if (symbol is IArrayTypeSymbol arrayType)
            {
                AddExperimentalDiagnosticIds(arrayType.ElementType, experimentalDiagnosticIds);
            }
            else if (symbol is INamedTypeSymbol namedType)
            {
                for (var currentType = namedType; currentType != null; currentType = currentType.ContainingType)
                {
                    AddExperimentalDiagnosticId(currentType, experimentalDiagnosticIds);

                    foreach (var typeArgument in currentType.TypeArguments)
                    {
                        AddExperimentalDiagnosticIds(typeArgument, experimentalDiagnosticIds);
                    }
                }
            }
            else
            {
                AddExperimentalDiagnosticId(symbol, experimentalDiagnosticIds);
            }
        }

        private static void AddExperimentalDiagnosticId(ISymbol symbol, HashSet<string> experimentalDiagnosticIds)
        {
            if (TryGetExperimentalDiagnosticId(symbol, out var diagnosticId) ||
                TryGetExperimentalDiagnosticId(symbol.ContainingModule, out diagnosticId) ||
                TryGetExperimentalDiagnosticId(symbol.ContainingAssembly, out diagnosticId))
            {
                experimentalDiagnosticIds.Add(diagnosticId);
            }
        }

        private static bool TryGetExperimentalDiagnosticId(ISymbol symbol, out string diagnosticId)
        {
            if (symbol != null)
            {
                foreach (var attribute in symbol.GetAttributes())
                {
                    if (IsExperimentalAttribute(attribute.AttributeClass) &&
                        TryGetConstructorArgument<string>(attribute, out var attributeDiagnosticId) &&
                        SyntaxFacts.IsValidIdentifier(attributeDiagnosticId))
                    {
                        diagnosticId = attributeDiagnosticId;

                        return true;
                    }
                }
            }

            diagnosticId = null;

            return false;
        }

        private static bool IsExperimentalAttribute(INamedTypeSymbol attributeClass)
        {
            if ((attributeClass == null) || (attributeClass.ContainingType != null) || (attributeClass.Name != "ExperimentalAttribute"))
            {
                return false;
            }

            var containingNamespace = attributeClass.ContainingNamespace;

            return
                (containingNamespace?.Name == "CodeAnalysis") &&
                (containingNamespace.ContainingNamespace?.Name == "Diagnostics") &&
                (containingNamespace.ContainingNamespace.ContainingNamespace?.Name == "System") &&
                (containingNamespace.ContainingNamespace.ContainingNamespace.ContainingNamespace?.IsGlobalNamespace == true);
        }

        private static ImmutableArray<ISymbol> GetSuitableMembers(ITypeSymbol type)
        {
            var membersBuilder = ImmutableArray.CreateBuilder<ISymbol>();

            while (type != null)
            {
                var members = type.GetMembers();

                foreach (var member in members)
                {
                    if ((member is IFieldSymbol) || (member is IPropertySymbol))
                    {
                        membersBuilder.Add(member);
                    }
                }

                type = type.BaseType;
            }

            return membersBuilder.ToImmutable();
        }

        private static (ITypeSymbol Type, string Name, bool IsSupported, bool IsNullableT) GetValueTypeInfo(ISymbol member)
        {
            var valueType = (member is IFieldSymbol field) ? field.Type : ((IPropertySymbol)member).Type;
            var memberTypeIsNullableT = false;

            if ((valueType is INamedTypeSymbol namedType) && (namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T))
            {
                memberTypeIsNullableT = true;
                valueType = namedType.TypeArguments[0];
            }

            var valueTypeName = valueType.ToDisplayString(s_displayFormat);
            var valueTypeIsSupported = SpecialTypeIsSupported(valueType.SpecialType) || s_supportedTypeNames.Contains(valueTypeName);

            return (valueType, valueTypeName, valueTypeIsSupported, memberTypeIsNullableT);
        }

        private static bool SpecialTypeIsSupported(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_Char:
                case SpecialType.System_DateTime:
                case SpecialType.System_Decimal:
                case SpecialType.System_Double:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_SByte:
                case SpecialType.System_Single:
                case SpecialType.System_String:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        private bool TypeMemberSupportsReading(ISymbol member)
        {
            if (member is IFieldSymbol)
            {
                return true;
            }
            else if (member is IPropertySymbol property)
            {
                var getMethod = GetPropertyGetMethod(property);

                return (getMethod != null) && _compilation.IsSymbolAccessibleWithin(getMethod, _compilation.Assembly);
            }
            else
            {
                return false;
            }
        }

        private bool TypeMemberSupportsWriting(ISymbol member)
        {
            if (member is IFieldSymbol field)
            {
                return !field.IsReadOnly && !field.IsConst;
            }
            else if (member is IPropertySymbol property)
            {
                var setMethod = GetPropertySetMethod(property);

                return (setMethod != null) && _compilation.IsSymbolAccessibleWithin(setMethod, _compilation.Assembly);
            }
            else
            {
                return false;
            }
        }

        private static IMethodSymbol GetPropertyGetMethod(IPropertySymbol property)
        {
            while (property != null)
            {
                if (property.GetMethod != null)
                {
                    return property.GetMethod;
                }

                property = property.OverriddenProperty;
            }

            return null;
        }

        private static IMethodSymbol GetPropertySetMethod(IPropertySymbol property)
        {
            while (property != null)
            {
                if (property.SetMethod != null)
                {
                    return property.SetMethod;
                }

                property = property.OverriddenProperty;
            }

            return null;
        }

        private static bool TryGetConstructorArgument<T>(AttributeData attribute, out T value)
        {
            if ((attribute.ConstructorArguments.Length == 1) &&
                (attribute.ConstructorArguments[0].Kind == TypedConstantKind.Primitive) &&
                (attribute.ConstructorArguments[0].Value is T argumentValue))
            {
                value = argumentValue;

                return true;
            }

            value = default;

            return false;
        }

        private bool TryGetSupportedConverter(AttributeData attribute, INamedTypeSymbol converterBaseType, ITypeSymbol valueType, out INamedTypeSymbol converterType, out IMethodSymbol converterConstructor, out string issue)
        {
            converterType = null;
            converterConstructor = null;

            if (!TryGetConverterType(attribute, out var converterTypeSymbol))
            {
                issue = "the converter type is not specified";

                return false;
            }

            if (!TypeIsUsableInGeneratedCode(converterTypeSymbol, out issue))
            {
                return false;
            }

            var converterTypeName = converterTypeSymbol.ToDisplayString();

            if (!(converterTypeSymbol is INamedTypeSymbol converterNamedType) || !TryGetConverterValueType(converterNamedType, converterBaseType, out var converterValueType))
            {
                issue = $"'{converterTypeName}' does not derive from TabularConverter<T>";

                return false;
            }

            if (converterNamedType.IsAbstract)
            {
                issue = $"'{converterTypeName}' is abstract";

                return false;
            }

            if (!TypeRequiredMembersAreSet(converterNamedType, new HashSet<string>(StringComparer.Ordinal)))
            {
                issue = $"'{converterTypeName}' has required members";

                return false;
            }

            if (!_compilation.ClassifyConversion(converterValueType, valueType).IsIdentity)
            {
                issue = $"'{converterTypeName}' converts '{converterValueType.ToDisplayString()}', but the member type is '{valueType.ToDisplayString()}'";

                return false;
            }

            if (!TryGetConverterConstructor(converterNamedType, out converterConstructor))
            {
                issue = $"'{converterTypeName}' has neither an accessible parameterless constructor nor exactly one accessible constructor with only optional parameters";

                return false;
            }

            converterType = converterNamedType;
            issue = null;

            return true;
        }

        private static bool TryGetConverterType(AttributeData attribute, out ITypeSymbol converterType)
        {
            if ((attribute.AttributeClass != null) &&
                (attribute.AttributeClass.IsGenericType) &&
                (attribute.AttributeClass.TypeArguments.Length == 1))
            {
                converterType = attribute.AttributeClass.TypeArguments[0];

                return true;
            }

            if ((attribute.ConstructorArguments.Length == 1) &&
                (attribute.ConstructorArguments[0].Kind == TypedConstantKind.Type) &&
                (attribute.ConstructorArguments[0].Value is ITypeSymbol converterType2))
            {
                converterType = converterType2;

                return true;
            }

            converterType = default;

            return false;
        }

        private static bool TryGetConverterValueType(INamedTypeSymbol converterType, INamedTypeSymbol converterBaseType, out ITypeSymbol converterValueType)
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

        private bool TryGetConverterConstructor(INamedTypeSymbol type, out IMethodSymbol constructor)
        {
            var optionalConstructor = default(IMethodSymbol);
            var optionalConstructorCount = 0;

            foreach (var instanceConstructor in type.InstanceConstructors)
            {
                if (instanceConstructor.IsVararg || !_compilation.IsSymbolAccessibleWithin(instanceConstructor, _compilation.Assembly))
                {
                    continue;
                }

                if (instanceConstructor.Parameters.IsDefaultOrEmpty)
                {
                    constructor = instanceConstructor;

                    return true;
                }

                var parametersAreOptional = true;

                foreach (var parameter in instanceConstructor.Parameters)
                {
                    if (!parameter.IsOptional)
                    {
                        parametersAreOptional = false;

                        break;
                    }
                }

                if (parametersAreOptional)
                {
                    optionalConstructor = instanceConstructor;
                    optionalConstructorCount++;
                }
            }

            if (optionalConstructorCount == 1)
            {
                constructor = optionalConstructor;

                return true;
            }

            constructor = null;

            return false;
        }

        private string GetRecordTypeIssue(INamedTypeSymbol recordType)
        {
            if (recordType.IsStatic)
            {
                return "the type is a static class";
            }

            if (recordType.IsRefLikeType)
            {
                return "the type is a ref struct";
            }

            for (var currentType = recordType; currentType != null; currentType = currentType.ContainingType)
            {
                if (currentType.IsFileLocal)
                {
                    return SymbolEqualityComparer.Default.Equals(currentType, recordType) ? "the type is file-local" : $"the containing type '{currentType.ToDisplayString()}' is file-local";
                }
            }

            for (var currentType = recordType; currentType != null; currentType = currentType.ContainingType)
            {
                if (!currentType.TypeParameters.IsDefaultOrEmpty)
                {
                    return SymbolEqualityComparer.Default.Equals(currentType, recordType) ? "the type is generic" : $"the containing type '{currentType.ToDisplayString()}' is generic";
                }
            }

            var inaccessibleType = GetOutermostInaccessibleType(recordType);

            return SymbolEqualityComparer.Default.Equals(inaccessibleType, recordType) ? "the type is not accessible within the assembly" : $"the containing type '{inaccessibleType.ToDisplayString()}' is not accessible within the assembly";
        }

        private bool TypeMemberIsUsable(INamedTypeSymbol type, ISymbol member, out string issue)
        {
            if ((member is IFieldSymbol field) && field.IsConst)
            {
                issue = "the member is a constant";

                return false;
            }

            if (member.IsStatic)
            {
                issue = "the member is static";

                return false;
            }

            if (member is IPropertySymbol property)
            {
                if (property.IsIndexer)
                {
                    issue = "the member is an indexer";

                    return false;
                }

                if (!property.ExplicitInterfaceImplementations.IsDefaultOrEmpty)
                {
                    issue = "the member is an explicit interface implementation";

                    return false;
                }
            }

            if (!_compilation.IsSymbolAccessibleWithin(member, _compilation.Assembly))
            {
                issue = "the member is not accessible within the assembly";

                return false;
            }

            if (TryGetHidingMember(type, member, out var hidingMember))
            {
                issue = $"the member is hidden by '{hidingMember.ToDisplayString()}'";

                return false;
            }

            issue = null;

            return true;
        }

        private bool TryGetHidingMember(INamedTypeSymbol type, ISymbol member, out ISymbol hidingMember)
        {
            for (var currentType = type; currentType != null; currentType = currentType.BaseType)
            {
                foreach (var candidate in currentType.GetMembers(member.Name))
                {
                    if (SymbolEqualityComparer.Default.Equals(candidate, member))
                    {
                        hidingMember = null;

                        return false;
                    }

                    if (!_compilation.IsSymbolAccessibleWithin(candidate, _compilation.Assembly))
                    {
                        continue;
                    }

                    for (var overriddenProperty = (candidate as IPropertySymbol)?.OverriddenProperty; overriddenProperty != null; overriddenProperty = overriddenProperty.OverriddenProperty)
                    {
                        if (SymbolEqualityComparer.Default.Equals(overriddenProperty, member))
                        {
                            hidingMember = null;

                            return false;
                        }
                    }

                    hidingMember = candidate;

                    return true;
                }
            }

            hidingMember = null;

            return false;
        }

        private bool TypeIsUsableInGeneratedCode(ITypeSymbol type, out string issue)
        {
            if (type is IArrayTypeSymbol arrayType)
            {
                return TypeIsUsableInGeneratedCode(arrayType.ElementType, out issue);
            }

            if (type is IDynamicTypeSymbol)
            {
                issue = null;

                return true;
            }

            if (!(type is INamedTypeSymbol namedType))
            {
                switch (type.TypeKind)
                {
                    case TypeKind.Pointer:
                    case TypeKind.FunctionPointer:
                        {
                            issue = $"'{type.ToDisplayString()}' is a pointer type";

                            break;
                        }
                    case TypeKind.TypeParameter:
                        {
                            issue = $"'{type.ToDisplayString()}' is a type parameter";

                            break;
                        }
                    default:
                        {
                            issue = $"'{type.ToDisplayString()}' cannot be referenced by the generated code";

                            break;
                        }
                }

                return false;
            }

            if (namedType.TypeKind == TypeKind.Error)
            {
                issue = $"'{namedType.ToDisplayString()}' cannot be resolved";

                return false;
            }

            if (namedType.IsUnboundGenericType)
            {
                issue = $"'{namedType.ToDisplayString()}' is an open generic type";

                return false;
            }

            if (!_compilation.IsSymbolAccessibleWithin(namedType, _compilation.Assembly))
            {
                issue = $"'{GetOutermostInaccessibleType(namedType).ToDisplayString()}' is not accessible within the assembly";

                return false;
            }

            for (var currentType = namedType; currentType != null; currentType = currentType.ContainingType)
            {
                if (currentType.IsFileLocal)
                {
                    issue = $"'{currentType.ToDisplayString()}' is file-local";

                    return false;
                }

                foreach (var typeArgument in currentType.TypeArguments)
                {
                    if (!TypeIsUsableInGeneratedCode(typeArgument, out issue))
                    {
                        return false;
                    }
                }
            }

            issue = null;

            return true;
        }

        private INamedTypeSymbol GetOutermostInaccessibleType(INamedTypeSymbol type)
        {
            var inaccessibleType = type;

            for (var currentType = type.ContainingType; currentType != null; currentType = currentType.ContainingType)
            {
                if (!_compilation.IsSymbolAccessibleWithin(currentType, _compilation.Assembly))
                {
                    inaccessibleType = currentType;
                }
            }

            return inaccessibleType;
        }

        private static bool TryGetAttribute(ISymbol member, INamedTypeSymbol attributeType, out AttributeData attribute)
        {
            var memberAttributes = member.GetAttributes();

            foreach (var memberAttribute in memberAttributes)
            {
                var memberAttributeType = attributeType.IsGenericType ?
                    memberAttribute.AttributeClass?.ConstructedFrom :
                    memberAttribute.AttributeClass;

                if (SymbolEqualityComparer.Default.Equals(memberAttributeType, attributeType))
                {
                    attribute = memberAttribute;

                    return true;
                }
            }

            attribute = null;

            return false;
        }

        private Diagnostic CreateMemberDiagnostic(DiagnosticDescriptor descriptor, INamedTypeSymbol recordType, ISymbol recordMember, string issue)
        {
            var location = recordMember.Locations.FirstOrDefault(x => x.IsInSource && _compilation.ContainsSyntaxTree(x.SourceTree)) ?? recordType.Locations.FirstOrDefault();

            return Diagnostic.Create(descriptor, location, recordMember.ToDisplayString(), issue);
        }
    }
}
