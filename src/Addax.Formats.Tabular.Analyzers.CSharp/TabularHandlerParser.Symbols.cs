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

        private bool TryGetSupportedConverter(AttributeData attribute, INamedTypeSymbol converterBaseType, ITypeSymbol valueType, out INamedTypeSymbol converterType, out IMethodSymbol converterConstructor)
        {
            converterConstructor = null;

            return
                TryGetConverterType(attribute, out converterType) &&
                !converterType.IsAbstract &&
                TypeIsUsableInGeneratedCode(converterType) &&
                TypeRequiredMembersAreSet(converterType, new HashSet<string>(StringComparer.Ordinal)) &&
                TryGetConverterValueType(converterType, converterBaseType, out var converterValueType) &&
                _compilation.ClassifyConversion(converterValueType, valueType).IsIdentity &&
                TryGetConverterConstructor(converterType, out converterConstructor);
        }

        private static bool TryGetConverterType(AttributeData attribute, out INamedTypeSymbol converterType)
        {
            if ((attribute.AttributeClass != null) &&
                (attribute.AttributeClass.IsGenericType) &&
                (attribute.AttributeClass.TypeArguments.Length == 1) &&
                (attribute.AttributeClass.TypeArguments[0] is INamedTypeSymbol converterType1))
            {
                converterType = converterType1;

                return true;
            }

            if ((attribute.ConstructorArguments.Length == 1) &&
                (attribute.ConstructorArguments[0].Kind == TypedConstantKind.Type) &&
                (attribute.ConstructorArguments[0].Value is INamedTypeSymbol converterType2))
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

        private bool TypeMemberIsHidden(INamedTypeSymbol type, ISymbol member)
        {
            for (var currentType = type; currentType != null; currentType = currentType.BaseType)
            {
                foreach (var candidate in currentType.GetMembers(member.Name))
                {
                    if (SymbolEqualityComparer.Default.Equals(candidate, member))
                    {
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
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private bool TypeIsUsableInGeneratedCode(ITypeSymbol type)
        {
            if (type is IArrayTypeSymbol arrayType)
            {
                return TypeIsUsableInGeneratedCode(arrayType.ElementType);
            }

            if (type is IDynamicTypeSymbol)
            {
                return true;
            }

            if (!(type is INamedTypeSymbol namedType) ||
                (namedType.TypeKind == TypeKind.Error) ||
                namedType.IsUnboundGenericType ||
                !_compilation.IsSymbolAccessibleWithin(namedType, _compilation.Assembly))
            {
                return false;
            }

            for (var currentType = namedType; currentType != null; currentType = currentType.ContainingType)
            {
                if (currentType.IsFileLocal)
                {
                    return false;
                }

                foreach (var typeArgument in currentType.TypeArguments)
                {
                    if (!TypeIsUsableInGeneratedCode(typeArgument))
                    {
                        return false;
                    }
                }
            }

            return true;
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

        private Diagnostic CreateMemberDiagnostic(DiagnosticDescriptor descriptor, INamedTypeSymbol recordType, ISymbol recordMember)
        {
            var location = recordMember.Locations.FirstOrDefault(x => x.IsInSource && _compilation.ContainsSyntaxTree(x.SourceTree)) ?? recordType.Locations.FirstOrDefault();

            return Diagnostic.Create(descriptor, location, recordMember.ToDisplayString());
        }
    }
}
