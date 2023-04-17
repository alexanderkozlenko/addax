// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers;

public partial class TabularConverterGenerator
{
    private sealed class Parser
    {
        private static readonly DiagnosticDescriptor _diagnosticDescriptor0001 = new(
            id: "TAB0001",
            title: "A record cannot be represented as a ref-like value type",
            messageFormat: "A record cannot be represented as a ref-like value type",
            category: "Addax.Formats.Tabular",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor _diagnosticDescriptor0011 = new(
            id: "TAB0011",
            title: "A field converter must derive from 'TabularFieldConverter<T>'",
            messageFormat: "A field converter must derive from 'TabularFieldConverter<T>'",
            category: "Addax.Formats.Tabular",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor _diagnosticDescriptor0012 = new(
            id: "TAB0013",
            title: "An explicitly applied field converter must handle the proper type",
            messageFormat: "An explicitly-applied field converter must handle the proper type",
            category: "Addax.Formats.Tabular",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor _diagnosticDescriptor0013 = new(
            id: "TAB0013",
            title: "An explicitly applied field converter must have a parameterless constructor",
            messageFormat: "An explicitly-applied field converter must have a parameterless constructor",
            category: "Addax.Formats.Tabular",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor _diagnosticDescriptor0021 = new(
            id: "TAB0021",
            title: "A field index must have a unique zero-based value",
            messageFormat: "A field index must have a unique zero-based value",
            category: "Addax.Formats.Tabular",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public bool TryGetImplementationSourceSpec(SourceProductionContext context, ImmutableArray<(INamedTypeSymbol, AttributeData)> types, out TabularConverterGeneratorSpec result)
        {
            var cancellationToken = context.CancellationToken;
            var recordSpecsBuilder = default(ImmutableArray<TabularRecordSpec>.Builder);

            foreach (var (recordType, recordAttribute) in types)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (recordType.IsValueType && recordType.IsRefLikeType)
                {
                    context.ReportDiagnostic(Diagnostic.Create(_diagnosticDescriptor0001, recordType.Locations.FirstOrDefault()));

                    continue;
                }

                if (!TryGetTabularRecordInfo(recordAttribute, out var recordSchemaIsStrict))
                {
                    continue;
                }

                var recordHasSupportedConstructor = TypeHasParameterlessConstructor(recordType, cancellationToken);
                var recordMembers = GetTypeMembers(recordType, cancellationToken);

                var fieldSpecsBuilder = default(ImmutableDictionary<int, TabularFieldSpec>.Builder);

                foreach (var recordMember in recordMembers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!TryGetTabularFieldAttribute(recordMember, out var fieldAttribute, cancellationToken))
                    {
                        continue;
                    }
                    if (!TryGetTabularFieldInfo(fieldAttribute!, out var fieldIndex, out var recordMemberConverterType))
                    {
                        continue;
                    }
                    if (!TryGetTypeMemberTypeInfo(recordMember, out var recordMemberType, out var recordMemberTypeKinds, out var recordMemberTypeName))
                    {
                        continue;
                    }

                    if (recordMemberConverterType is not null)
                    {
                        if (!TryGetConverterFieldType(recordMemberConverterType, out var memberConverterMemberType))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(_diagnosticDescriptor0011, recordMember.Locations.FirstOrDefault()));

                            continue;
                        }
                        if (!SymbolEqualityComparer.Default.Equals(recordMemberType, memberConverterMemberType))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(_diagnosticDescriptor0012, recordMember.Locations.FirstOrDefault()));

                            continue;
                        }
                        if (!TypeHasParameterlessConstructor(recordMemberConverterType, cancellationToken))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(_diagnosticDescriptor0013, recordMember.Locations.FirstOrDefault()));

                            continue;
                        }
                    }

                    if ((fieldIndex < 0) || (fieldSpecsBuilder?.ContainsKey(fieldIndex) is true))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(_diagnosticDescriptor0021, recordMember.Locations.FirstOrDefault()));

