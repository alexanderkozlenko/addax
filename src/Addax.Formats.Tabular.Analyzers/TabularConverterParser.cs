// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers;

internal sealed partial class TabularConverterParser
{
    public TabularConverterSourceSpec GetImplementationSourceSpec(SourceProductionContext context, ImmutableArray<(INamedTypeSymbol, AttributeData)> types)
    {
        var cancellationToken = context.CancellationToken;
        var recordSpecsBuilder = default(ImmutableArray<TabularRecordSpec>.Builder);

        foreach (var (recordType, recordAttribute) in types)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!TryGetTabularRecordInfo(recordAttribute, out var recordSchemaIsStrict))
            {
                continue;
            }

            if (recordType.IsValueType && recordType.IsRefLikeType)
            {
                ReportDiagnostic(context, _diagnostic0001, recordType);

                continue;
            }

            var recordHasSupportedConstructor = TypeHasSupportedConstructor(recordType, cancellationToken);
            var recordMembers = GetSupportedTypeMembers(recordType, cancellationToken);

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
                        ReportDiagnostic(context, _diagnostic0011, recordMember);

                        continue;
                    }
                    if (!SymbolEqualityComparer.Default.Equals(recordMemberType, memberConverterMemberType))
                    {
                        ReportDiagnostic(context, _diagnostic0012, recordMember);

                        continue;
                    }
                    if (!TypeHasSupportedConstructor(recordMemberConverterType, cancellationToken))
                    {
                        ReportDiagnostic(context, _diagnostic0013, recordMember);

                        continue;
                    }
                }

                if ((fieldIndex < 0) || (fieldSpecsBuilder?.ContainsKey(fieldIndex) is true))
                {
                    ReportDiagnostic(context, _diagnostic0021, recordMember);

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
            return new(recordSpecsBuilder.ToImmutable());
        }
        else
        {
            return new(ImmutableArray<TabularRecordSpec>.Empty);
        }
    }

    private static bool TryGetConverterFieldType(INamedTypeSymbol converterType, out INamedTypeSymbol? type)
    {
        if ((converterType.BaseType is { IsGenericType: true, TypeArguments.Length: 1 }) &&
            (converterType.BaseType.TypeArguments[0] is INamedTypeSymbol argumentType) &&
            (converterType.BaseType.ConstructedFrom is { IsGenericType: true, TypeArguments.Length: 1 }) &&
            (converterType.BaseType.ConstructedFrom.TypeArguments[0] is ITypeSymbol))
        {
            var constructedFromTypeName = converterType.BaseType.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            if (constructedFromTypeName == "global::Addax.Formats.Tabular.TabularFieldConverter<T>")
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
            converterType = arguments[1].Value as INamedTypeSymbol;

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

            var attributeTypeName = memberAttribute.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            if (attributeTypeName == "global::Addax.Formats.Tabular.TabularFieldAttribute")
            {
                attribute = memberAttribute;

                return true;
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
        else if (type.IsValueType && type.IsGenericType)
        {
            if ((type.ConstructedFrom.SpecialType is SpecialType.System_Nullable_T) &&
                (type.TypeArguments[0] is INamedTypeSymbol nullableType))
            {
                type = nullableType;
                typeKinds |= TypeKinds.IsNullableValueType;
            }
        }

        name = type.SpecialType switch
        {
            SpecialType.System_Boolean => "global::System.Boolean",
            SpecialType.System_Byte => "global::System.Byte",
            SpecialType.System_Char => "global::System.Char",
            SpecialType.System_DateTime => "global::System.DateTime",
            SpecialType.System_Decimal => "global::System.Decimal",
            SpecialType.System_Double => "global::System.Double",
            SpecialType.System_Int16 => "global::System.Int16",
            SpecialType.System_Int32 => "global::System.Int32",
            SpecialType.System_Int64 => "global::System.Int64",
            SpecialType.System_SByte => "global::System.SByte",
            SpecialType.System_Single => "global::System.Single",
            SpecialType.System_String => "global::System.String",
            SpecialType.System_UInt16 => "global::System.UInt16",
            SpecialType.System_UInt32 => "global::System.UInt32",
            SpecialType.System_UInt64 => "global::System.UInt64",
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

    private static ImmutableArray<ISymbol> GetSupportedTypeMembers(INamedTypeSymbol type, CancellationToken cancellationToken)
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

    private static bool TypeHasSupportedConstructor(INamedTypeSymbol type, CancellationToken cancellationToken)
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

    private static void ReportDiagnostic(SourceProductionContext context, DiagnosticDescriptor descriptor, ISymbol symbol)
    {
        context.ReportDiagnostic(Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault()));
    }
}
