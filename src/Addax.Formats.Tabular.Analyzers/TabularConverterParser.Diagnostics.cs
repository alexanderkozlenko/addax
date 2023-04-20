// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers;

internal partial class TabularConverterParser
{
    private static readonly DiagnosticDescriptor _diagnostic0001 = new(
        id: "TAB0001",
        title: "A tabular record cannot be mapped to a static class",
        messageFormat: "A tabular record cannot be mapped to a static class",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0002 = new(
        id: "TAB0002",
        title: "A tabular record cannot be mapped to a stack-only structure",
        messageFormat: "A tabular record cannot be mapped to a stack-only structure",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0011 = new(
        id: "TAB0011",
        title: "A tabular field cannot be mapped to a static type member",
        messageFormat: "A tabular field cannot be mapped to a static type member",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0012 = new(
        id: "TAB0012",
        title: "A tabular field cannot be mapped to an inaccessible type member",
        messageFormat: "A tabular field cannot be mapped to an inaccessible type member",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0013 = new(
        id: "TAB0013",
        title: "A tabular field converter must derive from 'TabularFieldConverter<T>' type",
        messageFormat: "A tabular field converter must derive from 'TabularFieldConverter<T>' type",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0014 = new(
        id: "TAB0014",
        title: "A tabular field converter specified for a type member must handle the corresponding type",
        messageFormat: "A tabular field converter specified for a type member must handle the corresponding type",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0015 = new(
        id: "TAB0015",
        title: "A tabular field converter specified for a type member must have a parameterless constructor",
        messageFormat: "A tabular field converter specified for a type member must have a parameterless constructor",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor _diagnostic0021 = new(
        id: "TAB0021",
        title: "A tabular field index must have a unique zero-based value",
        messageFormat: "A tabular field index must have a unique zero-based value",
        category: "Addax.Formats.Tabular",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
