// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using System.Threading;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    [Generator(LanguageNames.CSharp)]
    public sealed class TabularHandlerGenerator : IIncrementalGenerator
    {
        private static readonly TabularHandlerEmitter s_emitter = new TabularHandlerEmitter();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var parser = context.CompilationProvider
                .Select(CreateParser);

            var parseResults = context.SyntaxProvider
                .ForAttributeWithMetadataName("Addax.Formats.Tabular.TabularRecordAttribute", FilterSyntaxNode, TransformSyntaxContext)
                .Combine(parser)
                .Select(ParseRecordType)
                .Collect();

            var recordMappings = parseResults
                .Select(GetRecordMappings)
                .WithTrackingName("RecordMappings");

            var diagnostics = parseResults
                .Select(GetDiagnostics);

            context.RegisterImplementationSourceOutput(recordMappings, GenerateSources);
            context.RegisterSourceOutput(diagnostics, ReportDiagnostics);
        }

        private static TabularHandlerParser CreateParser(Compilation compilation, CancellationToken cancellationToken)
        {
            foreach (var reference in compilation.References)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol assembly)
                {
                    if (assembly.Name == "Addax.Formats.Tabular")
                    {
                        return new TabularHandlerParser((CSharpCompilation)compilation, assembly);
                    }
                }
            }

            return null;
        }

        private static (TabularRecordMapping? RecordMapping, ImmutableArray<Diagnostic> Diagnostics) ParseRecordType((INamedTypeSymbol RecordType, TabularHandlerParser Parser) parseArgs, CancellationToken cancellationToken)
        {
            if (parseArgs.Parser == null)
            {
                return (null, ImmutableArray<Diagnostic>.Empty);
            }

            return parseArgs.Parser.GetRecordMapping(parseArgs.RecordType, cancellationToken);
        }

        private static EquatableArray<TabularRecordMapping> GetRecordMappings(ImmutableArray<(TabularRecordMapping? RecordMapping, ImmutableArray<Diagnostic> Diagnostics)> parseResults, CancellationToken cancellationToken)
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

        private static ImmutableArray<Diagnostic> GetDiagnostics(ImmutableArray<(TabularRecordMapping? RecordMapping, ImmutableArray<Diagnostic> Diagnostics)> parseResults, CancellationToken cancellationToken)
        {
            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();

            foreach (var parseResult in parseResults)
            {
                diagnosticsBuilder.AddRange(parseResult.Diagnostics);
            }

            return diagnosticsBuilder.ToImmutable();
        }

        private static void GenerateSources(SourceProductionContext context, EquatableArray<TabularRecordMapping> recordMappings)
        {
            s_emitter.EmitRecordMappings(context, recordMappings);
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
