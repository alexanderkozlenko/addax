using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Addax.Formats.Tabular.Analyzers.CSharp.UnitTests;

[TestClass]
public sealed class TabularHandlerGeneratorTests
{
    private static readonly Compilation s_compilation = CreateCompilation();

    [TestMethod]
    [DataRow("NamespaceHasNoMappings", 0)]
    [DataRow("TypeHasSkippedIndices", 2)]
    [DataRow("TypeMemberHasName", 2)]
    [DataRow("TypeMemberHasSupportedType", 2)]
    [DataRow("TypeMemberHasSupportedTypeAsNullableT", 2)]
    [DataRow("TypeMemberHasUnsupportedTypeWithConverter", 2)]
    [DataRow("TypeMemberIsReadOnly", 2)]
    public void RunPositive(string inputFileName, int generatedFileCount)
    {
        var inputSourceText = SourceText.From(File.OpenRead(
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets", $"{inputFileName}.cs")));

        var compilation = s_compilation
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(inputSourceText));

        var generatorDriver = CSharpGeneratorDriver
            .Create(new TabularHandlerGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out compilation, out var generatorDiagnostics);

        var generatedSources = generatorDriver.GetRunResult().Results.First().GeneratedSources;
        var compilationDiagnostics = compilation.GetDiagnostics();

        Assert.AreEqual(0, generatorDiagnostics.Length);
        Assert.AreEqual(generatedFileCount, generatedSources.Length);
        Assert.AreEqual(0, compilationDiagnostics.Length);
    }

    [TestMethod]
    [DataRow("ConverterValueTypeMismatch", "TAB0004")]
    [DataRow("TypeMemberHasDuplicateIndex", "TAB0005")]
    [DataRow("TypeMemberHasUnsupportedType", "TAB0003")]
    public void RunNegative(string inputFileName, string diagnosticId)
    {
        var inputSourceText = SourceText.From(File.OpenRead(
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets", $"{inputFileName}.cs")));

        var compilation = s_compilation
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(inputSourceText));

        var generatorDriver = CSharpGeneratorDriver
            .Create(new TabularHandlerGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out compilation, out var generatorDiagnostics);

        var compilationDiagnostics = compilation.GetDiagnostics();

        Assert.AreEqual(1, generatorDiagnostics.Length);
        Assert.AreEqual(diagnosticId, generatorDiagnostics.Single().Id);
        Assert.AreEqual(0, compilationDiagnostics.Length);
    }

    private static CSharpCompilation CreateCompilation()
    {
        var runtimeDirectoryName = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        return CSharpCompilation.Create(
            "MyAssembly",
            [],
            [
                MetadataReference.CreateFromFile(Path.Combine(runtimeDirectoryName, "System.Memory.dll")),
                MetadataReference.CreateFromFile(Path.Combine(runtimeDirectoryName, "System.Private.CoreLib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(runtimeDirectoryName, "System.Private.Uri.dll")),
                MetadataReference.CreateFromFile(Path.Combine(runtimeDirectoryName, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(runtimeDirectoryName, "System.Runtime.Numerics.dll")),
                MetadataReference.CreateFromFile(typeof(TabularContentException).Assembly.Location),
            ],
            new(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                deterministic: true,
                nullableContextOptions: NullableContextOptions.Enable));
    }
}
