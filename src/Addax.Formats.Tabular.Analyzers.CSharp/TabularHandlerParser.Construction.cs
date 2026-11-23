// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal partial class TabularHandlerParser
    {
        private bool TryGetRecordConstructor(INamedTypeSymbol type, Dictionary<int, ISymbol> settableMembers, out IMethodSymbol constructor, out ImmutableArray<int> constructorArguments)
        {
            constructor = null;
            constructorArguments = ImmutableArray<int>.Empty;

            if (type.IsAbstract)
            {
                return false;
            }

            if ((settableMembers.Count != 0) && TryGetDefaultConstructor(type, out var defaultConstructor))
            {
                var settableMemberNames = new HashSet<string>(StringComparer.Ordinal);

                foreach (var settableMember in settableMembers.Values)
                {
                    settableMemberNames.Add(settableMember.Name);
                }

                if (!TypeRequiredMembersAreSet(type, settableMemberNames))
                {
                    return false;
                }

                constructor = defaultConstructor;

                return true;
            }

            if (!TryGetPositionalConstructor(type, out var positionalConstructor))
            {
                return false;
            }

            if (!TypeRequiredMembersAreSet(type, ImmutableHashSet<string>.Empty))
            {
                return false;
            }

            var argumentsBuilder = ImmutableArray.CreateBuilder<int>(positionalConstructor.Parameters.Length);

            foreach (var parameter in positionalConstructor.Parameters)
            {
                if ((parameter.RefKind != RefKind.None) || parameter.IsOptional || parameter.IsParams)
                {
                    return false;
                }

                if (!TryGetLinkedFieldOrder(parameter, settableMembers, out var fieldOrder))
                {
                    return false;
                }

                argumentsBuilder.Add(fieldOrder);
            }

            foreach (var fieldOrder in settableMembers.Keys)
            {
                if (!argumentsBuilder.Contains(fieldOrder))
                {
                    return false;
                }
            }

            constructor = positionalConstructor;
            constructorArguments = argumentsBuilder.MoveToImmutable();

            return true;
        }

        private bool TryGetDefaultConstructor(INamedTypeSymbol type, out IMethodSymbol constructor)
        {
            foreach (var instanceConstructor in type.InstanceConstructors)
            {
                if (!instanceConstructor.Parameters.IsDefaultOrEmpty || instanceConstructor.IsVararg)
                {
                    continue;
                }

                if (!_compilation.IsSymbolAccessibleWithin(instanceConstructor, _compilation.Assembly))
                {
                    break;
                }

                if (instanceConstructor.IsImplicitlyDeclared)
                {
                    foreach (var otherConstructor in type.InstanceConstructors)
                    {
                        if (otherConstructor.Parameters.IsDefaultOrEmpty && !otherConstructor.IsVararg)
                        {
                            continue;
                        }

                        if ((otherConstructor.Parameters.Length == 1) && SymbolEqualityComparer.Default.Equals(otherConstructor.Parameters[0].Type, type))
                        {
                            continue;
                        }

                        constructor = null;

                        return false;
                    }
                }

                constructor = instanceConstructor;

                return true;
            }

            constructor = null;

            return false;
        }

        private static bool TryGetPositionalConstructor(INamedTypeSymbol type, out IMethodSymbol constructor)
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

        private static bool TryGetLinkedFieldOrder(IParameterSymbol parameter, Dictionary<int, ISymbol> settableMembers, out int fieldOrder)
        {
            if (parameter.DeclaringSyntaxReferences.Length == 1)
            {
                var parameterSyntax = parameter.DeclaringSyntaxReferences[0];

                foreach (var kvp in settableMembers)
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
    }
}
