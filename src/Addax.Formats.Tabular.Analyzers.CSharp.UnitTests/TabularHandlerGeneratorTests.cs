using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Addax.Formats.Tabular.Analyzers.CSharp.UnitTests;

[TestClass]
public sealed partial class TabularHandlerGeneratorTests
{
    [TestMethod]
    [DataRow("Construction", "MyNamespace.AbstractClass", "Write")]
    [DataRow("Construction", "MyNamespace.ClassPrimaryConstructorWithRequiredMember", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithOptionalConstructor", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithParameterlessAndVarargConstructors", "Read, Write")]
    [DataRow("Construction", "MyNamespace.ClassWithPrimaryConstructor", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithPrivateConstructor", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithSeveralConstructors", "Read, Write")]
    [DataRow("Construction", "MyNamespace.ClassWithTwoOptionalConstructors", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithVarargConstructor", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithCaseDistinctParameters", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithConverterAndKeywords", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithExtraConstructor", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithExtraSettableMember", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithInParameter", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithOptionalParameter", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithParamsParameter", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithRequiredMember", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithReversedOrders", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithUnmappedParameter", "Write")]
    [DataRow("Construction", "MyNamespace.ReadOnlyRecordStruct", "Read, Write")]
    [DataRow("Construction", "MyNamespace.ReadOnlyStructWithConstructor", "Write")]
    [DataRow("Construction", "MyNamespace.ReadOnlyStructWithPrimaryConstructor", "Write")]
    [DataRow("Construction", "MyNamespace.RecordStructWithExtraGetOnlyMember", "Read, Write")]
    [DataRow("Construction", "MyNamespace.RecordStructWithExtraSettableMember", "Write")]
    [DataRow("Construction", "MyNamespace.RequiredMappedMember", "Read, Write")]
    [DataRow("Construction", "MyNamespace.RequiredMappedMemberInBase", "Read, Write")]
    [DataRow("Construction", "MyNamespace.RequiredUnmappedMember", "Write")]
    [DataRow("Construction", "MyNamespace.RequiredUnmappedMemberInBase", "Write")]
    [DataRow("Construction", "MyNamespace.RequiredUnmappedMemberWithSetsRequiredMembers", "Write")]
    [DataRow("Construction", "MyNamespace.StructWithParameterizedConstructor", "Write")]
    [DataRow("Construction", "MyNamespace.StructWithParameterlessAndParameterizedConstructors", "Read, Write")]
    [DataRow("Construction", "MyNamespace.StructWithVarargConstructor", "Write")]
    [DataRow("Converters", "MyNamespace.AbstractConverterRecord", "TAB0004 MyNamespace.AbstractConverterRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.AmbiguousConstructorsRecord", "TAB0004 MyNamespace.AmbiguousConstructorsRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.ArrayConverterRecord", "TAB0004 MyNamespace.ArrayConverterRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.BuiltInArrayConvertersRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.CustomConverterRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.DefaultAndOptionalConstructorsRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.DynamicTypeArgumentRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.FileLocalConverterRecord", "TAB0004 MyNamespace.FileLocalConverterRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.FileLocalTypeArgumentRecord", "TAB0004 MyNamespace.FileLocalTypeArgumentRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.IdentityConvertibleValueTypeRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.InaccessibleConstructorRecord", "TAB0004 MyNamespace.InaccessibleConstructorRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.NotConverterRecord", "TAB0004 MyNamespace.NotConverterRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.NullableValueConvertersRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.OptionalConstructorRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.PointerTypeArgumentRecord", "TAB0004 MyNamespace.PointerTypeArgumentRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.RefStructTypeArgumentRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.RequiredMembersRecord", "TAB0004 MyNamespace.RequiredMembersRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.SetsRequiredMembersRecord", "TAB0004 MyNamespace.SetsRequiredMembersRecord.MyValue0")]
    [DataRow("Converters", "MyNamespace.ValueTypeMismatchRecord", "TAB0004 MyNamespace.ValueTypeMismatchRecord.MyValue0")]
    [DataRow("Experimental", "MyNamespace.ExperimentalAccessorsRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalConstructorRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalConverterTypesRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalConvertersRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalMembersRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalOuterType.NestedInExperimentalRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalOverriddenMemberRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalPrimaryConstructorRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalValueTypeRecord", "Read, Write")]
    [DataRow("FieldMappings", "MyNamespace.DuplicateOrderRecord", "TAB0005 MyNamespace.DuplicateOrderRecord.MyValue1")]
    [DataRow("FieldMappings", "MyNamespace.HeaderRecord", "Read, Write, Header")]
    [DataRow("FieldMappings", "MyNamespace.LeadingGapRecord", "Read")]
    [DataRow("FieldMappings", "MyNamespace.NegativeOrderRecord", "TAB0005 MyNamespace.NegativeOrderRecord.MyValue0")]
    [DataRow("FieldMappings", "MyNamespace.NullNameRecord", "TAB0005 MyNamespace.NullNameRecord.MyValue0")]
    [DataRow("FieldMappings", "MyNamespace.PositionsRecord", "Read, Write")]
    [DataRow("FieldMappings", "MyNamespace.SkippedFieldsRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.ConstantMemberRecord", "TAB0002 MyNamespace.ConstantMemberRecord.MyValue0")]
    [DataRow("Members", "MyNamespace.ConstructedGenericBaseRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.ExplicitInterfaceMemberRecord", "TAB0002 MyNamespace.ExplicitInterfaceMemberRecord.MyNamespace.IValueSource.MyValue0")]
    [DataRow("Members", "MyNamespace.GetterOnlyOverrideRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.HiddenMemberRecord", "TAB0002 MyNamespace.HiddenMemberBase.MyValue0")]
    [DataRow("Members", "MyNamespace.InaccessibleAccessorsRecord", "Read")]
    [DataRow("Members", "MyNamespace.InaccessibleMethodHidingRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.IndexerRecord", "TAB0002 MyNamespace.IndexerRecord.this[int]")]
    [DataRow("Members", "MyNamespace.KeywordNamesRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.NeitherDirectionRecord", "None")]
    [DataRow("Members", "MyNamespace.OverrideAtOtherOrderRecord", "Read")]
    [DataRow("Members", "MyNamespace.OverrideAtSameOrderRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.OverrideChainRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.OverrideOfAbstractRecord", "Read, Write, Header")]
    [DataRow("Members", "MyNamespace.OverrideOfGenericBaseRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.PrivateMemberRecord", "TAB0002 MyNamespace.PrivateMemberRecord.MyValue0")]
    [DataRow("Members", "MyNamespace.PrivateUnsupportedMemberRecord", "TAB0002 MyNamespace.PrivateUnsupportedMemberRecord.MyValue0")]
    [DataRow("Members", "MyNamespace.ProtectedMemberRecord", "TAB0002 MyNamespace.ProtectedMemberRecord.MyValue0")]
    [DataRow("Members", "MyNamespace.ReadOnlyFieldRecord", "Write")]
    [DataRow("Members", "MyNamespace.RefStructMemberRecord", "TAB0002 MyNamespace.RefStructMemberRecord.MyValue0")]
    [DataRow("Members", "MyNamespace.SetOnlyPropertyRecord", "Read")]
    [DataRow("Members", "MyNamespace.StaticMemberRecord", "TAB0002 MyNamespace.StaticMemberRecord.MyValue0")]
    [DataRow("Members", "MyNamespace.UnmappedOverrideRecord", "Read, Write")]
    [DataRow("Records", "Addax.Formats.Tabular.Handlers.Handler", "Read, Write")]
    [DataRow("Records", "Addax.Formats.Tabular.Handlers.Registration", "Read, Write")]
    [DataRow("Records", "Handler0", "Read, Write")]
    [DataRow("Records", "MyNamespace.ClassRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.FileLocalOuterType.NestedInFileLocalRecord", "TAB0001")]
    [DataRow("Records", "MyNamespace.FileLocalRecord", "TAB0001")]
    [DataRow("Records", "MyNamespace.GenericOuterType<T>.NestedInGenericRecord", "TAB0001")]
    [DataRow("Records", "MyNamespace.GenericRecord<T>", "TAB0001")]
    [DataRow("Records", "MyNamespace.InitOnlyRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.InternalRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.OuterType.InternalNestedRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.OuterType.PrivateNestedRecord", "TAB0001")]
    [DataRow("Records", "MyNamespace.OuterType.ProtectedInternalNestedRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.OuterType.ProtectedNestedRecord", "TAB0001")]
    [DataRow("Records", "MyNamespace.RefStructRecord", "TAB0001")]
    [DataRow("Records", "MyNamespace.StaticRecord", "TAB0001")]
    [DataRow("Records", "MyNamespace.StructRecord", "Read, Write")]
    [DataRow("Records", "MyOtherNamespace.ClassRecord", "Read, Write")]
    [DataRow("References", "MyNamespace.ExperimentalAssemblyRecord", "Read, Write")]
    [DataRow("References", "MyNamespace.HiddenReferencedMemberRecord", "TAB0002 MyLibrary.MyBase.MyValue0")]
    [DataRow("ValueTypes", "MyNamespace.SupportedNullableTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.SupportedTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.UnsupportedNullableTypeRecord", "TAB0003 MyNamespace.UnsupportedNullableTypeRecord.MyValue0")]
    [DataRow("ValueTypes", "MyNamespace.UnsupportedTypeRecord", "TAB0003 MyNamespace.UnsupportedTypeRecord.MyValue0")]
    public async Task GenerateHandler(string assetName, string recordTypeName, string expectedOutcome)
    {
        var assetResult = await GetAssetResultAsync(assetName);

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.AreEqual(expectedOutcome, assetResult.GetRecordOutcome(recordTypeName));
    }

    [TestMethod]
    [DataRow("MyNamespace.NonConstantOrderRecord", "TAB0005 MyNamespace.NonConstantOrderRecord.MyValue0")]
    [DataRow("MyNamespace.RefStructMemberWithConverterRecord", "TAB0002 MyNamespace.RefStructMemberWithConverterRecord.MyValue0")]
    public async Task GenerateHandlerWithInvalidSource(string recordTypeName, string expectedOutcome)
    {
        var assetResult = await GetAssetResultAsync("InvalidSource");

        Assert.IsNotEmpty(assetResult.CompilationDiagnostics.Where(static x => x.Severity == DiagnosticSeverity.Error));
        Assert.AreEqual(expectedOutcome, assetResult.GetRecordOutcome(recordTypeName));
    }

    [TestMethod]
    [DataRow("Construction", "MyNamespace.PositionalRecordWithReversedOrders", "PositionalConstructor")]
    [DataRow("Construction", "MyNamespace.RequiredMappedMember", "RequiredMember")]
    [DataRow("Converters", "MyNamespace.BuiltInArrayConvertersRecord", "BuiltInConverters")]
    [DataRow("Converters", "MyNamespace.CustomConverterRecord", "CustomConverter")]
    [DataRow("FieldMappings", "MyNamespace.HeaderRecord", "Header")]
    [DataRow("FieldMappings", "MyNamespace.PositionsRecord", "Positions")]
    [DataRow("FieldMappings", "MyNamespace.SkippedFieldsRecord", "SkippedFields")]
    [DataRow("Members", "MyNamespace.KeywordNamesRecord", "KeywordNames")]
    [DataRow("Members", "MyNamespace.OverrideChainRecord", "OverrideChain")]
    [DataRow("ValueTypes", "MyNamespace.SupportedNullableTypesRecord", "EmptyNullableValues")]
    [DataRow("ValueTypes", "MyNamespace.SupportedNullableTypesRecord", "NullableValues")]
    [DataRow("ValueTypes", "MyNamespace.SupportedTypesRecord", "Values")]
    public async Task RunHandler(string assetName, string recordTypeName, string roundTripName)
    {
        var assetResult = await GetAssetResultAsync(assetName);

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.IsNotNull(assetResult.Assembly);

        var (content, expectedContent) = await GetRoundTripAsync(roundTripName);
        var roundTrip = CreateRoundTripMethod(assetResult.Assembly, recordTypeName);
        var roundTripAsync = CreateRoundTripAsyncMethod(assetResult.Assembly, recordTypeName);

        Assert.AreEqual(expectedContent, roundTrip(content));
        Assert.AreEqual(expectedContent, await roundTripAsync(content));
    }

    [TestMethod]
    [DataRow("Construction", "MyNamespace.ClassWithPrimaryConstructor", "ReadUnsupported")]
    [DataRow("FieldMappings", "MyNamespace.LeadingGapRecord", "LeadingGap")]
    [DataRow("Members", "MyNamespace.SetOnlyPropertyRecord", "WriteUnsupported")]
    public async Task RunHandlerWithUnsupportedDirection(string assetName, string recordTypeName, string roundTripName)
    {
        var assetResult = await GetAssetResultAsync(assetName);

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.IsNotNull(assetResult.Assembly);

        var (content, _) = await GetRoundTripAsync(roundTripName);
        var roundTrip = CreateRoundTripMethod(assetResult.Assembly, recordTypeName);
        var roundTripAsync = CreateRoundTripAsyncMethod(assetResult.Assembly, recordTypeName);

        Assert.ThrowsExactly<NotSupportedException>(() => roundTrip(content));
        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => roundTripAsync(content));
    }

    [TestMethod]
    public async Task RunWithoutRecords()
    {
        var compilation = s_compilation
            .AddSyntaxTrees(await ParseAssetAsync("Pipeline.Other"));

        var generatorDriver = CSharpGeneratorDriver
            .Create(new TabularHandlerGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out _, out var generatorDiagnostics);

        Assert.IsEmpty(generatorDiagnostics);
        Assert.IsEmpty(generatorDriver.GetRunResult().GeneratedTrees);
    }

    [TestMethod]
    public async Task RunWithoutLibraryReference()
    {
        var compilation = s_compilation
            .RemoveReferences(s_compilation.References.Last())
            .AddSyntaxTrees(await ParseAssetAsync("Pipeline.Record"), await ParseAssetAsync("Pipeline.Attributes"));

        var generatorDriver = CSharpGeneratorDriver
            .Create(new TabularHandlerGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

        Assert.IsEmpty(generatorDiagnostics);
        Assert.IsEmpty(generatorDriver.GetRunResult().GeneratedTrees);
        Assert.IsEmpty(outputCompilation.GetDiagnostics());
    }

    [TestMethod]
    public async Task RunIncrementalSources()
    {
        var recordTree = await ParseAssetAsync("Pipeline.Record");
        var otherTree = await ParseAssetAsync("Pipeline.Other");

        var compilation = s_compilation
            .AddSyntaxTrees(recordTree, otherTree);

        var generatorDriver = CreateTrackingGeneratorDriver()
            .RunGenerators(compilation);

        compilation = compilation.ReplaceSyntaxTree(otherTree, otherTree.WithChangedText(SourceText.From(otherTree.ToString().Replace("MyOtherType", "MyChangedType", StringComparison.Ordinal))));
        generatorDriver = generatorDriver.RunGenerators(compilation);

        Assert.AreNotEqual(IncrementalStepRunReason.Modified, GetTrackedStepReason(generatorDriver, "RecordMappings"));
        Assert.AreEqual(IncrementalStepRunReason.Cached, GetTrackedOutputStepReason(generatorDriver, WellKnownGeneratorOutputs.ImplementationSourceOutput));

        compilation = compilation.ReplaceSyntaxTree(recordTree, recordTree.WithChangedText(SourceText.From(recordTree.ToString().Replace("\"my-value\"", "\"my-other-value\"", StringComparison.Ordinal))));
        generatorDriver = generatorDriver.RunGenerators(compilation);

        Assert.AreEqual(IncrementalStepRunReason.Modified, GetTrackedStepReason(generatorDriver, "RecordMappings"));
        Assert.AreEqual(IncrementalStepRunReason.Modified, GetTrackedOutputStepReason(generatorDriver, WellKnownGeneratorOutputs.ImplementationSourceOutput));
    }

    [TestMethod]
    public async Task RunIncrementalDiagnostics()
    {
        var otherTree = await ParseAssetAsync("Pipeline.Other");

        var compilation = s_compilation
            .AddSyntaxTrees(await ParseAssetAsync("Pipeline.InvalidRecord"), otherTree);

        var generatorDriver = CreateTrackingGeneratorDriver()
            .RunGenerators(compilation);

        compilation = compilation.ReplaceSyntaxTree(otherTree, otherTree.WithChangedText(SourceText.From(otherTree.ToString().Replace("MyOtherType", "MyChangedType", StringComparison.Ordinal))));
        generatorDriver = generatorDriver.RunGenerators(compilation);

        var generatorDiagnostics = generatorDriver.GetRunResult().Diagnostics;

        Assert.AreEqual(IncrementalStepRunReason.Cached, GetTrackedOutputStepReason(generatorDriver, WellKnownGeneratorOutputs.ImplementationSourceOutput));
        Assert.HasCount(1, generatorDiagnostics);
        Assert.AreEqual("TAB0002", generatorDiagnostics.Single().Id);
    }
}
