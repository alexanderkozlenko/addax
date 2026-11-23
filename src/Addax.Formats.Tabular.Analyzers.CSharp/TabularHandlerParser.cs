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

        private static readonly SymbolDisplayFormat s_displayFormat = SymbolDisplayFormat.FullyQualifiedFormat
            .WithMiscellaneousOptions(SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions & ~SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        private static readonly SymbolDisplayFormat s_typeDisplayNameFormat = SymbolDisplayFormat.FullyQualifiedFormat
            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
            .WithMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.None);

        private static readonly SymbolDisplayFormat s_valueTypeKeyFormat = s_displayFormat
            .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.ExpandValueTuple);

        private readonly CSharpCompilation _compilation;
        private readonly INamedTypeSymbol _converterBaseType;
        private readonly INamedTypeSymbol _converterAttributeType;
        private readonly INamedTypeSymbol _fieldOrderAttributeType;
        private readonly INamedTypeSymbol _fieldNameAttributeType;

        private TabularHandlerParser(
            CSharpCompilation compilation,
            INamedTypeSymbol converterBaseType,
            INamedTypeSymbol converterAttributeType,
            INamedTypeSymbol fieldOrderAttributeType,
            INamedTypeSymbol fieldNameAttributeType)
        {
            _compilation = compilation;
            _converterBaseType = converterBaseType;
            _converterAttributeType = converterAttributeType;
            _fieldOrderAttributeType = fieldOrderAttributeType;
            _fieldNameAttributeType = fieldNameAttributeType;
        }

        public static TabularHandlerParser Create(Compilation compilation, CancellationToken cancellationToken)
        {
            foreach (var reference in compilation.References)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if ((compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol assembly) && (assembly.Name == "Addax.Formats.Tabular"))
                {
                    var converterBaseType = assembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularConverter`1");
                    var converterAttributeType = assembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularConverterAttribute`1");
                    var fieldOrderAttributeType = assembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularFieldOrderAttribute");
                    var fieldNameAttributeType = assembly.GetTypeByMetadataName("Addax.Formats.Tabular.TabularFieldNameAttribute");

                    if ((converterBaseType == null) ||
                        (converterAttributeType == null) ||
                        (fieldOrderAttributeType == null) ||
                        (fieldNameAttributeType == null))
                    {
                        return null;
                    }

                    return new TabularHandlerParser(
                        (CSharpCompilation)compilation,
                        converterBaseType,
                        converterAttributeType,
                        fieldOrderAttributeType,
                        fieldNameAttributeType);
                }
            }

            return null;
        }

        public TabularRecordParseResult GetRecordMapping(INamedTypeSymbol recordType, EquatableArray<TabularConverterMapping> converterMappings, CancellationToken cancellationToken)
        {
            Debug.Assert(recordType != null);

            var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
            var recordMapping = GetRecordMapping(recordType, converterMappings, diagnosticsBuilder, cancellationToken);

            return new TabularRecordParseResult(recordMapping, diagnosticsBuilder.ToImmutable());
        }

        private TabularRecordMapping? GetRecordMapping(INamedTypeSymbol recordType, EquatableArray<TabularConverterMapping> converterMappings, ImmutableArray<Diagnostic>.Builder diagnosticsBuilder, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_compilation.LanguageVersion < s_minimumLanguageVersion)
            {
                diagnosticsBuilder.Add(Diagnostic.Create(s_diagnostic0000, Location.None, _compilation.LanguageVersion.ToDisplayString()));

                return null;
            }

            if (!RecordTypeIsUsable(recordType, out var recordTypeIssue))
            {
                diagnosticsBuilder.Add(Diagnostic.Create(s_diagnostic0001, recordType.Locations.FirstOrDefault(), recordType.ToDisplayString(), recordTypeIssue));

                return null;
            }

            var recordTypeHasErrors = false;
            var recordMembers = GetFieldsAndProperties(recordType);
            var memberMappings = new SortedDictionary<int, TabularMemberMapping>();
            var settableMembers = new Dictionary<int, ISymbol>();
            var overriddenMembers = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

            foreach (var recordMember in recordMembers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!TryGetAttribute(recordMember, _fieldOrderAttributeType, out var fieldOrderAttribute))
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

                var valueTypeInfo = GetValueTypeInfo(recordMember);

                if (!TypeMemberIsUsable(recordType, recordMember, valueTypeInfo.Type, out var recordMemberIssue))
                {
                    diagnosticsBuilder.Add(CreateMemberDiagnostic(s_diagnostic0002, recordType, recordMember, recordMemberIssue));
                    recordTypeHasErrors = true;

                    continue;
                }

                var converterAttributeCount = 0;
                var converterAttributeIsInvalid = false;

                foreach (var memberAttribute in recordMember.GetAttributes())
                {
                    if (SymbolEqualityComparer.Default.Equals(memberAttribute.AttributeClass?.ConstructedFrom, _converterAttributeType))
                    {
                        converterAttributeCount++;

                        if (!TryGetSupportedConverter(memberAttribute, null, out _, out _, out _, out var converterAttributeIssue) && (converterAttributeIssue == null))
                        {
                            converterAttributeIsInvalid = true;
                        }
                    }
                }

                if (converterAttributeIsInvalid)
                {
                    recordTypeHasErrors = true;

                    continue;
                }

                if (converterAttributeCount > 1)
                {
                    diagnosticsBuilder.Add(CreateMemberDiagnostic(s_diagnostic0003, recordType, recordMember, "the member has more than one converter"));
                    recordTypeHasErrors = true;

                    continue;
                }

                var valueTypeKey = GetValueTypeKey(valueTypeInfo.Type);
                var converterMapping = default(TabularConverterMapping?);

                if (TryGetAttribute(recordMember, _converterAttributeType, out var converterAttribute))
                {
                    if (!TryGetSupportedConverter(converterAttribute, valueTypeInfo.Type, out var converterType, out _, out var converterConstructor, out var converterIssue))
                    {
                        if (converterIssue != null)
                        {
                            diagnosticsBuilder.Add(CreateMemberDiagnostic(s_diagnostic0004, recordType, recordMember, converterIssue));
                        }

                        recordTypeHasErrors = true;

                        continue;
                    }

                    var converterDiagnosticIdentifiers = new HashSet<string>(StringComparer.Ordinal);

                    AddExperimentalDiagnosticIdentifiers(converterType, converterDiagnosticIdentifiers);
                    AddExperimentalDiagnosticIdentifiers(converterConstructor, converterDiagnosticIdentifiers);

                    converterMapping = new TabularConverterMapping(
                        valueTypeKey,
                        converterType.ToDisplayString(s_displayFormat),
                        new EquatableArray<string>(converterDiagnosticIdentifiers.ToImmutableArray().Sort(StringComparer.Ordinal)));
                }
                else
                {
                    foreach (var assemblyConverterMapping in converterMappings)
                    {
                        if (assemblyConverterMapping.ValueTypeKey == valueTypeKey)
                        {
                            converterMapping = assemblyConverterMapping;

                            break;
                        }
                    }
                }

                if (!TryGetConstructorArgument<int>(fieldOrderAttribute, out var fieldOrder))
                {
                    recordTypeHasErrors = true;

                    continue;
                }

                if (fieldOrder < 0)
                {
                    diagnosticsBuilder.Add(CreateMemberDiagnostic(s_diagnostic0003, recordType, recordMember, $"the field order {fieldOrder} is negative"));
                    recordTypeHasErrors = true;

                    continue;
                }

                if (memberMappings.TryGetValue(fieldOrder, out var fieldOrderOwner))
                {
                    diagnosticsBuilder.Add(CreateMemberDiagnostic(s_diagnostic0003, recordType, recordMember, $"the field order {fieldOrder} is already used by '{fieldOrderOwner.Member.ToDisplayString()}'"));
                    recordTypeHasErrors = true;

                    continue;
                }

                var fieldNameLiteral = default(string);

                if (TryGetAttribute(recordMember, _fieldNameAttributeType, out var fieldNameAttribute))
                {
                    if (!TryGetConstructorArgument<string>(fieldNameAttribute, out var fieldName))
                    {
                        if ((fieldNameAttribute.ConstructorArguments.Length == 1) &&
                            (fieldNameAttribute.ConstructorArguments[0].Kind == TypedConstantKind.Primitive) &&
                            (fieldNameAttribute.ConstructorArguments[0].IsNull))
                        {
                            diagnosticsBuilder.Add(CreateMemberDiagnostic(s_diagnostic0003, recordType, recordMember, "the field name is null"));
                        }

                        recordTypeHasErrors = true;

                        continue;
                    }

                    fieldNameLiteral = SyntaxFactory.Literal(fieldName).Text;
                }

                var fieldAccess = TabularFieldAccess.None;

                if (TypeMemberIsSettable(recordMember))
                {
                    fieldAccess |= TabularFieldAccess.Read;
                    settableMembers.Add(fieldOrder, recordMember);
                }

                if (TypeMemberIsGettable(recordMember))
                {
                    fieldAccess |= TabularFieldAccess.Write;
                }

                if (fieldAccess == TabularFieldAccess.None)
                {
                    continue;
                }

                var valueTypeCode = (!converterMapping.HasValue && valueTypeInfo.IsSupported) ? valueTypeInfo.Type.Name : null;

                var memberMapping = new TabularMemberMapping(
                    recordMember,
                    fieldAccess,
                    valueTypeInfo.Type,
                    valueTypeInfo.Modifier,
                    valueTypeInfo.Name,
                    valueTypeKey,
                    valueTypeCode,
                    converterMapping,
                    fieldNameLiteral);

                memberMappings.Add(fieldOrder, memberMapping);
            }

            if (recordTypeHasErrors)
            {
                return null;
            }

            var recordIsConstructible = TryGetRecordConstructor(recordType, settableMembers, out var recordConstructor, out var constructorArguments);
            var recordTypeName = recordType.ToDisplayString(s_displayFormat);
            var recordTypeDisplayName = recordType.ToDisplayString(s_typeDisplayNameFormat);
            var fieldMappingsBuilder = ImmutableArray.CreateBuilder<TabularFieldMapping>(memberMappings.Count);
            var diagnosticIdentifiers = new HashSet<string>(StringComparer.Ordinal);
            var recordAccess = TabularRecordAccess.None;
            var writtenFieldCount = 0;
            var lastWrittenFieldOrder = -1;

            AddExperimentalDiagnosticIdentifiers(recordType, diagnosticIdentifiers);
            AddExperimentalDiagnosticIdentifiers(recordConstructor, diagnosticIdentifiers);

            foreach (var kvp in memberMappings)
            {
                var memberMapping = kvp.Value;
                var fieldAccess = recordIsConstructible ? memberMapping.FieldAccess : (memberMapping.FieldAccess & ~TabularFieldAccess.Read);

                if (fieldAccess == TabularFieldAccess.None)
                {
                    continue;
                }

                AddExperimentalDiagnosticIdentifiers(memberMapping.ValueType, diagnosticIdentifiers);

                if (memberMapping.Member is IPropertySymbol memberProperty)
                {
                    while (memberProperty.OverriddenProperty != null)
                    {
                        memberProperty = memberProperty.OverriddenProperty;
                    }

                    AddExperimentalDiagnosticIdentifiers(memberProperty, diagnosticIdentifiers);

                    if ((fieldAccess & TabularFieldAccess.Write) != 0)
                    {
                        AddExperimentalDiagnosticIdentifiers(memberProperty.GetMethod, diagnosticIdentifiers);
                    }

                    if ((fieldAccess & TabularFieldAccess.Read) != 0)
                    {
                        AddExperimentalDiagnosticIdentifiers(memberProperty.SetMethod, diagnosticIdentifiers);
                    }
                }
                else
                {
                    AddExperimentalDiagnosticIdentifiers(memberMapping.Member, diagnosticIdentifiers);
                }

                if (memberMapping.ConverterMapping.HasValue)
                {
                    foreach (var converterDiagnosticIdentifier in memberMapping.ConverterMapping.Value.DiagnosticIdentifiers)
                    {
                        diagnosticIdentifiers.Add(converterDiagnosticIdentifier);
                    }
                }

                if ((fieldAccess & TabularFieldAccess.Read) != 0)
                {
                    recordAccess |= TabularRecordAccess.Read;
                }

                if ((fieldAccess & TabularFieldAccess.Write) != 0)
                {
                    writtenFieldCount++;
                    lastWrittenFieldOrder = kvp.Key;
                }

                var memberIdentifier = $"@{memberMapping.Member.Name}";
                var memberNameLiteral = SyntaxFactory.Literal($"{recordTypeDisplayName}.{memberMapping.Member.Name}").Text;
                var converterTypeName = memberMapping.ConverterMapping?.ConverterTypeName;

                var fieldMapping = new TabularFieldMapping(
                    kvp.Key,
                    memberIdentifier,
                    memberNameLiteral,
                    fieldAccess,
                    memberMapping.ValueTypeModifier,
                    memberMapping.ValueTypeName,
                    memberMapping.ValueTypeKey,
                    memberMapping.ValueTypeCode,
                    converterTypeName,
                    memberMapping.FieldNameLiteral);

                fieldMappingsBuilder.Add(fieldMapping);
            }

            if ((writtenFieldCount != 0) && (lastWrittenFieldOrder == writtenFieldCount - 1))
            {
                recordAccess |= TabularRecordAccess.Write;
            }

            return new TabularRecordMapping(
                recordTypeName,
                new EquatableArray<TabularFieldMapping>(fieldMappingsBuilder.ToImmutable()),
                new EquatableArray<int>(constructorArguments),
                new EquatableArray<string>(diagnosticIdentifiers.ToImmutableArray().Sort(StringComparer.Ordinal)),
                recordAccess);
        }

        private bool RecordTypeIsUsable(INamedTypeSymbol recordType, out string issue)
        {
            if (recordType.IsStatic)
            {
                issue = "the type is a static class";

                return false;
            }

            if (recordType.IsRefLikeType)
            {
                issue = "the type is a ref struct";

                return false;
            }

            var currentType = recordType;

            while (currentType != null)
            {
                if (currentType.IsFileLocal)
                {
                    issue = SymbolEqualityComparer.Default.Equals(currentType, recordType) ? "the type is file-local" : $"the containing type '{currentType.ToDisplayString()}' is file-local";

                    return false;
                }

                currentType = currentType.ContainingType;
            }

            currentType = recordType;

            while (currentType != null)
            {
                if (!currentType.TypeParameters.IsDefaultOrEmpty)
                {
                    issue = SymbolEqualityComparer.Default.Equals(currentType, recordType) ? "the type is generic" : $"the containing type '{currentType.ToDisplayString()}' is generic";

                    return false;
                }

                currentType = currentType.ContainingType;
            }

            if (!_compilation.IsSymbolAccessibleWithin(recordType, _compilation.Assembly))
            {
                var inaccessibleType = GetOutermostInaccessibleType(recordType);

                issue = SymbolEqualityComparer.Default.Equals(inaccessibleType, recordType) ? "the type is not accessible within the assembly" : $"the containing type '{inaccessibleType.ToDisplayString()}' is not accessible within the assembly";

                return false;
            }

            issue = null;

            return true;
        }

        private Diagnostic CreateMemberDiagnostic(DiagnosticDescriptor descriptor, INamedTypeSymbol recordType, ISymbol recordMember, string issue)
        {
            var location = recordMember.Locations.FirstOrDefault(x => x.IsInSource && _compilation.ContainsSyntaxTree(x.SourceTree)) ?? recordType.Locations.FirstOrDefault();

            return Diagnostic.Create(descriptor, location, recordMember.ToDisplayString(), issue);
        }

        private static bool TryGetAttribute(ISymbol member, INamedTypeSymbol attributeType, out AttributeData attribute)
        {
            var memberAttributes = member.GetAttributes();

            foreach (var memberAttribute in memberAttributes)
            {
                var memberAttributeType = attributeType.IsGenericType ?
                    memberAttribute.AttributeClass?.ConstructedFrom :
                    memberAttribute.AttributeClass;

                if (SymbolEqualityComparer.Default.Equals(memberAttributeType, attributeType))
                {
                    attribute = memberAttribute;

                    return true;
                }
            }

            attribute = null;

            return false;
        }

        private static bool TryGetConstructorArgument<T>(AttributeData attribute, out T value)
        {
            if ((attribute.ConstructorArguments.Length == 1) &&
                (attribute.ConstructorArguments[0].Kind == TypedConstantKind.Primitive) &&
                (attribute.ConstructorArguments[0].Value is T argumentValue))
            {
                value = argumentValue;

                return true;
            }

            value = default;

            return false;
        }

        private static bool TypeRequiredMembersAreSet(INamedTypeSymbol type, ISet<string> setMemberNames)
        {
            while (type != null)
            {
                foreach (var member in type.GetMembers())
                {
                    var memberIsRequired =
                        ((member is IFieldSymbol field) && field.IsRequired) ||
                        ((member is IPropertySymbol property) && property.IsRequired);

                    if (memberIsRequired && !setMemberNames.Contains(member.Name))
                    {
                        return false;
                    }
                }

                type = type.BaseType;
            }

            return true;
        }

        private INamedTypeSymbol GetOutermostInaccessibleType(INamedTypeSymbol type)
        {
            var inaccessibleType = type;
            var currentType = type.ContainingType;

            while (currentType != null)
            {
                if (!_compilation.IsSymbolAccessibleWithin(currentType, _compilation.Assembly))
                {
                    inaccessibleType = currentType;
                }

                currentType = currentType.ContainingType;
            }

            return inaccessibleType;
        }
    }
}
