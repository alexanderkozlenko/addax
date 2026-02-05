using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;
using Perfolizer.Metrology;

var config = ManualConfig
    .CreateEmpty()
    .AddJob(Job.Default
        .WithLaunchCount(1)
        .WithWarmupCount(1)
        .WithIterationCount(3)
        .WithToolchain(InProcessEmitToolchain.Instance)
        .WithStrategy(RunStrategy.Throughput))
    .AddDiagnoser(MemoryDiagnoser.Default)
    .AddColumnProvider(DefaultColumnProviders.Instance)
    .AddColumn(StatisticColumn.OperationsPerSecond, StatisticColumn.Median, StatisticColumn.Min, StatisticColumn.Max)
    .WithSummaryStyle(SummaryStyle.Default
        .WithSizeUnit(SizeUnit.B)
        .WithTimeUnit(TimeUnit.Nanosecond))
    .AddLogger(ConsoleLogger.Default)
    .AddExporter(MarkdownExporter.GitHub)
    .AddExporter(JsonExporter.Default);

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args, config);
