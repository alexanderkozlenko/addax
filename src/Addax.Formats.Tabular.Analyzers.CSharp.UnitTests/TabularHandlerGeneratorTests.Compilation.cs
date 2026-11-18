using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Addax.Formats.Tabular.Analyzers.CSharp.UnitTests;

public sealed partial class TabularHandlerGeneratorTests
{
    private static readonly TabularDialect s_dialect = new("\n", ',', '"', '"', '#');
    private static readonly CSharpCompilation s_compilation = CreateCompilation();
    private static readonly ConcurrentDictionary<string, Lazy<Task<AssetResult>>> s_assetResults = new(StringComparer.Ordinal);

    private static CSharpGeneratorDriver CreateTrackingGeneratorDriver()
    {
        return CSharpGeneratorDriver.Create(
            [new TabularHandlerGenerator().AsSourceGenerator()],
            driverOptions: new(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));
    }

    private static IncrementalStepRunReason GetTrackedStepReason(GeneratorDriver generatorDriver, string stepName)
    {
        return generatorDriver.GetRunResult().Results.Single().TrackedSteps[stepName].Single().Outputs.Single().Reason;
    }

    private static IncrementalStepRunReason GetTrackedOutputStepReason(GeneratorDriver generatorDriver, string stepName)
    {
        return generatorDriver.GetRunResult().Results.Single().TrackedOutputSteps[stepName].Single().Outputs.Single().Reason;
    }

