// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    [Generator(LanguageNames.CSharp)]
    public sealed class TabularHandlerGenerator : IIncrementalGenerator
    {
        private static readonly TabularHandlerParser s_parser = new TabularHandlerParser();
        private static readonly TabularHandlerEmitter s_emitter = new TabularHandlerEmitter();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider
                .ForAttributeWithMetadataName("Addax.Formats.Tabular.TabularRecordAttribute", FilterSyntaxNode, TransformSyntaxContext)
                .Collect()
                .Combine(context.CompilationProvider);

            context.RegisterImplementationSourceOutput(provider, GenerateSources);
        }

        private static void GenerateSources(SourceProductionContext context, (ImmutableArray<INamedTypeSymbol> RecordTypes, Compilation Compilation) generateArgs)
        {
            if (generateArgs.RecordTypes.IsDefaultOrEmpty)
            {
                return;
            }

            var recordMappings = s_parser.GetRecordMappings((CSharpCompilation)generateArgs.Compilation, context, generateArgs.RecordTypes);

            if (recordMappings.IsDefaultOrEmpty)
            {
                return;
            }

            s_emitter.EmitRecordMappings(context, recordMappings);
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
            return context.TargetSymbol as INamedTypeSymbol;
        }
    }
}
