#pragma warning disable IDE1006

using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.Analyzers.UnitTests;

[TestClass]
public sealed class TabularSourceGeneratorTests
{
    private static readonly string _contentPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "Content");
    private static readonly string _assetsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "Assets");
    private static readonly string _runtimePath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

    [DataTestMethod]
    [DataRow("field-with-access-read-only.cs", 2, 0)]
    [DataRow("field-with-access-read-write.cs", 2, 0)]
    [DataRow("namespace-with-0-types.cs", 0, 0)]
    [DataRow("namespace-with-2-types.cs", 3, 0)]
    [DataRow("property-with-access-read-only.cs", 2, 0)]
    [DataRow("property-with-access-read-only-strict.cs", 2, 0)]
    [DataRow("property-with-access-read-only-all.cs", 2, 0)]
    [DataRow("property-with-access-read-only-all-strict.cs", 2, 0)]
    [DataRow("property-with-access-write-only.cs", 2, 0)]
    [DataRow("property-with-access-write-only-strict.cs", 2, 0)]
    [DataRow("property-with-access-write-only-all.cs", 2, 0)]
    [DataRow("property-with-access-write-only-all-strict.cs", 2, 0)]
    [DataRow("property-with-duplicate-field-indices.cs", 2, 1)]
    [DataRow("property-with-no-attribute.cs", 2, 0)]
    [DataRow("property-with-non-zero-field-index.cs", 2, 0)]
    [DataRow("property-with-non-zero-field-index-strict.cs", 2, 0)]
    [DataRow("property-with-unsupported-type.cs", 2, 0)]
    [DataRow("property-with-unsupported-type-generic.cs", 2, 0)]
    [DataRow("type-class.cs", 2, 0)]
    [DataRow("type-class-record.cs", 2, 0)]
    [DataRow("type-struct.cs", 2, 0)]
    [DataRow("type-struct-record.cs", 2, 0)]
    [DataRow("type-with-ctor.cs", 2, 0)]
    [DataRow("type-with-no-attribute.cs", 0, 0)]
    [DataRow("type-with-no-properties.cs", 2, 0)]
    [DataRow("type-with-supported-types.cs", 2, 0)]
    [DataRow("type-with-supported-types-strict.cs", 2, 0)]
    [DataRow("type-with-supported-types-nullable.cs", 2, 0)]
    [DataRow("type-with-supported-types-nullable-strict.cs", 2, 0)]
    public async Task Run(string filePath, int sourceCount, int diagnosticCount)
    {
        var staticSource =
            await File.ReadAllTextAsync(Path.Combine(_contentPath, "Addax.Formats.Tabular.g.cs"), CancellationToken);
        var inputSource =
            await File.ReadAllTextAsync(Path.Combine(_assetsPath, filePath), CancellationToken);

        var compilation = (Compilation)CSharpCompilation.Create("MyAssembly")
            .WithOptions(new(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable))
            .AddReferences(MetadataReference.CreateFromFile(CreateRuntimeAssemblyPath("System.Collections.Concurrent")))
            .AddReferences(MetadataReference.CreateFromFile(CreateRuntimeAssemblyPath("System.Memory")))
            .AddReferences(MetadataReference.CreateFromFile(CreateRuntimeAssemblyPath("System.Private.CoreLib")))
            .AddReferences(MetadataReference.CreateFromFile(CreateRuntimeAssemblyPath("System.Runtime")))
            .AddReferences(MetadataReference.CreateFromFile(CreateRuntimeAssemblyPath("System.Runtime.Numerics")))
            .AddReferences(MetadataReference.CreateFromFile(typeof(TabularDataException).Assembly.Location))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(staticSource, cancellationToken: CancellationToken))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(inputSource, cancellationToken: CancellationToken));

        var generatorDriver = CSharpGeneratorDriver.Create(new TabularConverterGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out compilation, out _, CancellationToken);

        var generatorDriverResult = generatorDriver.GetRunResult();
        var generatorResult = generatorDriverResult.Results.First();
        var compilationDiagnostics = compilation.GetDiagnostics(CancellationToken);

        Assert.AreEqual(1, generatorDriverResult.Results.Length);
        Assert.IsNull(generatorResult.Exception);
        Assert.AreEqual(diagnosticCount, generatorResult.Diagnostics.Length);
        Assert.AreEqual(sourceCount, generatorResult.GeneratedSources.Length);
        Assert.AreEqual(0, compilationDiagnostics.Length);
    }

    private static string CreateRuntimeAssemblyPath(string name)
    {
        return Path.Combine(_runtimePath, $"{name}.dll");
    }

    private CancellationToken CancellationToken
    {
        get
        {
            return TestContext?.CancellationTokenSource?.Token ?? default;
        }
    }

    public TestContext? TestContext
    {
        get;
        set;
    }
}
