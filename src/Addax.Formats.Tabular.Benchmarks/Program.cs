using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Exporters;
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
        .WithToolchain(InProcessEmitToolchain.Instance)
        .WithStrategy(RunStrategy.Throughput))
    .AddDiagnoser(MemoryDiagnoser.Default)
    .AddColumnProvider(DefaultColumnProviders.Instance)
    .AddColumn(StatisticColumn.OperationsPerSecond, StatisticColumn.Median, StatisticColumn.Min, StatisticColumn.Max)
    .WithSummaryStyle(SummaryStyle.Default
        .WithSizeUnit(SizeUnit.B)
        .WithTimeUnit(TimeUnit.Millisecond))
    .AddLogger(ConsoleLogger.Default)
    .AddExporter(MarkdownExporter.GitHub);

BenchmarkSwitcher.FromAssembly(Assembly.GetEntryAssembly()!).Run(args, config);
