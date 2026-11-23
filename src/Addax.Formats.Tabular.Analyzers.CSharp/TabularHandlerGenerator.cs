// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    [Generator(LanguageNames.CSharp)]
    public sealed class TabularHandlerGenerator : IIncrementalGenerator
    {
        public TabularHandlerGenerator()
        {
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var parser = context.CompilationProvider
                .Select(TabularHandlerParser.Create);

            var converterParseResult = parser
                .Select(ParseConverters);

            var parseResults = context.SyntaxProvider
                .ForAttributeWithMetadataName("Addax.Formats.Tabular.TabularRecordAttribute", FilterSyntaxNode, TransformSyntaxContext)
                .Combine(parser)
                .Combine(converterParseResult)
                .Select(ParseRecordType)
                .Collect();

            var recordMappings = parseResults
                .Select(GetRecordMappings)
                .WithTrackingName("RecordMappings");

            var diagnostics = parseResults
                .Combine(converterParseResult)
                .Select(GetDiagnostics);

            context.RegisterImplementationSourceOutput(recordMappings, TabularHandlerEmitter.EmitRecordMappings);
            context.RegisterSourceOutput(diagnostics, ReportDiagnostics);
        }

        private static TabularConverterParseResult ParseConverters(TabularHandlerParser parser, CancellationToken cancellationToken)
        {
            if (parser == null)
            {
                return new TabularConverterParseResult(new EquatableArray<TabularConverterMapping>(ImmutableArray<TabularConverterMapping>.Empty), ImmutableArray<Diagnostic>.Empty);
            }

            return parser.GetConverterMappings(cancellationToken);
        }

        private static TabularRecordParseResult ParseRecordType(((INamedTypeSymbol RecordType, TabularHandlerParser Parser) Record, TabularConverterParseResult ConverterParseResult) parseArgs, CancellationToken cancellationToken)
        {
            if (parseArgs.Record.Parser == null)
            {
                return new TabularRecordParseResult(null, ImmutableArray<Diagnostic>.Empty);
            }

            return parseArgs.Record.Parser.GetRecordMapping(parseArgs.Record.RecordType, parseArgs.ConverterParseResult.ConverterMappings, cancellationToken);
        }

        private static EquatableArray<TabularRecordMapping> GetRecordMappings(ImmutableArray<TabularRecordParseResult> parseResults, CancellationToken cancellationToken)
        {
            var recordMappingsBuilder = ImmutableArray.CreateBuilder<TabularRecordMapping>(parseResults.Length);

            foreach (var parseResult in parseResults)
            {
                if (parseResult.RecordMapping.HasValue)
                {
                    recordMappingsBuilder.Add(parseResult.RecordMapping.Value);
                }
            }

            return new EquatableArray<TabularRecordMapping>(recordMappingsBuilder.ToImmutable());
        }

        private static ImmutableArray<Diagnostic> GetDiagnostics((ImmutableArray<TabularRecordParseResult> RecordParseResults, TabularConverterParseResult ConverterParseResult) parseResults, CancellationToken cancellationToken)
        {
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
            var reportedDiagnostics = new HashSet<(string Id, Location Location, string Message)>();

            diagnosticsBuilder.AddRange(parseResults.ConverterParseResult.Diagnostics);

            foreach (var parseResult in parseResults.RecordParseResults)
            {
                foreach (var diagnostic in parseResult.Diagnostics)
                {
                    if (reportedDiagnostics.Add((diagnostic.Id, diagnostic.Location, diagnostic.GetMessage(CultureInfo.InvariantCulture))))
                    {
                        diagnosticsBuilder.Add(diagnostic);
                    }
                }
            }

            return diagnosticsBuilder.ToImmutable();
        }

        private static void ReportDiagnostics(SourceProductionContext context, ImmutableArray<Diagnostic> diagnostics)
        {
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private static bool FilterSyntaxNode(SyntaxNode syntax, CancellationToken cancellationToken)
        {
            var syntaxKind = syntax.Kind();

            return
                (syntaxKind == SyntaxKind.ClassDeclaration) ||
                (syntaxKind == SyntaxKind.StructDeclaration) ||
                (syntaxKind == SyntaxKind.RecordDeclaration) ||
                (syntaxKind == SyntaxKind.RecordStructDeclaration);
        }

        private static INamedTypeSymbol TransformSyntaxContext(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
        {
            return (INamedTypeSymbol)context.TargetSymbol;
        }
    }
}
