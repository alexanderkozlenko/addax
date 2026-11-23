// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal partial class TabularHandlerParser
    {
        private static void AddExperimentalDiagnosticIdentifiers(ISymbol symbol, HashSet<string> diagnosticIdentifiers)
        {
            if (symbol == null)
            {
                return;
            }

            if (symbol is IArrayTypeSymbol arrayType)
            {
                AddExperimentalDiagnosticIdentifiers(arrayType.ElementType, diagnosticIdentifiers);
            }
            else if (symbol is INamedTypeSymbol namedType)
            {
                while (namedType != null)
                {
                    AddExperimentalDiagnosticIdentifier(namedType, diagnosticIdentifiers);

                    foreach (var typeArgument in namedType.TypeArguments)
                    {
                        AddExperimentalDiagnosticIdentifiers(typeArgument, diagnosticIdentifiers);
                    }

                    namedType = namedType.ContainingType;
                }
            }
            else
            {
                AddExperimentalDiagnosticIdentifier(symbol, diagnosticIdentifiers);
            }
        }

        private static void AddExperimentalDiagnosticIdentifier(ISymbol symbol, HashSet<string> diagnosticIdentifiers)
        {
            if (TryGetExperimentalDiagnosticIdentifier(symbol, out var diagnosticIdentifier) ||
                TryGetExperimentalDiagnosticIdentifier(symbol.ContainingModule, out diagnosticIdentifier) ||
                TryGetExperimentalDiagnosticIdentifier(symbol.ContainingAssembly, out diagnosticIdentifier))
            {
                diagnosticIdentifiers.Add(diagnosticIdentifier);
            }
        }

        private static bool TryGetExperimentalDiagnosticIdentifier(ISymbol symbol, out string diagnosticIdentifier)
        {
            if (symbol != null)
            {
                foreach (var attribute in symbol.GetAttributes())
                {
                    if ((attribute.AttributeClass?.ToDisplayString() == "System.Diagnostics.CodeAnalysis.ExperimentalAttribute") &&
                        TryGetConstructorArgument<string>(attribute, out var attributeDiagnosticIdentifier) &&
                        SyntaxFacts.IsValidIdentifier(attributeDiagnosticIdentifier))
                    {
                        diagnosticIdentifier = attributeDiagnosticIdentifier;

                        return true;
                    }
                }
            }

            diagnosticIdentifier = null;

            return false;
        }
    }
}
