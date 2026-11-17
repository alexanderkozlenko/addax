using System.Globalization;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Addax.Formats.Tabular.Analyzers.CSharp.UnitTests;

public sealed partial class TabularHandlerGeneratorTests
{
    private sealed class AssetResult
    {
        private readonly Compilation _compilation;
        private readonly IReadOnlyList<SyntaxTree> _generatedTrees;
        private readonly IReadOnlyList<(INamedTypeSymbol Record, Diagnostic Diagnostic)> _generatorDiagnostics;
        private readonly IReadOnlyList<INamedTypeSymbol> _records;

        public AssetResult(
            Compilation compilation,
            IReadOnlyList<Diagnostic> compilationDiagnostics,
            Assembly? assembly,
            IReadOnlyList<SyntaxTree> generatedTrees,
            IReadOnlyList<(INamedTypeSymbol Record, Diagnostic Diagnostic)> generatorDiagnostics,
            IReadOnlyList<INamedTypeSymbol> records)
        {
            _compilation = compilation;
            _generatedTrees = generatedTrees;
            _generatorDiagnostics = generatorDiagnostics;
            _records = records;

            CompilationDiagnostics = compilationDiagnostics;
            Assembly = assembly;
        }

        public string GetRecordOutcome(string recordTypeName)
        {
            var record = _records.Single(x => x.ToDisplayString() == recordTypeName);
            var diagnostics = _generatorDiagnostics.Where(x => SymbolEqualityComparer.Default.Equals(x.Record, record)).Select(static x => x.Diagnostic).ToArray();
            var handlerBaseType = _compilation.GetTypeByMetadataName("Addax.Formats.Tabular.TabularHandler`1")!.Construct(record);
            var handlers = new List<INamedTypeSymbol>();

            foreach (var generatedTree in _generatedTrees)
            {
                var semanticModel = _compilation.GetSemanticModel(generatedTree);

                foreach (var classDeclaration in generatedTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    var handler = semanticModel.GetDeclaredSymbol(classDeclaration)!;

                    if (SymbolEqualityComparer.Default.Equals(handler.BaseType, handlerBaseType))
                    {
                        handlers.Add(handler);
                    }
                }
            }

            if (diagnostics.Length != 0)
            {
                Assert.IsEmpty(handlers);

                return string.Join("; ", diagnostics.Select(FormatDiagnostic));
            }

            Assert.HasCount(1, handlers);

            var overrides = handlers[0].GetMembers().Where(static x => x.IsOverride).Select(static x => x.Name).ToHashSet(StringComparer.Ordinal);
            var capabilities = new List<string>();

            Assert.AreEqual(overrides.Contains("Read"), overrides.Contains("ReadAsync"));
            Assert.AreEqual(overrides.Contains("Write"), overrides.Contains("WriteAsync"));

            if (overrides.Contains("Read"))
            {
                capabilities.Add("Read");
            }

            if (overrides.Contains("Write"))
            {
                capabilities.Add("Write");
            }

            if (overrides.Contains("Header"))
            {
                capabilities.Add("Header");
            }

            return (capabilities.Count != 0) ? string.Join(", ", capabilities) : "None";
        }

        private static string FormatDiagnostic(Diagnostic diagnostic)
        {
            var match = DiagnosticArgumentRegex.Match(diagnostic.GetMessage(CultureInfo.InvariantCulture));

            return match.Success ? $"{diagnostic.Id} {match.Groups["argument"].Value}" : diagnostic.Id;
        }

        public IReadOnlyList<Diagnostic> CompilationDiagnostics
        {
            get;
        }

        public Assembly? Assembly
        {
            get;
        }
    }
}
