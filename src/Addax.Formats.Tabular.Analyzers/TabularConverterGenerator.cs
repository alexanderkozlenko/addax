// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers;

[Generator(LanguageNames.CSharp)]
public sealed partial class TabularConverterGenerator : IIncrementalGenerator
{
    private static readonly Version _assemblyVersion = typeof(TabularConverterGenerator).Assembly.GetName().Version;

    private static readonly Parser _parser = new();
    private static readonly Emitter _emitter = new();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var source = context
            .CompilationProvider
            .Combine(context
                .SyntaxProvider
                .ForAttributeWithMetadataName("Addax.Formats.Tabular.TabularRecordAttribute", FilterSyntaxNode, TransformSyntaxContext)
                .Collect());

        context.RegisterImplementationSourceOutput(source, GenerateImplementationSourceOutput);
    }

    private static bool FilterSyntaxNode(SyntaxNode syntax, CancellationToken cancellationToken)
    {
        return syntax.Kind() is
            SyntaxKind.ClassDeclaration or
            SyntaxKind.StructDeclaration or
            SyntaxKind.RecordDeclaration or
            SyntaxKind.RecordStructDeclaration;
    }

    private static (INamedTypeSymbol, AttributeData) TransformSyntaxContext(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        return ((INamedTypeSymbol)context.TargetSymbol, context.Attributes[0]);
    }

    private static void GenerateImplementationSourceOutput(SourceProductionContext context, (Compilation, ImmutableArray<(INamedTypeSymbol, AttributeData)>) value)
    {
        var (compilation, types) = value;

        if (types.IsDefaultOrEmpty)
        {
            return;
        }
        if (!compilation.ReferencedAssemblyNames.Any(static x => (x.Name == "Addax.Formats.Tabular") && _assemblyVersion.Equals(x.Version)))
        {
            return;
        }
        if (!_parser.TryGetImplementationSourceSpec(context, types, out var generatorSpec))
        {
            return;
        }

        _emitter.EmitImplementationSourceOutput(context, generatorSpec);
    }
}
