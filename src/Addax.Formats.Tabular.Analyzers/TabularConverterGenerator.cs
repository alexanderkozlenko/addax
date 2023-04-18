// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers;

[Generator(LanguageNames.CSharp)]
public sealed partial class TabularConverterGenerator : IIncrementalGenerator
{
    private static readonly Version _assemblyVersion = typeof(TabularConverterGenerator).Assembly.GetName().Version;

    private static readonly TabularConverterParser _parser = new();
    private static readonly TabularConverterEmitter _emitter = new();

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

        if (types.IsDefaultOrEmpty || !CompilationHasRequiredTypes(compilation, context.CancellationToken))
        {
            return;
        }

        var sourceSpec = _parser.GetImplementationSourceSpec(context, types);

        _emitter.EmitImplementationSourceOutput(context, sourceSpec);
    }

    private static bool CompilationHasRequiredTypes(Compilation compilation, CancellationToken cancellationToken)
    {
        foreach (var assembly in compilation.ReferencedAssemblyNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ((assembly.Name == "Addax.Formats.Tabular") && _assemblyVersion.Equals(assembly.Version))
            {
                return true;
            }
        }

        return false;
    }
}
