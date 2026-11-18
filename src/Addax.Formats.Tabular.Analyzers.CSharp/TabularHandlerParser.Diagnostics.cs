// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal partial class TabularHandlerParser
    {
        private static readonly DiagnosticDescriptor s_diagnostic0000 = new DiagnosticDescriptor(
            "TAB0000",
            "The current language version is not supported by the source generator",
            "The current language version '{0}' is not supported by the source generator",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0001 = new DiagnosticDescriptor(
            "TAB0001",
            "The type cannot be used for tabular record mapping",
            "The type '{0}' cannot be used for tabular record mapping: {1}",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0002 = new DiagnosticDescriptor(
            "TAB0002",
            "The type member cannot be used for tabular field mapping",
            "The type member '{0}' cannot be used for tabular field mapping: {1}",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0003 = new DiagnosticDescriptor(
            "TAB0003",
            "The type member requires a converter for tabular field mapping",
            "The type member '{0}' requires a converter for tabular field mapping: {1}",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0004 = new DiagnosticDescriptor(
            "TAB0004",
            "The specified converter cannot be used for tabular field mapping",
            "The specified converter cannot be used for tabular field mapping of '{0}': {1}",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);

        private static readonly DiagnosticDescriptor s_diagnostic0005 = new DiagnosticDescriptor(
            "TAB0005",
            "The type member has an invalid tabular field mapping",
            "The type member '{0}' has an invalid tabular field mapping: {1}",
            "Addax.Formats.Tabular",
            DiagnosticSeverity.Error,
            true,
            customTags: WellKnownDiagnosticTags.NotConfigurable);
    }
}