                        continue;
                    }

                    var recordMemberAccessTypes = GetTypeMemberAccessTypes(recordMember);

                    if (!recordHasSupportedConstructor)
                    {
                        recordMemberAccessTypes &= ~TypeMemberAccessTypes.Write;

                        if (recordMemberAccessTypes is TypeMemberAccessTypes.None)
                        {
                            continue;
                        }
                    }

                    var recordMemberConverterTypeName = recordMemberConverterType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    var tabularFieldSpec = new TabularFieldSpec(
                        recordMember.Name,
                        recordMemberTypeName!,
                        recordMemberTypeKinds,
                        recordMemberAccessTypes,
                        recordMemberConverterTypeName);

                    fieldSpecsBuilder ??= ImmutableDictionary.CreateBuilder<int, TabularFieldSpec>();
                    fieldSpecsBuilder[fieldIndex] = tabularFieldSpec;
                }

                var fieldSpecs = fieldSpecsBuilder is not null ?
                    fieldSpecsBuilder.ToImmutable() :
                    ImmutableDictionary<int, TabularFieldSpec>.Empty;

                var recordTypeName = recordType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                var recordSpec = new TabularRecordSpec(
                    recordTypeName,
                    recordSchemaIsStrict,
                    fieldSpecs);

                recordSpecsBuilder ??= ImmutableArray.CreateBuilder<TabularRecordSpec>();
                recordSpecsBuilder.Add(recordSpec);
            }

            if (recordSpecsBuilder is not null)
            {
                result = new(recordSpecsBuilder.ToImmutable());

                return true;
            }
            else
            {
                result = new(ImmutableArray<TabularRecordSpec>.Empty);

                return false;
            }
        }

        private static bool TryGetConverterFieldType(INamedTypeSymbol converterType, out INamedTypeSymbol? type)
        {
            if ((converterType.BaseType is { IsGenericType: true, TypeArguments.Length: 1 }) &&
                (converterType.BaseType.TypeArguments[0] is INamedTypeSymbol argumentType))
            {
                var unboundGenericType = converterType.BaseType.ConstructUnboundGenericType();
                var unboundGenericTypeName = unboundGenericType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                if (unboundGenericTypeName == "global::Addax.Formats.Tabular.TabularFieldConverter<>")
                {
                    type = argumentType;

                    return true;
                }
            }

            type = default;

            return false;
        }

        private static bool TryGetTabularFieldInfo(AttributeData attribute, out int index, out INamedTypeSymbol? converterType)
        {
            var arguments = attribute.ConstructorArguments;

            if ((arguments.Length == 2) &&
                (arguments[0] is { Type.SpecialType: SpecialType.System_Int32 }) &&
                (arguments[1] is { Kind: TypedConstantKind.Type }))
            {
                index = (int)arguments[0].Value!;
                converterType = (INamedTypeSymbol?)arguments[1].Value;

                return true;
            }

            index = default;
            converterType = default;

            return false;
        }

        private static bool TryGetTabularFieldAttribute(ISymbol member, out AttributeData? attribute, CancellationToken cancellationToken)
        {
            var memberAttributes = member.GetAttributes();

            foreach (var memberAttribute in memberAttributes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (memberAttribute.AttributeClass is not null)
                {
                    var attributeTypeName = memberAttribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    if (attributeTypeName == "global::Addax.Formats.Tabular.TabularFieldAttribute")
                    {
                        attribute = memberAttribute;

                        return true;
                    }
                }
            }

            attribute = default;

            return false;
        }

        private static bool TryGetTabularRecordInfo(AttributeData attribute, out bool schemaIsStrict)
        {
            var arguments = attribute.ConstructorArguments;

            if ((arguments.Length == 1) &&
                (arguments[0] is { Type.SpecialType: SpecialType.System_Boolean }))
            {
                schemaIsStrict = (bool)arguments[0].Value!;

                return true;
            }

            schemaIsStrict = default;

            return false;
        }

        private static bool TryGetTypeMemberTypeInfo(ISymbol member, out INamedTypeSymbol? type, out TypeKinds typeKinds, out string? name)
        {
            type = member switch
            {
                IFieldSymbol { Type: INamedTypeSymbol } field => (INamedTypeSymbol)field.Type,
                IPropertySymbol { Type: INamedTypeSymbol } property => (INamedTypeSymbol)property.Type,
                _ => null,
            };

            name = default;
            typeKinds = default;

            if ((type is null) || type.IsStatic)
            {
                return false;
            }

            if (type.IsReferenceType)
            {
                typeKinds |= TypeKinds.IsReferenceType;
            }

            if (type.IsGenericType)
            {
                if ((type.ConstructedFrom.SpecialType is SpecialType.System_Nullable_T) &&
                    (type.TypeArguments[0] is INamedTypeSymbol nullableTypeSymbol))
                {
                    type = nullableTypeSymbol;
                    typeKinds |= TypeKinds.IsNullableValueType;
                }
            }

            name = type.SpecialType switch
            {
                SpecialType.System_Char => "global::System.Char",
                SpecialType.System_String => "global::System.String",
                SpecialType.System_Boolean => "global::System.Boolean",
                SpecialType.System_SByte => "global::System.SByte",
                SpecialType.System_Byte => "global::System.Byte",
                SpecialType.System_Int16 => "global::System.Int16",
                SpecialType.System_UInt16 => "global::System.UInt16",
                SpecialType.System_Int32 => "global::System.Int32",
                SpecialType.System_UInt32 => "global::System.UInt32",
                SpecialType.System_Int64 => "global::System.Int64",
                SpecialType.System_UInt64 => "global::System.UInt64",
                SpecialType.System_Single => "global::System.Single",
                SpecialType.System_Double => "global::System.Double",
                SpecialType.System_Decimal => "global::System.Decimal",
                SpecialType.System_DateTime => "global::System.DateTime",
                _ => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            };

            return true;
        }

        private static TypeMemberAccessTypes GetTypeMemberAccessTypes(ISymbol member)
        {
            if (member is IFieldSymbol field)
            {
                var accessTypes = TypeMemberAccessTypes.Read;

                if (!field.IsReadOnly)
                {
                    accessTypes |= TypeMemberAccessTypes.Write;
                }

                return accessTypes;
            }
            else if (member is IPropertySymbol property)
            {
                var accessTypes = TypeMemberAccessTypes.None;

                if (property.GetMethod is not null)
                {
                    accessTypes |= TypeMemberAccessTypes.Read;
                }
                if (property.SetMethod is not null)
                {
                    accessTypes |= TypeMemberAccessTypes.Write;
                }

                return accessTypes;
            }
            else
            {
                return TypeMemberAccessTypes.None;
            }
        }

        private static ImmutableArray<ISymbol> GetTypeMembers(INamedTypeSymbol type, CancellationToken cancellationToken)
        {
            var assembly = type.ContainingAssembly;
            var builder = default(ImmutableArray<ISymbol>.Builder);

            while (type is { SpecialType: SpecialType.None })
            {
                cancellationToken.ThrowIfCancellationRequested();

                var members = type.GetMembers();

                foreach (var member in members)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (member is not (IFieldSymbol or IPropertySymbol))
                    {
                        continue;
                    }

                    if (member.DeclaredAccessibility is Accessibility.Public)
                    {
                        builder ??= ImmutableArray.CreateBuilder<ISymbol>();
                        builder.Add(member);
                    }
                    else if (member.DeclaredAccessibility is Accessibility.Internal)
                    {
                        if (SymbolEqualityComparer.Default.Equals(assembly, type.ContainingAssembly))
                        {
                            builder ??= ImmutableArray.CreateBuilder<ISymbol>();
                            builder.Add(member);
                        }
                    }
                }

                type = type.BaseType!;
            }

            return builder?.ToImmutable() ?? ImmutableArray<ISymbol>.Empty;
        }

        private static bool TypeHasParameterlessConstructor(INamedTypeSymbol type, CancellationToken cancellationToken)
        {
            foreach (var method in type.InstanceConstructors)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if ((method.DeclaredAccessibility is (Accessibility.Public or Accessibility.Internal)) &&
                    (method.Parameters.Length == 0))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