    private static Func<string, string> CreateRoundTripMethod(Assembly assembly, string recordTypeName)
    {
        var recordType = assembly.GetType(recordTypeName, throwOnError: true)!;

        return typeof(TabularHandlerGeneratorTests)
            .GetMethod(nameof(RoundTrip), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(recordType)
            .CreateDelegate<Func<string, string>>();
    }

    private static Func<string, Task<string>> CreateRoundTripAsyncMethod(Assembly assembly, string recordTypeName)
    {
        var recordType = assembly.GetType(recordTypeName, throwOnError: true)!;

        return typeof(TabularHandlerGeneratorTests)
            .GetMethod(nameof(RoundTripAsync), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(recordType)
            .CreateDelegate<Func<string, Task<string>>>();
    }

    private static string RoundTrip<T>(string content)
        where T : notnull
    {
        var records = new List<T>();

        using (var reader = new TabularReader<T>(new MemoryStream(Encoding.UTF8.GetBytes(content)), s_dialect))
        {
            while (reader.TryReadRecord())
            {
                records.Add(reader.CurrentRecord);
            }
        }

        using var stream = new MemoryStream();

        using (var writer = new TabularWriter<T>(stream, s_dialect))
        {
            foreach (var record in records)
            {
                writer.WriteRecord(record);
            }
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static async Task<string> RoundTripAsync<T>(string content)
        where T : notnull
    {
        var records = new List<T>();

        await using (var reader = new TabularReader<T>(new MemoryStream(Encoding.UTF8.GetBytes(content)), s_dialect))
        {
            while (await reader.TryReadRecordAsync())
            {
                records.Add(reader.CurrentRecord);
            }
        }

        using var stream = new MemoryStream();

        await using (var writer = new TabularWriter<T>(stream, s_dialect))
        {
            foreach (var record in records)
            {
                await writer.WriteRecordAsync(record);
            }
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private static Task<AssetResult> GetAssetResultAsync(string assetName)
    {
        return s_assetResults.GetOrAdd(assetName, static x => new(() => CreateAssetResultAsync(x))).Value;
    }

    private static async Task<AssetResult> CreateAssetResultAsync(string assetName)
    {
        var assetTree = await ParseAssetAsync(assetName);

        var compilation = s_compilation
            .WithAssemblyName(assetName)
            .AddSyntaxTrees(assetTree);

        if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets", $"{assetName}.Library.cs")))
        {
            var assetLibraryTree = await ParseAssetAsync($"{assetName}.Library");

            var libraryCompilation = CSharpCompilation.Create(
                "MyLibrary",
                [assetLibraryTree],
                s_compilation.References,
                s_compilation.Options);

            Assert.IsEmpty(libraryCompilation.GetDiagnostics());

            compilation = compilation.AddReferences(libraryCompilation.ToMetadataReference());
        }

        var generatorDriver = CSharpGeneratorDriver
            .Create(new TabularHandlerGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

        var semanticModel = outputCompilation.GetSemanticModel(assetTree);

        var records = assetTree.GetRoot()
            .DescendantNodes()
            .OfType<BaseTypeDeclarationSyntax>()
            .Select(x => semanticModel.GetDeclaredSymbol(x)!)
            .Where(static x => x.GetAttributes().Any(static x => x.AttributeClass?.ToDisplayString() == "Addax.Formats.Tabular.TabularRecordAttribute"))
            .ToArray();

        using var assemblyStream = new MemoryStream();

        var emitResult = outputCompilation.Emit(assemblyStream);
        var assembly = emitResult.Success ? Assembly.Load(assemblyStream.ToArray()) : null;

        return new(
            outputCompilation,
            outputCompilation.GetDiagnostics(),
            assembly,
            generatorDriver.GetRunResult().GeneratedTrees,
            generatorDiagnostics.Select(x => (GetDiagnosticRecord(semanticModel, x, records), x)).ToArray(),
            records);
    }

    private static INamedTypeSymbol GetDiagnosticRecord(SemanticModel semanticModel, Diagnostic diagnostic, IReadOnlyList<INamedTypeSymbol> records)
    {
        Assert.AreEqual(semanticModel.SyntaxTree, diagnostic.Location.SourceTree);

        var declarationSyntax = semanticModel.SyntaxTree.GetRoot()
            .FindNode(diagnostic.Location.SourceSpan)
            .AncestorsAndSelf()
            .OfType<BaseTypeDeclarationSyntax>()
            .First();

        var declaringType = semanticModel.GetDeclaredSymbol(declarationSyntax)!;
        var diagnosticRecords = new List<INamedTypeSymbol>();

        foreach (var record in records)
        {
            for (var currentType = record; currentType is not null; currentType = currentType.BaseType)
            {
                if (SymbolEqualityComparer.Default.Equals(currentType.OriginalDefinition, declaringType))
                {
                    diagnosticRecords.Add(record);

                    break;
                }
            }
        }

        Assert.HasCount(1, diagnosticRecords);

        return diagnosticRecords[0];
    }

    private static async Task<SyntaxTree> ParseAssetAsync(string assetName)
    {
        var assetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets", $"{assetName}.cs");
        var assetText = await File.ReadAllTextAsync(assetPath);

        return CSharpSyntaxTree.ParseText(SourceText.From(assetText, Encoding.UTF8), path: assetPath);
    }

    private static async Task<(string Content, string? ExpectedContent)> GetRoundTripAsync(string roundTripName)
    {
        var assetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Assets", "RoundTrips.json");

        using var assetDocument = JsonDocument.Parse(await File.ReadAllTextAsync(assetPath));

        var roundTrip = assetDocument.RootElement.GetProperty(roundTripName);

        var content = string.Join("\n", roundTrip.GetProperty("content").EnumerateArray().Select(static x => x.GetString()));
        var expectedContent = roundTrip.TryGetProperty("expected", out var expected) ? string.Join("\n", expected.EnumerateArray().Select(static x => x.GetString())) : null;

        return (content, expectedContent);
    }

    private static CSharpCompilation CreateCompilation()
    {
        var runtimeLocation = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        return CSharpCompilation.Create(
            "MyAssembly",
            [],
            [
                MetadataReference.CreateFromFile(GetLibraryPath("System.Linq.Expressions")),
                MetadataReference.CreateFromFile(GetLibraryPath("System.Memory")),
                MetadataReference.CreateFromFile(GetLibraryPath("System.Private.CoreLib")),
                MetadataReference.CreateFromFile(GetLibraryPath("System.Private.Uri")),
                MetadataReference.CreateFromFile(GetLibraryPath("System.Runtime")),
                MetadataReference.CreateFromFile(GetLibraryPath("System.Runtime.Numerics")),
                MetadataReference.CreateFromFile(typeof(TabularContentException).Assembly.Location),
            ],
            new(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                deterministic: true,
                nullableContextOptions: NullableContextOptions.Enable));

        string GetLibraryPath(string libraryName)
        {
            return Path.Combine(runtimeLocation, $"{libraryName}.dll");
        }
    }

    [GeneratedRegex("'(?<argument>[^']+)'[^:']*(?:: (?<issue>.+))?$")]
    private static partial Regex DiagnosticArgumentRegex
    {
        get;
    }
}
