// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal sealed partial class TabularHandlerParser
    {
        private const LanguageVersion s_minimumLanguageVersion = LanguageVersion.CSharp15;

        private static readonly HashSet<string> s_supportedTypeNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "global::System.DateOnly",
            "global::System.DateTimeOffset",
            "global::System.Guid",
            "global::System.Half",
            "global::System.Int128",
            "global::System.TimeOnly",
            "global::System.TimeSpan",
            "global::System.UInt128",
            "global::System.Uri",
            "global::System.Numerics.BFloat16",
            "global::System.Numerics.BigInteger",
            "global::System.Numerics.Decimal32",
            "global::System.Numerics.Decimal64",
            "global::System.Numerics.Decimal128",
            "global::System.Text.Rune",
        };

        private readonly CSharpCompilation _compilation;
        private readonly IAssemblySymbol _featureAssembly;

        public TabularHandlerParser(CSharpCompilation compilation, IAssemblySymbol featureAssembly)
        {
            Debug.Assert(compilation != null);
            Debug.Assert(featureAssembly != null);

            _compilation = compilation;
            _featureAssembly = featureAssembly;
        }

        public (TabularRecordMapping? RecordMapping, ImmutableArray<Diagnostic> Diagnostics) GetRecordMapping(INamedTypeSymbol recordType, CancellationToken cancellationToken)
        {
            Debug.Assert(recordType != null);

            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            var recordMapping = GetRecordMapping(recordType, diagnostics, cancellationToken);

            return (recordMapping, diagnostics.ToImmutable());
        }

        private TabularRecordMapping? GetRecordMapping(INamedTypeSymbol recordType, ImmutableArray<Diagnostic>.Builder diagnostics, CancellationToken cancellationToken)
        {
            var converterBaseType = _featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularConverter`1");
            var converterAttributeType1 = _featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularConverterAttribute`1");
            var converterAttributeType2 = _featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularConverterAttribute");
            var fieldOrderAttributeType = _featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularFieldOrderAttribute");
            var fieldNameAttributeType = _featureAssembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularFieldNameAttribute");

            if ((converterBaseType == null) ||
                (converterBaseType.Arity != 1) ||
                (converterAttributeType1 == null) ||
                (converterAttributeType2 == null) ||
                (fieldOrderAttributeType == null) ||
                (fieldNameAttributeType == null))
            {
                return null;
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (_compilation.LanguageVersion < s_minimumLanguageVersion)
            {
                diagnostics.Add(Diagnostic.Create(s_diagnostic0000, recordType.Locations.FirstOrDefault()));

                return null;
            }

            if (recordType.IsStatic || recordType.IsRefLikeType || !TypeIsUsableInGeneratedCode(recordType))
            {
                diagnostics.Add(Diagnostic.Create(s_diagnostic0001, recordType.Locations.FirstOrDefault()));

                return null;
            }

            var recordTypeHasErrors = false;
            var recordMembers = GetSuitableMembers(recordType);
            var fieldMappingsBuilder = ImmutableDictionary.CreateBuilder<int, TabularFieldMapping>();
            var fieldMembers = new Dictionary<int, ISymbol>();
            var converterConstructors = new Dictionary<int, IMethodSymbol>();
            var overriddenMembers = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

            foreach (var recordMember in recordMembers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!TryGetAttribute(recordMember, fieldOrderAttributeType, out var fieldOrderAttribute))
                {
                    continue;
                }

                if (overriddenMembers.Contains(recordMember))
                {
                    continue;
                }

                if (recordMember is IPropertySymbol recordProperty)
                {
                    var overriddenProperty = recordProperty.OverriddenProperty;

                    while (overriddenProperty != null)
                    {
                        overriddenMembers.Add(overriddenProperty);
                        overriddenProperty = overriddenProperty.OverriddenProperty;
                    }
                }

                var recordMemberIsIndexer = (recordMember as IPropertySymbol)?.IsIndexer == true;

                if (recordMember.IsStatic || recordMemberIsIndexer || !_compilation.IsSymbolAccessibleWithin(recordMember, _compilation.Assembly) || TypeMemberIsHidden(recordType, recordMember))
                {
                    diagnostics.Add(CreateMemberDiagnostic(s_diagnostic0002, recordType, recordMember));
                    recordTypeHasErrors = true;

                    continue;
                }

                var valueTypeInfo = GetValueTypeInfo(recordMember);

                if (valueTypeInfo.Type.IsStatic || valueTypeInfo.Type.IsRefLikeType)
                {
                    diagnostics.Add(CreateMemberDiagnostic(s_diagnostic0002, recordType, recordMember));
                    recordTypeHasErrors = true;

                    continue;
                }

                var converterAttribute = default(AttributeData);
                var converterTypeName = default(string);
                var converterConstructor = default(IMethodSymbol);

                if (!TryGetAttribute(recordMember, converterAttributeType1, out converterAttribute) &&
                    !TryGetAttribute(recordMember, converterAttributeType2, out converterAttribute))
                {
                    if (!valueTypeInfo.IsSupported)
                    {
                        diagnostics.Add(CreateMemberDiagnostic(s_diagnostic0003, recordType, recordMember));
                        recordTypeHasErrors = true;

                        continue;
                    }
                }
                else
                {
                    if (!TryGetSupportedConverter(converterAttribute, converterBaseType, valueTypeInfo.Type, out var converterType, out converterConstructor))
                    {
                        diagnostics.Add(CreateMemberDiagnostic(s_diagnostic0004, recordType, recordMember));
                        recordTypeHasErrors = true;

                        continue;
                    }

                    converterTypeName = converterType.ToDisplayString(s_displayFormat);
                }

                if (!TryGetConstructorArgument<int>(fieldOrderAttribute, out var fieldOrder) || (fieldOrder < 0))
                {
                    diagnostics.Add(CreateMemberDiagnostic(s_diagnostic0005, recordType, recordMember));
                    recordTypeHasErrors = true;

                    continue;
                }

                if (fieldMappingsBuilder.ContainsKey(fieldOrder))
                {
                    diagnostics.Add(CreateMemberDiagnostic(s_diagnostic0005, recordType, recordMember));
                    recordTypeHasErrors = true;

                    continue;
                }

                var fieldNameLiteral = default(string);

                if (TryGetAttribute(recordMember, fieldNameAttributeType, out var fieldNameAttribute))
                {
                    if (!TryGetConstructorArgument<string>(fieldNameAttribute, out var fieldName))
                    {
                        diagnostics.Add(CreateMemberDiagnostic(s_diagnostic0005, recordType, recordMember));
                        recordTypeHasErrors = true;

                        continue;
                    }

                    fieldNameLiteral = SyntaxFactory.Literal(fieldName).Text;
                }

                var recordMemberSupportsReading = TypeMemberSupportsReading(recordMember);
                var recordMemberSupportsWriting = TypeMemberSupportsWriting(recordMember);

                if (!recordMemberSupportsReading && !recordMemberSupportsWriting)
                {
                    continue;
                }

                var fieldMapping = new TabularFieldMapping(
                    fieldOrder,
                    "@" + recordMember.Name,
                    recordMemberSupportsReading,
                    recordMemberSupportsWriting,
                    valueTypeInfo.IsNullableT,
                    valueTypeInfo.Name,
                    converterTypeName,
                    fieldNameLiteral);

                fieldMappingsBuilder[fieldOrder] = fieldMapping;
                fieldMembers[fieldOrder] = recordMember;

                if (converterConstructor != null)
                {
                    converterConstructors[fieldOrder] = converterConstructor;
                }
            }

            if (recordTypeHasErrors)
            {
                return null;
            }

            if (!TryApplyConstruction(recordType, fieldMappingsBuilder, fieldMembers, out var recordConstructor, out var constructorArguments))
            {
                DisableFieldWriting(fieldMappingsBuilder);
            }

            var experimentalDiagnosticIds = new HashSet<string>(StringComparer.Ordinal);

            AddExperimentalDiagnosticIds(recordType, experimentalDiagnosticIds);
            AddExperimentalDiagnosticIds(recordConstructor, experimentalDiagnosticIds);

            foreach (var kvp in fieldMappingsBuilder)
            {
                var fieldOrder = kvp.Key;
                var fieldMember = fieldMembers[fieldOrder];

                AddExperimentalDiagnosticIds((fieldMember is IFieldSymbol field) ? field.Type : ((IPropertySymbol)fieldMember).Type, experimentalDiagnosticIds);

                if (fieldMember is IPropertySymbol fieldProperty)
                {
                    while (fieldProperty.OverriddenProperty != null)
                    {
                        fieldProperty = fieldProperty.OverriddenProperty;
                    }

                    AddExperimentalDiagnosticIds(fieldProperty, experimentalDiagnosticIds);

                    if (kvp.Value.SupportsReading)
                    {
                        AddExperimentalDiagnosticIds(fieldProperty.GetMethod, experimentalDiagnosticIds);
                    }

                    if (kvp.Value.SupportsWriting)
                    {
                        AddExperimentalDiagnosticIds(fieldProperty.SetMethod, experimentalDiagnosticIds);
                    }
                }
                else
                {
                    AddExperimentalDiagnosticIds(fieldMember, experimentalDiagnosticIds);
                }

                if (converterConstructors.TryGetValue(fieldOrder, out var fieldConverterConstructor))
                {
                    AddExperimentalDiagnosticIds(fieldConverterConstructor.ContainingType, experimentalDiagnosticIds);
                    AddExperimentalDiagnosticIds(fieldConverterConstructor, experimentalDiagnosticIds);
                }
            }

            var recordTypeName = recordType.ToDisplayString(s_displayFormat);
            var fieldMappings = fieldMappingsBuilder.Values.OrderBy(x => x.FieldOrder).ToImmutableArray();
            var experimentalDiagnosticIdsSorted = experimentalDiagnosticIds.OrderBy(x => x, StringComparer.Ordinal).ToImmutableArray();

            return new TabularRecordMapping(recordTypeName, new EquatableArray<TabularFieldMapping>(fieldMappings), new EquatableArray<int>(constructorArguments), new EquatableArray<string>(experimentalDiagnosticIdsSorted));
        }
    }
}
