using System.Globalization;
using Addax.Formats.Tabular.Converters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Addax.Formats.Tabular.Analyzers.CSharp.UnitTests;

[TestClass]
public sealed partial class TabularHandlerGeneratorTests
{
    [TestMethod]
    [DataRow("AssemblyConverters", "MyNamespace.AssemblyConvertersRecord", "Read, Write")]
    [DataRow("AssemblyConverters", "MyNamespace.BuiltInTypeAssemblyConverterRecord", "Read, Write")]
    [DataRow("AssemblyConverters", "MyNamespace.DynamicAssemblyConvertersRecord", "Read, Write")]
    [DataRow("AssemblyConverters", "MyNamespace.MemberConvertersRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.AbstractClassRecord", "Write")]
    [DataRow("Construction", "MyNamespace.ClassPrimaryConstructorWithRequiredMemberRecord", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithOptionalConstructorRecord", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithParameterlessAndVarargConstructorsRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.ClassWithPrimaryConstructorRecord", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithPrivateConstructorRecord", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithSeveralConstructorsRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.ClassWithTwoOptionalConstructorsRecord", "Write")]
    [DataRow("Construction", "MyNamespace.ClassWithVarargConstructorRecord", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalStructWithExtraGetOnlyMemberRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalStructWithExtraSettableMemberRecord", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithCaseDistinctParametersRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithConverterAndKeywordsRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithExtraConstructorRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithExtraSettableMemberRecord", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithInParameterRecord", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithOptionalParameterRecord", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithParamsParameterRecord", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithRequiredMemberRecord", "Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithReversedOrdersRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.PositionalWithUnmappedParameterRecord", "Write")]
    [DataRow("Construction", "MyNamespace.ReadOnlyPositionalStructRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.ReadOnlyStructWithConstructorRecord", "Write")]
    [DataRow("Construction", "MyNamespace.ReadOnlyStructWithPrimaryConstructorRecord", "Write")]
    [DataRow("Construction", "MyNamespace.RequiredMappedMemberInBaseRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.RequiredMappedMemberRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.RequiredUnmappedMemberInBaseRecord", "Write")]
    [DataRow("Construction", "MyNamespace.RequiredUnmappedMemberRecord", "Write")]
    [DataRow("Construction", "MyNamespace.RequiredUnmappedMemberWithSetsRequiredMembersRecord", "Write")]
    [DataRow("Construction", "MyNamespace.StructWithParameterizedConstructorRecord", "Write")]
    [DataRow("Construction", "MyNamespace.StructWithParameterlessAndParameterizedConstructorsRecord", "Read, Write")]
    [DataRow("Construction", "MyNamespace.StructWithVarargConstructorRecord", "Write")]
    [DataRow("Converters", "MyNamespace.BuiltInArrayConvertersRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.CustomConverterRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.DefaultAndOptionalConstructorsRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.FileLocalConverterRecord", "TAB0004 MyNamespace.FileLocalConverterRecord.MyValue0: 'MyNamespace.FileLocalConverter' is file-local")]
    [DataRow("Converters", "MyNamespace.FileLocalTypeArgumentRecord", "TAB0004 MyNamespace.FileLocalTypeArgumentRecord.MyValue0: 'MyNamespace.FileLocalTypeArgument' is file-local")]
    [DataRow("Converters", "MyNamespace.IdentityConvertibleValueTypeRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.NullableValueConvertersRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.PrivateConverterRecord", "TAB0004 MyNamespace.PrivateConverterRecord.MyValue0: 'MyNamespace.PrivateConverterRecord.PrivateConverter' is not accessible within the assembly")]
    [DataRow("Converters", "MyNamespace.RefStructTypeArgumentRecord", "Read, Write")]
    [DataRow("Converters", "MyNamespace.SetsRequiredMembersRecord", "TAB0004 MyNamespace.SetsRequiredMembersRecord.MyValue0: 'MyNamespace.SetsRequiredMembersConverter' has required members")]
    [DataRow("Converters", "MyNamespace.ValueTypeMismatchRecord", "TAB0004 MyNamespace.ValueTypeMismatchRecord.MyValue0: 'Addax.Formats.Tabular.Converters.TabularBooleanConverter' converts 'bool', but the member type is 'string?'")]
    [DataRow("Experimental", "MyNamespace.ExperimentalAccessorsRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalAssemblyConverterRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalConstructorRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalConverterTypesRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalConvertersRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalMembersRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalOuterType.NestedInExperimentalRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalOverriddenMemberRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalPrimaryConstructorRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalRecord", "Read, Write")]
    [DataRow("Experimental", "MyNamespace.ExperimentalValueTypeRecord", "Read, Write")]
    [DataRow("FieldMappings", "MyNamespace.DuplicateOrderRecord", "TAB0003 MyNamespace.DuplicateOrderRecord.MyValue1: the field order 0 is already used by 'MyNamespace.DuplicateOrderRecord.MyValue0'")]
    [DataRow("FieldMappings", "MyNamespace.HeaderRecord", "Read, Write, Header")]
    [DataRow("FieldMappings", "MyNamespace.HeaderWithReadOnlyFieldRecord", "Read, Write, Header")]
    [DataRow("FieldMappings", "MyNamespace.LeadingGapRecord", "Read")]
    [DataRow("FieldMappings", "MyNamespace.MultipleConvertersRecord", "TAB0003 MyNamespace.MultipleConvertersRecord.MyValue0: the member has more than one converter")]
    [DataRow("FieldMappings", "MyNamespace.NegativeOrderRecord", "TAB0003 MyNamespace.NegativeOrderRecord.MyValue0: the field order -1 is negative")]
    [DataRow("FieldMappings", "MyNamespace.NullNameRecord", "TAB0003 MyNamespace.NullNameRecord.MyValue0: the field name is null")]
    [DataRow("FieldMappings", "MyNamespace.PositionsRecord", "Read, Write")]
    [DataRow("FieldMappings", "MyNamespace.SkippedFieldsRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.ConstantMemberRecord", "TAB0002 MyNamespace.ConstantMemberRecord.MyValue0: the member is a constant")]
    [DataRow("Members", "MyNamespace.ConstructedGenericBaseRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.ExplicitInterfaceMemberRecord", "TAB0002 MyNamespace.ExplicitInterfaceMemberRecord.MyNamespace.IValueSource.MyValue0: the member is an explicit interface implementation")]
    [DataRow("Members", "MyNamespace.GetterOnlyOverrideRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.HiddenMemberRecord", "TAB0002 MyNamespace.HiddenMemberBase.MyValue0: the member is hidden by 'MyNamespace.HiddenMemberRecord.MyValue0'")]
    [DataRow("Members", "MyNamespace.InaccessibleAccessorsRecord", "Read")]
    [DataRow("Members", "MyNamespace.InaccessibleMethodHidingRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.IndexerRecord", "TAB0002 MyNamespace.IndexerRecord.this[int]: the member is an indexer")]
    [DataRow("Members", "MyNamespace.KeywordNamesRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.NeitherDirectionRecord", "None")]
    [DataRow("Members", "MyNamespace.OverrideAtOtherOrderRecord", "Read")]
    [DataRow("Members", "MyNamespace.OverrideAtSameOrderRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.OverrideChainRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.OverrideOfAbstractRecord", "Read, Write, Header")]
    [DataRow("Members", "MyNamespace.OverrideOfGenericBaseRecord", "Read, Write")]
    [DataRow("Members", "MyNamespace.PrivateMemberRecord", "TAB0002 MyNamespace.PrivateMemberRecord.MyValue0: the member is not accessible within the assembly")]
    [DataRow("Members", "MyNamespace.PrivateUnsupportedMemberRecord", "TAB0002 MyNamespace.PrivateUnsupportedMemberRecord.MyValue0: the member is not accessible within the assembly")]
    [DataRow("Members", "MyNamespace.ProtectedMemberRecord", "TAB0002 MyNamespace.ProtectedMemberRecord.MyValue0: the member is not accessible within the assembly")]
    [DataRow("Members", "MyNamespace.ReadOnlyFieldRecord", "Write")]
    [DataRow("Members", "MyNamespace.RefStructMemberRecord", "TAB0002 MyNamespace.RefStructMemberRecord.MyValue0: the member type 'System.Span<int>' is a ref struct")]
    [DataRow("Members", "MyNamespace.SetOnlyPropertyRecord", "Read")]
    [DataRow("Members", "MyNamespace.StaticMemberRecord", "TAB0002 MyNamespace.StaticMemberRecord.MyValue0: the member is static")]
    [DataRow("Members", "MyNamespace.UnmappedOverrideRecord", "Read, Write")]
    [DataRow("Records", "Addax.Formats.Tabular.Handlers.Handler", "Read, Write")]
    [DataRow("Records", "Addax.Formats.Tabular.Handlers.Registration", "Read, Write")]
    [DataRow("Records", "Handler0", "Read, Write")]
    [DataRow("Records", "MyNamespace.ClassRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.FileLocalOuterType.NestedInFileLocalRecord", "TAB0001 MyNamespace.FileLocalOuterType.NestedInFileLocalRecord: the containing type 'MyNamespace.FileLocalOuterType' is file-local")]
    [DataRow("Records", "MyNamespace.FileLocalRecord", "TAB0001 MyNamespace.FileLocalRecord: the type is file-local")]
    [DataRow("Records", "MyNamespace.GenericOuterType<T>.NestedInGenericRecord", "TAB0001 MyNamespace.GenericOuterType<T>.NestedInGenericRecord: the containing type 'MyNamespace.GenericOuterType<T>' is generic")]
    [DataRow("Records", "MyNamespace.GenericRecord<T>", "TAB0001 MyNamespace.GenericRecord<T>: the type is generic")]
    [DataRow("Records", "MyNamespace.InitOnlyRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.InternalRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.OuterType.InternalNestedRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.OuterType.PrivateNestedRecord", "TAB0001 MyNamespace.OuterType.PrivateNestedRecord: the type is not accessible within the assembly")]
    [DataRow("Records", "MyNamespace.OuterType.PrivateOuterType.NestedInPrivateRecord", "TAB0001 MyNamespace.OuterType.PrivateOuterType.NestedInPrivateRecord: the containing type 'MyNamespace.OuterType.PrivateOuterType' is not accessible within the assembly")]
    [DataRow("Records", "MyNamespace.OuterType.ProtectedInternalNestedRecord", "Read, Write")]
    [DataRow("Records", "MyNamespace.OuterType.ProtectedNestedRecord", "TAB0001 MyNamespace.OuterType.ProtectedNestedRecord: the type is not accessible within the assembly")]
    [DataRow("Records", "MyNamespace.RefStructRecord", "TAB0001 MyNamespace.RefStructRecord: the type is a ref struct")]
    [DataRow("Records", "MyNamespace.StaticRecord", "TAB0001 MyNamespace.StaticRecord: the type is a static class")]
    [DataRow("Records", "MyNamespace.StructRecord", "Read, Write")]
    [DataRow("Records", "MyOtherNamespace.ClassRecord", "Read, Write")]
    [DataRow("References", "MyNamespace.ExperimentalAssemblyRecord", "Read, Write")]
    [DataRow("References", "MyNamespace.HiddenReferencedMemberRecord", "TAB0002 MyLibrary.MyBase.MyValue0: the member is hidden by 'MyNamespace.HiddenReferencedMemberRecord.MyValue0'")]
    [DataRow("ValueTypes", "MyNamespace.BinaryFloatingPointTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.DecimalTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.IntegerTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.NullableBinaryFloatingPointTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.NullableDecimalTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.NullableIntegerTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.NullableOtherTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.NullableTextTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.NullableTimeTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.OtherTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.RegisteredTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.TextTypesRecord", "Read, Write")]
    [DataRow("ValueTypes", "MyNamespace.TimeTypesRecord", "Read, Write")]
    public async Task GenerateHandler(string assetName, string recordTypeName, string expectedOutcome)
    {
        var assetResult = await GetAssetResultAsync(assetName);

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.AreEqual(expectedOutcome, assetResult.GetRecordOutcome(recordTypeName));
    }

