// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal partial class TabularHandlerParser
    {
        private static bool TypeRequiredMembersAreSet(INamedTypeSymbol type, HashSet<string> setMemberNames)
        {
            for (var currentType = type; currentType != null; currentType = currentType.BaseType)
            {
                foreach (var member in currentType.GetMembers())
                {
                    var memberIsRequired =
                        ((member is IFieldSymbol field) && field.IsRequired) ||
                        ((member is IPropertySymbol property) && property.IsRequired);

                    if (memberIsRequired && !setMemberNames.Contains(member.Name))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void DisableFieldWriting(ImmutableDictionary<int, TabularFieldMapping>.Builder fieldMappingsBuilder)
        {
            var fieldOrders = fieldMappingsBuilder.Keys.ToArray();

            foreach (var fieldOrder in fieldOrders)
            {
                var fieldMapping = fieldMappingsBuilder[fieldOrder];

                if (fieldMapping.SupportsReading)
                {
                    fieldMappingsBuilder[fieldOrder] = new TabularFieldMapping(
                        fieldMapping.FieldOrder,
                        fieldMapping.MemberIdentifier,
                        fieldMapping.SupportsReading,
                        false,
                        fieldMapping.IsNullableT,
                        fieldMapping.ValueTypeName,
                        fieldMapping.ConverterTypeName,
                        fieldMapping.FieldNameLiteral);
                }
                else
                {
                    fieldMappingsBuilder.Remove(fieldOrder);
                }
            }
        }

        private bool TryApplyConstruction(INamedTypeSymbol type, ImmutableDictionary<int, TabularFieldMapping>.Builder fieldMappingsBuilder, Dictionary<int, ISymbol> fieldMembers, out IMethodSymbol constructor, out ImmutableArray<int> constructorArguments)
        {
            constructor = null;
            constructorArguments = ImmutableArray<int>.Empty;

            if (type.IsAbstract)
            {
                return false;
            }

            var settableMemberNames = new HashSet<string>(StringComparer.Ordinal);

            foreach (var kvp in fieldMappingsBuilder)
            {
                if (kvp.Value.SupportsWriting)
                {
                    settableMemberNames.Add(fieldMembers[kvp.Key].Name);
                }
            }

            var hasDefaultConstructor = TryGetParameterlessConstructor(type, out var defaultConstructor) && _compilation.IsSymbolAccessibleWithin(defaultConstructor, _compilation.Assembly);

            if (hasDefaultConstructor && defaultConstructor.IsImplicitlyDeclared && TypeDeclaresParameterizedConstructors(type))
            {
                hasDefaultConstructor = false;
            }

            if ((settableMemberNames.Count != 0) && hasDefaultConstructor)
            {
                if (!TypeRequiredMembersAreSet(type, settableMemberNames))
                {
                    return false;
                }

                constructor = defaultConstructor;

                return true;
            }

            return TryApplyParameterizedConstruction(type, fieldMappingsBuilder, fieldMembers, out constructor, out constructorArguments);
        }

        private static bool TypeDeclaresParameterizedConstructors(INamedTypeSymbol type)
        {
            foreach (var instanceConstructor in type.InstanceConstructors)
            {
                if (instanceConstructor.Parameters.IsDefaultOrEmpty && !instanceConstructor.IsVararg)
                {
                    continue;
                }

                if ((instanceConstructor.Parameters.Length == 1) && SymbolEqualityComparer.Default.Equals(instanceConstructor.Parameters[0].Type, type))
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private bool TryApplyParameterizedConstruction(INamedTypeSymbol type, ImmutableDictionary<int, TabularFieldMapping>.Builder fieldMappingsBuilder, Dictionary<int, ISymbol> fieldMembers, out IMethodSymbol constructor, out ImmutableArray<int> constructorArguments)
        {
            constructor = null;
            constructorArguments = ImmutableArray<int>.Empty;

            if (!TryGetPrimaryConstructor(type, out var primaryConstructor))
            {
                return false;
            }

            if (!TypeRequiredMembersAreSet(type, new HashSet<string>(StringComparer.Ordinal)))
            {
                return false;
            }

            if (!TryBindConstructor(primaryConstructor, fieldMappingsBuilder, fieldMembers, out constructorArguments))
            {
                return false;
            }

            constructor = primaryConstructor;

            return true;
        }

        private static bool TryGetPrimaryConstructor(INamedTypeSymbol type, out IMethodSymbol constructor)
        {
            foreach (var instanceConstructor in type.InstanceConstructors)
            {
                if ((instanceConstructor.DeclaringSyntaxReferences.Length == 1) &&
                    (instanceConstructor.DeclaringSyntaxReferences[0].GetSyntax() is RecordDeclarationSyntax) &&
                    !instanceConstructor.Parameters.IsDefaultOrEmpty)
                {
                    constructor = instanceConstructor;

                    return true;
                }
            }

            constructor = null;

            return false;
        }

        private static bool TryBindConstructor(IMethodSymbol constructor, ImmutableDictionary<int, TabularFieldMapping>.Builder fieldMappingsBuilder, Dictionary<int, ISymbol> fieldMembers, out ImmutableArray<int> constructorArguments)
        {
            var argumentsBuilder = ImmutableArray.CreateBuilder<int>(constructor.Parameters.Length);

            constructorArguments = ImmutableArray<int>.Empty;

            foreach (var parameter in constructor.Parameters)
            {
                if ((parameter.RefKind != RefKind.None) || parameter.IsOptional || parameter.IsParams)
                {
                    return false;
                }

                if (!TryGetLinkedFieldOrder(parameter, fieldMembers, out var fieldOrder))
                {
                    return false;
                }

                argumentsBuilder.Add(fieldOrder);
            }

            foreach (var kvp in fieldMappingsBuilder)
            {
                if (kvp.Value.SupportsWriting && !argumentsBuilder.Contains(kvp.Key))
                {
                    return false;
                }
            }

            foreach (var fieldOrder in argumentsBuilder)
            {
                var fieldMapping = fieldMappingsBuilder[fieldOrder];

                fieldMappingsBuilder[fieldOrder] = new TabularFieldMapping(
                    fieldMapping.FieldOrder,
                    fieldMapping.MemberIdentifier,
                    fieldMapping.SupportsReading,
                    true,
                    fieldMapping.IsNullableT,
                    fieldMapping.ValueTypeName,
                    fieldMapping.ConverterTypeName,
                    fieldMapping.FieldNameLiteral);
            }

            constructorArguments = argumentsBuilder.MoveToImmutable();

            return true;
        }

        private static bool TryGetLinkedFieldOrder(IParameterSymbol parameter, Dictionary<int, ISymbol> fieldMembers, out int fieldOrder)
        {
            if (parameter.DeclaringSyntaxReferences.Length == 1)
            {
                var parameterSyntax = parameter.DeclaringSyntaxReferences[0];

                foreach (var kvp in fieldMembers)
                {
                    var memberSyntaxReferences = kvp.Value.DeclaringSyntaxReferences;

                    if ((memberSyntaxReferences.Length == 1) &&
                        (memberSyntaxReferences[0].SyntaxTree == parameterSyntax.SyntaxTree) &&
                        (memberSyntaxReferences[0].Span == parameterSyntax.Span))
                    {
                        fieldOrder = kvp.Key;

                        return true;
                    }
                }
            }

            fieldOrder = default;

            return false;
        }

        private static bool TryGetParameterlessConstructor(INamedTypeSymbol type, out IMethodSymbol constructor)
        {
            foreach (var instanceConstructor in type.InstanceConstructors)
            {
                if (instanceConstructor.Parameters.IsDefaultOrEmpty && !instanceConstructor.IsVararg)
                {
                    constructor = instanceConstructor;

                    return true;
                }
            }

            constructor = null;

            return false;
        }
    }
}
