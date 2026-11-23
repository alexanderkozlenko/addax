using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;
using Perfolizer.Metrology;

var assembly = Assembly.GetEntryAssembly()!;

var artifactsPath = assembly
    .GetCustomAttributes<AssemblyMetadataAttribute>()
    .Single(static x => x.Key == "ArtifactsPath").Value!;

var config = DefaultConfig.Instance
    .WithArtifactsPath(Path.Combine(artifactsPath, "benchmarks"))
    .AddDiagnoser(MemoryDiagnoser.Default)
    .AddColumn(StatisticColumn.OperationsPerSecond, StatisticColumn.Median, StatisticColumn.Min, StatisticColumn.Max)
    .WithSummaryStyle(SummaryStyle.Default
        .WithSizeUnit(SizeUnit.B)
        .WithTimeUnit(TimeUnit.Nanosecond))
    .AddExporter(JsonExporter.Default);

if (!args.Contains("--inProcess", StringComparer.OrdinalIgnoreCase))
{
    args = [.. args, "--inProcess"];
}

BenchmarkSwitcher.FromAssembly(assembly).Run(args, config);