    [TestMethod]
    [DataRow("MyNamespace.PointerArrayMemberRecord", "TAB0002 MyNamespace.PointerArrayMemberRecord.MyValue0: the member type 'int*[]' contains the pointer type 'int*'")]
    [DataRow("MyNamespace.PointerMemberRecord", "TAB0002 MyNamespace.PointerMemberRecord.MyValue0: the member type 'int*' is a pointer type")]
    [DataRow("MyNamespace.PointerTypeArgumentRecord", "TAB0004 MyNamespace.PointerTypeArgumentRecord.MyValue0: 'int*' is a pointer type")]
    [DataRow("MyNamespace.RefStructMemberWithConverterRecord", "TAB0002 MyNamespace.RefStructMemberWithConverterRecord.MyValue0: the member type 'System.Span<int>' is a ref struct")]
    public async Task GenerateHandlerWithInvalidSource(string recordTypeName, string expectedOutcome)
    {
        var assetResult = await GetAssetResultAsync("InvalidSource");

        Assert.IsNotEmpty(assetResult.CompilationDiagnostics.Where(static x => x.Severity == DiagnosticSeverity.Error));
        Assert.AreEqual(expectedOutcome, assetResult.GetRecordOutcome(recordTypeName));
    }

    [TestMethod]
    public async Task GenerateHandlerWithInvalidAssemblyConverters()
    {
        var assetTree = await ParseAssetAsync("AssemblyConverters.Invalid");

        var compilation = s_compilation
            .AddSyntaxTrees(assetTree);

        CSharpGeneratorDriver
            .Create(new TabularHandlerGenerator())
            .RunGeneratorsAndUpdateCompilation(compilation, out _, out var generatorDiagnostics);

        var assetText = assetTree.GetText();

        Assert.HasCount(2, generatorDiagnostics);
        Assert.AreEqual("TAB0005", generatorDiagnostics[0].Id);
        Assert.AreEqual("TabularConverter<TabularBase64ArrayConverter>", assetText.ToString(generatorDiagnostics[0].Location.SourceSpan));
        Assert.AreEqual("The type 'byte[]' has more than one assembly-level converter for tabular field mapping: 'Addax.Formats.Tabular.Converters.TabularBase16ArrayConverter' and 'Addax.Formats.Tabular.Converters.TabularBase64ArrayConverter'", generatorDiagnostics[0].GetMessage(CultureInfo.InvariantCulture));
        Assert.AreEqual("TAB0004", generatorDiagnostics[1].Id);
        Assert.AreEqual("TabularConverter<MyNamespace.NullableInt32Converter>", assetText.ToString(generatorDiagnostics[1].Location.SourceSpan));
        Assert.AreEqual("The specified converter cannot be used for tabular field mapping in 'MyAssembly': 'MyNamespace.NullableInt32Converter' converts the nullable type 'int?'", generatorDiagnostics[1].GetMessage(CultureInfo.InvariantCulture));
    }

