// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal partial class TabularHandlerParser
    {
        public TabularConverterParseResult GetConverterMappings(CancellationToken cancellationToken)
        {
            if (_compilation.LanguageVersion < s_minimumLanguageVersion)
            {
                return new TabularConverterParseResult(new EquatableArray<TabularConverterMapping>(ImmutableArray<TabularConverterMapping>.Empty), ImmutableArray<Diagnostic>.Empty);
            }

            var converterMappingsBuilder = ImmutableArray.CreateBuilder<TabularConverterMapping>();
            var converterTypes = new Dictionary<string, INamedTypeSymbol>(StringComparer.Ordinal);
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

            foreach (var attribute in _compilation.Assembly.GetAttributes())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!SymbolEqualityComparer.Default.Equals(attribute.AttributeClass?.ConstructedFrom, _converterAttributeType))
                {
                    continue;
                }

                var location = attribute.ApplicationSyntaxReference?.GetSyntax(cancellationToken).GetLocation() ?? Location.None;

                if (!TryGetSupportedConverter(attribute, null, out var converterType, out var converterValueType, out var converterConstructor, out var converterIssue))
                {
                    if (converterIssue != null)
                    {
                        diagnosticsBuilder.Add(Diagnostic.Create(s_diagnostic0004, location, _compilation.Assembly.Name, converterIssue));
                    }

                    continue;
                }

                if ((converterValueType is INamedTypeSymbol converterNamedValueType) && (converterNamedValueType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T))
                {
                    diagnosticsBuilder.Add(Diagnostic.Create(s_diagnostic0004, location, _compilation.Assembly.Name, $"'{converterType.ToDisplayString()}' converts the nullable type '{converterValueType.ToDisplayString()}'"));

                    continue;
                }

                var valueTypeKey = GetValueTypeKey(converterValueType);

                if (converterTypes.TryGetValue(valueTypeKey, out var otherConverterType))
                {
                    diagnosticsBuilder.Add(Diagnostic.Create(s_diagnostic0005, location, converterValueType.ToDisplayString(), otherConverterType.ToDisplayString(), converterType.ToDisplayString()));

                    continue;
                }

                var converterDiagnosticIdentifiers = new HashSet<string>(StringComparer.Ordinal);

                AddExperimentalDiagnosticIdentifiers(converterType, converterDiagnosticIdentifiers);
                AddExperimentalDiagnosticIdentifiers(converterConstructor, converterDiagnosticIdentifiers);

                var converterMapping = new TabularConverterMapping(
                    valueTypeKey,
                    converterType.ToDisplayString(s_displayFormat),
                    new EquatableArray<string>(converterDiagnosticIdentifiers.ToImmutableArray().Sort(StringComparer.Ordinal)));

                converterTypes.Add(valueTypeKey, converterType);
                converterMappingsBuilder.Add(converterMapping);
            }

            return new TabularConverterParseResult(new EquatableArray<TabularConverterMapping>(converterMappingsBuilder.ToImmutable()), diagnosticsBuilder.ToImmutable());
        }

        private string GetValueTypeKey(ITypeSymbol valueType)
        {
            var objectTypeKey = _compilation.ObjectType.ToDisplayString(s_valueTypeKeyFormat);
            var valueTypeKeyBuilder = new StringBuilder();

            foreach (var valueTypePart in valueType.ToDisplayParts(s_valueTypeKeyFormat))
            {
                valueTypeKeyBuilder.Append((valueTypePart.Symbol is IDynamicTypeSymbol) ? objectTypeKey : valueTypePart.ToString());
            }

            return valueTypeKeyBuilder.ToString();
        }

        private bool TryGetSupportedConverter(AttributeData attribute, ITypeSymbol valueType, out INamedTypeSymbol converterType, out ITypeSymbol converterValueType, out IMethodSymbol converterConstructor, out string issue)
        {
            converterType = null;
            converterValueType = null;
            converterConstructor = null;

            var converterTypeSymbol = attribute.AttributeClass.TypeArguments[0];

            if (!ConverterTypeIsUsable(converterTypeSymbol, out issue))
            {
                return false;
            }

            var converterTypeName = converterTypeSymbol.ToDisplayString();

            if (!(converterTypeSymbol is INamedTypeSymbol converterNamedType) || !TryGetConverterValueType(converterNamedType, out var converterNamedTypeValueType))
            {
                issue = null;

                return false;
            }

            foreach (var instanceConstructor in converterNamedType.InstanceConstructors)
            {
                if ((instanceConstructor.DeclaredAccessibility == Accessibility.Public) && instanceConstructor.Parameters.IsDefaultOrEmpty)
                {
                    converterConstructor = instanceConstructor;

                    break;
                }
            }

            if (converterConstructor == null)
            {
                issue = null;

                return false;
            }

            if (!TypeRequiredMembersAreSet(converterNamedType, ImmutableHashSet<string>.Empty))
            {
                issue = null;

                foreach (var constructorAttribute in converterConstructor.GetAttributes())
                {
                    if (constructorAttribute.AttributeClass?.ToDisplayString() == "System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute")
                    {
                        issue = $"'{converterTypeName}' has required members";

                        break;
                    }
                }

                converterConstructor = null;

                return false;
            }

            if ((valueType != null) && !_compilation.ClassifyConversion(converterNamedTypeValueType, valueType).IsIdentity)
            {
                issue = $"'{converterTypeName}' converts '{converterNamedTypeValueType.ToDisplayString()}', but the member type is '{valueType.ToDisplayString()}'";
                converterConstructor = null;

                return false;
            }

            converterType = converterNamedType;
            converterValueType = converterNamedTypeValueType;
            issue = null;

            return true;
        }

        private bool TryGetConverterValueType(INamedTypeSymbol converterType, out ITypeSymbol converterValueType)
        {
            while (converterType != null)
            {
                if (SymbolEqualityComparer.Default.Equals(converterType.ConstructedFrom, _converterBaseType))
                {
                    converterValueType = converterType.TypeArguments[0];

                    return true;
                }

                converterType = converterType.BaseType;
            }

            converterValueType = null;

            return false;
        }

        private bool ConverterTypeIsUsable(ITypeSymbol type, out string issue)
        {
            if (type is IArrayTypeSymbol arrayType)
            {
                return ConverterTypeIsUsable(arrayType.ElementType, out issue);
            }

            if ((type.TypeKind == TypeKind.Pointer) || (type.TypeKind == TypeKind.FunctionPointer))
            {
                issue = $"'{type.ToDisplayString()}' is a pointer type";

                return false;
            }

            if (!(type is INamedTypeSymbol namedType) || (namedType.TypeKind == TypeKind.Error))
            {
                issue = null;

                return false;
            }

            if (!_compilation.IsSymbolAccessibleWithin(namedType, _compilation.Assembly))
            {
                issue = $"'{GetOutermostInaccessibleType(namedType).ToDisplayString()}' is not accessible within the assembly";

                return false;
            }

            while (namedType != null)
            {
                if (namedType.IsFileLocal)
                {
                    issue = $"'{namedType.ToDisplayString()}' is file-local";

                    return false;
                }

                foreach (var typeArgument in namedType.TypeArguments)
                {
                    if (!ConverterTypeIsUsable(typeArgument, out issue))
                    {
                        return false;
                    }
                }

                namedType = namedType.ContainingType;
            }

            issue = null;

            return true;
        }
    }
}
