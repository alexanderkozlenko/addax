// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers;

internal partial class TabularConverterParser
{
    private static readonly DiagnosticDescriptor _diagnostic0001 = new(
        id: "TAB0001",
        title: "A record cannot be represented as a ref-like value type",
        messageFormat: "A record cannot be represented as a ref-like value type",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0011 = new(
        id: "TAB0011",
        title: "A field converter must derive from 'TabularFieldConverter<T>'",
        messageFormat: "A field converter must derive from 'TabularFieldConverter<T>'",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0012 = new(
        id: "TAB0013",
        title: "An explicitly applied field converter must handle the proper type",
        messageFormat: "An explicitly-applied field converter must handle the proper type",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0013 = new(
        id: "TAB0013",
        title: "An explicitly applied field converter must have a parameterless constructor",
        messageFormat: "An explicitly-applied field converter must have a parameterless constructor",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0021 = new(
        id: "TAB0021",
        title: "A field index must have a unique zero-based value",
        messageFormat: "A field index must have a unique zero-based value",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