    [TestMethod]
    [DataRow("AssemblyConverters", "MyNamespace.AssemblyConvertersRecord", "0a0b^0c0d$^", "0a0b^0c0d$^$")]
    [DataRow("AssemblyConverters", "MyNamespace.BuiltInTypeAssemblyConverterRecord", "20200120^20210121$20220122^", "20200120^20210121$20220122^$")]
    [DataRow("AssemblyConverters", "MyNamespace.DynamicAssemblyConvertersRecord", "a^b|c$d^e", "a^b|c$d^e$")]
    [DataRow("AssemblyConverters", "MyNamespace.MemberConvertersRecord", "2020-01-20^AQI=", "2020-01-20^AQI=$")]
    [DataRow("Construction", "MyNamespace.PositionalWithReversedOrdersRecord", "x^5", "x^5$")]
    [DataRow("Construction", "MyNamespace.RequiredMappedMemberRecord", "5", "5$")]
    [DataRow("Converters", "MyNamespace.BuiltInArrayConvertersRecord", "0a0b^AQI=", "0a0b^AQI=$")]
    [DataRow("Converters", "MyNamespace.CustomConverterRecord", "XyZ^7$q^", "XYZ^7$Q^$")]
    [DataRow("FieldMappings", "MyNamespace.HeaderRecord", "!note$h0^h1$1^2", "'a^''b'''^$1^2$")]
    [DataRow("FieldMappings", "MyNamespace.HeaderRecord", "h0^h1$1^2", "'a^''b'''^$1^2$")]
    [DataRow("FieldMappings", "MyNamespace.HeaderWithReadOnlyFieldRecord", "h0^h1^h2$1^x^2", "^^my-value$1$")]
    [DataRow("FieldMappings", "MyNamespace.PositionsRecord", "1^x^2.5$!note$5^z^6.5", "1^x^2.5$5^z^6.5$")]
    [DataRow("FieldMappings", "MyNamespace.SkippedFieldsRecord", "a^1^b^c^4", "-^1^-^-^4$")]
    [DataRow("Members", "MyNamespace.KeywordNamesRecord", "1^a^2^3^4.5", "1^a^2^3^4.5$")]
    [DataRow("Members", "MyNamespace.OverrideChainRecord", "5", "5$")]
    [DataRow("ValueTypes", "MyNamespace.BinaryFloatingPointTypesRecord", "1.5^2.5^3.5^4.5", "1.5^2.5^3.5^4.5$")]
    [DataRow("ValueTypes", "MyNamespace.DecimalTypesRecord", "1.5^2.5^3.5^4.5", "1.5^2.5^3.5^4.5$")]
    [DataRow("ValueTypes", "MyNamespace.IntegerTypesRecord", "-1^2^-3^4^-5^6^-7^8^-9^10^-11", "-1^2^-3^4^-5^6^-7^8^-9^10^-11$")]
    [DataRow("ValueTypes", "MyNamespace.NullableBinaryFloatingPointTypesRecord", "1.5^2.5^3.5^4.5", "1.5^2.5^3.5^4.5$")]
    [DataRow("ValueTypes", "MyNamespace.NullableBinaryFloatingPointTypesRecord", "^^^", "^^^$")]
    [DataRow("ValueTypes", "MyNamespace.NullableDecimalTypesRecord", "1.5^2.5^3.5^4.5", "1.5^2.5^3.5^4.5$")]
    [DataRow("ValueTypes", "MyNamespace.NullableDecimalTypesRecord", "^^^", "^^^$")]
    [DataRow("ValueTypes", "MyNamespace.NullableIntegerTypesRecord", "-1^2^-3^4^-5^6^-7^8^-9^10^-11", "-1^2^-3^4^-5^6^-7^8^-9^10^-11$")]
    [DataRow("ValueTypes", "MyNamespace.NullableIntegerTypesRecord", "^^^^^^^^^^", "^^^^^^^^^^$")]
    [DataRow("ValueTypes", "MyNamespace.NullableOtherTypesRecord", "^^", "^^$")]
    [DataRow("ValueTypes", "MyNamespace.NullableOtherTypesRecord", "true^00000000-0000-0000-0000-000000000002^https://example.com/3", "true^00000000-0000-0000-0000-000000000002^https://example.com/3$")]
    [DataRow("ValueTypes", "MyNamespace.NullableTextTypesRecord", "^^", "^^$")]
    [DataRow("ValueTypes", "MyNamespace.NullableTextTypesRecord", "a^b^c", "a^b^c$")]
    [DataRow("ValueTypes", "MyNamespace.NullableTimeTypesRecord", "P0DT1H0M0.0000000S^02:00:00.0000000^2020-01-03^2020-01-04T00:00:00.0000000Z^2020-01-05T00:00:00.0000000+00:00", "P0DT1H0M0.0000000S^02:00:00.0000000^2020-01-03^2020-01-04T00:00:00.0000000Z^2020-01-05T00:00:00.0000000+00:00$")]
    [DataRow("ValueTypes", "MyNamespace.NullableTimeTypesRecord", "^^^^", "^^^^$")]
    [DataRow("ValueTypes", "MyNamespace.OtherTypesRecord", "true^00000000-0000-0000-0000-000000000002^https://example.com/3", "true^00000000-0000-0000-0000-000000000002^https://example.com/3$")]
    [DataRow("ValueTypes", "MyNamespace.TextTypesRecord", "a^b^c", "a^b^c$")]
    [DataRow("ValueTypes", "MyNamespace.TimeTypesRecord", "P0DT1H0M0.0000000S^02:00:00.0000000^2020-01-03^2020-01-04T00:00:00.0000000Z^2020-01-05T00:00:00.0000000+00:00", "P0DT1H0M0.0000000S^02:00:00.0000000^2020-01-03^2020-01-04T00:00:00.0000000Z^2020-01-05T00:00:00.0000000+00:00$")]
    public async Task RunHandler(string assetName, string recordTypeName, string content, string expectedContent)
    {
        var assetResult = await GetAssetResultAsync(assetName);

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.IsNotNull(assetResult.Assembly);

        var roundTrip = CreateRoundTripMethod(assetResult.Assembly, recordTypeName);
        var roundTripAsync = CreateRoundTripAsyncMethod(assetResult.Assembly, recordTypeName);

        Assert.AreEqual(expectedContent, roundTrip(content, null));
        Assert.AreEqual(expectedContent, await roundTripAsync(content, null));
    }

    [TestMethod]
    public async Task RunHandlerWithRegisteredConverters()
    {
        var assetResult = await GetAssetResultAsync("ValueTypes");

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.IsNotNull(assetResult.Assembly);

        var content = "0a0b^0c0d^AQI=$0e0f^^";
        var expectedContent = "0a0b^0c0d^AQI=$0e0f^^$";
        var roundTrip = CreateRoundTripMethod(assetResult.Assembly, "MyNamespace.RegisteredTypesRecord");
        var roundTripAsync = CreateRoundTripAsyncMethod(assetResult.Assembly, "MyNamespace.RegisteredTypesRecord");
        var options = new TabularOptions();

        options.Converters[typeof(Memory<byte>)] = new TabularBase16MemoryConverter();
        options.Converters[typeof(byte[])] = new TabularBase16ArrayConverter();
        options.Converters[typeof(ReadOnlyMemory<byte>)] = new TabularBase64ReadOnlyMemoryConverter();

        Assert.AreEqual(expectedContent, roundTrip(content, options));
        Assert.AreEqual(expectedContent, await roundTripAsync(content, options));
    }

    [TestMethod]
    public async Task RunHandlerWithoutRegisteredConverters()
    {
        var assetResult = await GetAssetResultAsync("ValueTypes");

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.IsNotNull(assetResult.Assembly);

        var content = "0a0b^0c0d^AQI=$0e0f^^";
        var roundTrip = CreateRoundTripMethod(assetResult.Assembly, "MyNamespace.RegisteredTypesRecord");
        var roundTripAsync = CreateRoundTripAsyncMethod(assetResult.Assembly, "MyNamespace.RegisteredTypesRecord");

        Assert.ThrowsExactly<InvalidOperationException>(() => roundTrip(content, null));

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => roundTripAsync(content, null));
    }

    [TestMethod]
    [DataRow("Converters", "MyNamespace.CustomConverterRecord", "q^x", "The field 1 of the record 0 cannot be converted to 'System.Int32' for the member 'MyNamespace.CustomConverterRecord.MyValue1'.")]
    [DataRow("FieldMappings", "MyNamespace.PositionsRecord", "1^x^2.5$3^y", "The record 1 has no field 2 for the member 'MyNamespace.PositionsRecord.MyValue2'.")]
    [DataRow("FieldMappings", "MyNamespace.PositionsRecord", "^w^2.5", "The field 0 of the record 0 cannot be converted to 'System.Int32' for the member 'MyNamespace.PositionsRecord.MyValue0'.")]
    [DataRow("FieldMappings", "MyNamespace.PositionsRecord", "bad^w^2.5", "The field 0 of the record 0 cannot be converted to 'System.Int32' for the member 'MyNamespace.PositionsRecord.MyValue0'.")]
    [DataRow("FieldMappings", "MyNamespace.SkippedFieldsRecord", "x^2", "The record 0 has no field 4 for the member 'MyNamespace.SkippedFieldsRecord.MyValue4'.")]
    public async Task RunHandlerWithInvalidContent(string assetName, string recordTypeName, string content, string expectedMessage)
    {
        var assetResult = await GetAssetResultAsync(assetName);

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.IsNotNull(assetResult.Assembly);

        var roundTrip = CreateRoundTripMethod(assetResult.Assembly, recordTypeName);
        var roundTripAsync = CreateRoundTripAsyncMethod(assetResult.Assembly, recordTypeName);

        var exception = Assert.ThrowsExactly<TabularException>(() => roundTrip(content, null));
        var exceptionAsync = await Assert.ThrowsExactlyAsync<TabularException>(() => roundTripAsync(content, null));

        Assert.AreEqual(expectedMessage, exception.Message);
        Assert.AreEqual(expectedMessage, exceptionAsync.Message);
    }

    [TestMethod]
    [DataRow("Construction", "MyNamespace.ClassWithPrimaryConstructorRecord", "5^6")]
    [DataRow("FieldMappings", "MyNamespace.LeadingGapRecord", "a^b")]
    [DataRow("Members", "MyNamespace.SetOnlyPropertyRecord", "5")]
    public async Task RunHandlerWithUnsupportedDirection(string assetName, string recordTypeName, string content)
    {
        var assetResult = await GetAssetResultAsync(assetName);

        Assert.IsEmpty(assetResult.CompilationDiagnostics);
        Assert.IsNotNull(assetResult.Assembly);

        var roundTrip = CreateRoundTripMethod(assetResult.Assembly, recordTypeName);
        var roundTripAsync = CreateRoundTripAsyncMethod(assetResult.Assembly, recordTypeName);

        Assert.ThrowsExactly<NotSupportedException>(() => roundTrip(content, null));

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => roundTripAsync(content, null));
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

        var otherText = otherTree.ToString().Replace("MyOtherType", "MyChangedType", StringComparison.Ordinal);

        compilation = compilation.ReplaceSyntaxTree(otherTree, otherTree.WithChangedText(SourceText.From(otherText)));
        generatorDriver = generatorDriver.RunGenerators(compilation);

        Assert.AreNotEqual(IncrementalStepRunReason.Modified, GetTrackedStepReason(generatorDriver, "RecordMappings"));
        Assert.AreEqual(IncrementalStepRunReason.Cached, GetTrackedOutputStepReason(generatorDriver, WellKnownGeneratorOutputs.ImplementationSourceOutput));

        var recordText = recordTree.ToString().Replace("\"my-value\"", "\"my-other-value\"", StringComparison.Ordinal);

        compilation = compilation.ReplaceSyntaxTree(recordTree, recordTree.WithChangedText(SourceText.From(recordText)));
        generatorDriver = generatorDriver.RunGenerators(compilation);

        Assert.AreEqual(IncrementalStepRunReason.Modified, GetTrackedStepReason(generatorDriver, "RecordMappings"));
        Assert.AreEqual(IncrementalStepRunReason.Modified, GetTrackedOutputStepReason(generatorDriver, WellKnownGeneratorOutputs.ImplementationSourceOutput));
    }

    [TestMethod]
    public async Task RunIncrementalConverters()
    {
        var converterTree = await ParseAssetAsync("Pipeline.Converter");
        var otherTree = await ParseAssetAsync("Pipeline.Other");

        var compilation = s_compilation
            .AddSyntaxTrees(await ParseAssetAsync("Pipeline.Record"), converterTree, otherTree);

        var generatorDriver = CreateTrackingGeneratorDriver()
            .RunGenerators(compilation);

        var otherText = otherTree.ToString().Replace("MyOtherType", "MyChangedType", StringComparison.Ordinal);

        compilation = compilation.ReplaceSyntaxTree(otherTree, otherTree.WithChangedText(SourceText.From(otherText)));
        generatorDriver = generatorDriver.RunGenerators(compilation);

        Assert.AreNotEqual(IncrementalStepRunReason.Modified, GetTrackedStepReason(generatorDriver, "RecordMappings"));
        Assert.AreEqual(IncrementalStepRunReason.Cached, GetTrackedOutputStepReason(generatorDriver, WellKnownGeneratorOutputs.ImplementationSourceOutput));

        var converterText = converterTree.ToString().Replace("TabularConverter<MyNamespace.MyConverter>", "TabularConverter<MyNamespace.MyOtherConverter>", StringComparison.Ordinal);

        compilation = compilation.ReplaceSyntaxTree(converterTree, converterTree.WithChangedText(SourceText.From(converterText)));
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

        var otherText = otherTree.ToString().Replace("MyOtherType", "MyChangedType", StringComparison.Ordinal);

        compilation = compilation.ReplaceSyntaxTree(otherTree, otherTree.WithChangedText(SourceText.From(otherText)));
        generatorDriver = generatorDriver.RunGenerators(compilation);

        var generatorDiagnostics = generatorDriver.GetRunResult().Diagnostics;

        Assert.AreEqual(IncrementalStepRunReason.Cached, GetTrackedOutputStepReason(generatorDriver, WellKnownGeneratorOutputs.ImplementationSourceOutput));
        Assert.HasCount(1, generatorDiagnostics);
        Assert.AreEqual("TAB0002", generatorDiagnostics.Single().Id);
    }
}
