// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers;

public partial class TabularConverterGenerator
{
    private sealed class Parser
    {
        private static readonly DiagnosticDescriptor _diagnosticDescriptor0011 = new(
            id: "TAB0011",
            title: "Use a supported property type",
            messageFormat: "Use a supported property type",
            category: "Addax.Formats.Tabular",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private static readonly DiagnosticDescriptor _diagnosticDescriptor0012 = new(
            id: "TAB0012",
            title: "Use a unique zero-based field index",
            messageFormat: "Use a unique zero-based field index",
            category: "Addax.Formats.Tabular",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public bool TryGetImplementationSourceSpec(SourceProductionContext context, ImmutableArray<(INamedTypeSymbol TypeSymbol, AttributeData AttributeData)> typeSymbols, out TabularConverterGeneratorSpec result)
        {
            var cancellationToken = context.CancellationToken;
            var tabularRecordSpecsBuilder = default(ImmutableArray<TabularRecordSpec>.Builder);

            foreach (var (typeSymbol, attributeData) in typeSymbols)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!TryGetTabularRecordInfo(attributeData, out var tabularRecordSpecIsStrict))
                {
                    continue;
                }

                var typeHasSuitableConstructor = TypeHasSuitableConstructor(typeSymbol);
                var typeHasSuitableMemberSet = true;

                var typeMemberSymbols = GetTypeMembers(typeSymbol, cancellationToken);
                var tabularFieldSpecsBuilder = default(ImmutableDictionary<int, TabularFieldSpec>.Builder);

                foreach (var typeMemberSymbol in typeMemberSymbols)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var typeMemberAttributeData = typeMemberSymbol
                        .GetAttributes()
                        .FirstOrDefault(static x => x.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::Addax.Formats.Tabular.TabularFieldAttribute");

                    if ((typeMemberAttributeData is null) || !TryGetTabularFieldInfo(typeMemberAttributeData, out var tabularFieldIndex))
                    {
                        continue;
                    }

                    if ((tabularFieldIndex < 0) || ((tabularFieldSpecsBuilder is not null) && tabularFieldSpecsBuilder.ContainsKey(tabularFieldIndex)))
                    {
                        typeHasSuitableMemberSet = false;
                        context.ReportDiagnostic(Diagnostic.Create(_diagnosticDescriptor0012, typeMemberSymbol.Locations.FirstOrDefault()));

                        break;
                    }

                    if (!TryGetTypeMemberType(typeMemberSymbol, out var tabularFieldType, out var typeMemberIsNullable))
                    {
                        typeHasSuitableMemberSet = false;
                        context.ReportDiagnostic(Diagnostic.Create(_diagnosticDescriptor0011, typeMemberSymbol.Locations.FirstOrDefault()));

                        break;
                    }

                    var typeMemberAccessTypes = GetTypeMemberAccessTypes(typeMemberSymbol);

                    if (!typeHasSuitableConstructor)
                    {
                        typeMemberAccessTypes &= ~TypeMemberAccessTypes.Write;

                        if (typeMemberAccessTypes == TypeMemberAccessTypes.None)
                        {
                            continue;
                        }
                    }

                    var tabularFieldSpec = new TabularFieldSpec(
                        typeMemberSymbol.Name,
                        typeMemberIsNullable,
                        typeMemberAccessTypes,
                        tabularFieldType);

                    tabularFieldSpecsBuilder ??= ImmutableDictionary.CreateBuilder<int, TabularFieldSpec>();
                    tabularFieldSpecsBuilder[tabularFieldIndex] = tabularFieldSpec;
                }

                if (typeHasSuitableMemberSet)
                {
                    var tabularFieldSpecs = tabularFieldSpecsBuilder is not null ?
                        tabularFieldSpecsBuilder.ToImmutable() :
                        ImmutableDictionary<int, TabularFieldSpec>.Empty;

                    var tabularRecordSpec = new TabularRecordSpec(
                        typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        tabularRecordSpecIsStrict,
                        tabularFieldSpecs);

                    tabularRecordSpecsBuilder ??= ImmutableArray.CreateBuilder<TabularRecordSpec>();
                    tabularRecordSpecsBuilder.Add(tabularRecordSpec);
                }
            }

            if (tabularRecordSpecsBuilder is not null)
            {
                result = new(tabularRecordSpecsBuilder.ToImmutable());

                return true;
            }
            else
            {
                result = new(ImmutableArray<TabularRecordSpec>.Empty);

                return false;
            }
        }

        private static bool TryGetTypeMemberType(ISymbol typeMemberSymbol, out TabularFieldType tabularFieldType, out bool typeMemberIsNullable)
        {
            tabularFieldType = TabularFieldType.None;
            typeMemberIsNullable = false;

            var typeMemberTypeSymbol = typeMemberSymbol switch
            {
                IFieldSymbol { Type: INamedTypeSymbol } fieldSymbol => (INamedTypeSymbol)fieldSymbol.Type,
                IPropertySymbol { Type: INamedTypeSymbol } propertySymbol => (INamedTypeSymbol)propertySymbol.Type,
                _ => null,
            };

            if (typeMemberTypeSymbol is null)
            {
                return false;
            }

            if (typeMemberTypeSymbol.IsGenericType)
            {
                if ((typeMemberTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T) &&
                    (typeMemberTypeSymbol.TypeArguments[0] is INamedTypeSymbol nullableTypeSymbol))
                {
                    typeMemberIsNullable = true;
                    typeMemberTypeSymbol = nullableTypeSymbol;
                }
                else
                {
                    return false;
                }
            }

            tabularFieldType = typeMemberTypeSymbol.SpecialType switch
            {
                SpecialType.System_Char => TabularFieldType.Char,
                SpecialType.System_String => TabularFieldType.String,
                SpecialType.System_Boolean => TabularFieldType.Boolean,
                SpecialType.System_SByte => TabularFieldType.SByte,
                SpecialType.System_Byte => TabularFieldType.Byte,
                SpecialType.System_Int16 => TabularFieldType.Int16,
                SpecialType.System_UInt16 => TabularFieldType.UInt16,
                SpecialType.System_Int32 => TabularFieldType.Int32,
                SpecialType.System_UInt32 => TabularFieldType.UInt32,
                SpecialType.System_Int64 => TabularFieldType.Int64,
                SpecialType.System_UInt64 => TabularFieldType.UInt64,
                SpecialType.System_Single => TabularFieldType.Single,
                SpecialType.System_Double => TabularFieldType.Double,
                SpecialType.System_Decimal => TabularFieldType.Decimal,
                SpecialType.System_DateTime => TabularFieldType.DateTime,
                _ => TabularFieldType.None,
            };

            if (tabularFieldType == TabularFieldType.None)
            {
                if (typeMemberTypeSymbol.IsReferenceType)
                {
                    return false;
                }

                tabularFieldType = typeMemberTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) switch
                {
                    "global::System.Text.Rune" => TabularFieldType.Rune,
                    "global::System.Int128" => TabularFieldType.Int128,
                    "global::System.UInt128" => TabularFieldType.UInt128,
                    "global::System.Numerics.BigInteger" => TabularFieldType.BigInteger,
                    "global::System.Half" => TabularFieldType.Half,
                    "global::System.Numerics.Complex" => TabularFieldType.Complex,
                    "global::System.TimeSpan" => TabularFieldType.TimeSpan,
                    "global::System.TimeOnly" => TabularFieldType.TimeOnly,
                    "global::System.DateOnly" => TabularFieldType.DateOnly,
                    "global::System.DateTimeOffset" => TabularFieldType.DateTimeOffset,
                    "global::System.Guid" => TabularFieldType.Guid,
                    _ => TabularFieldType.None,
                };
            }

            return tabularFieldType != TabularFieldType.None;
        }

        private static TypeMemberAccessTypes GetTypeMemberAccessTypes(ISymbol typeMemberSymbol)
        {
            if (typeMemberSymbol is IFieldSymbol fieldSymbol)
            {
                var propertyAccessTypes = TypeMemberAccessTypes.Read;

                if (!fieldSymbol.IsReadOnly)
                {
                    propertyAccessTypes |= TypeMemberAccessTypes.Write;
                }

                return propertyAccessTypes;
            }
            else if (typeMemberSymbol is IPropertySymbol propertySymbol)
            {
                var propertyAccessTypes = TypeMemberAccessTypes.None;

                if (propertySymbol.GetMethod is not null)
                {
                    propertyAccessTypes |= TypeMemberAccessTypes.Read;
                }
                if (propertySymbol.SetMethod is not null)
                {
                    propertyAccessTypes |= TypeMemberAccessTypes.Write;
                }

                return propertyAccessTypes;
            }
            else
            {
                return TypeMemberAccessTypes.None;
            }
        }

        private static ImmutableArray<ISymbol> GetTypeMembers(INamedTypeSymbol typeSymbol, CancellationToken cancellationToken)
        {
            var typeSymbolAssembly = typeSymbol.ContainingAssembly;
            var builder = default(ImmutableArray<ISymbol>.Builder);

            while (typeSymbol is { SpecialType: SpecialType.None })
            {
                cancellationToken.ThrowIfCancellationRequested();

                var typeMemberSymbols = typeSymbol.GetMembers();

                foreach (var typeMemberSymbol in typeMemberSymbols)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (typeMemberSymbol is not (IFieldSymbol or IPropertySymbol))
                    {
                        continue;
                    }

                    if (typeMemberSymbol.DeclaredAccessibility is Accessibility.Public)
                    {
                        builder ??= ImmutableArray.CreateBuilder<ISymbol>();
                        builder.Add(typeMemberSymbol);
                    }
                    else if (typeMemberSymbol.DeclaredAccessibility is Accessibility.Internal)
                    {
                        if (SymbolEqualityComparer.Default.Equals(typeSymbolAssembly, typeSymbol.ContainingAssembly))
                        {
                            builder ??= ImmutableArray.CreateBuilder<ISymbol>();
                            builder.Add(typeMemberSymbol);
                        }
                    }
                }

                typeSymbol = typeSymbol.BaseType!;
            }

            return builder is not null ? builder.ToImmutable() : ImmutableArray<ISymbol>.Empty;
        }

        private static bool TryGetTabularFieldInfo(AttributeData attributeData, out int tabularFieldIndex)
        {
            var constructorArguments = attributeData.ConstructorArguments;

            if ((constructorArguments.Length == 1) && (constructorArguments[0] is { Type.SpecialType: SpecialType.System_Int32 }))
            {
                tabularFieldIndex = (int)constructorArguments[0].Value!;

                return true;
            }

            tabularFieldIndex = default;

            return false;
        }

        private static bool TryGetTabularRecordInfo(AttributeData attributeData, out bool tabularRecordSpecIsStrict)
        {
            var constructorArguments = attributeData.ConstructorArguments;

            if ((constructorArguments.Length == 1) && (constructorArguments[0] is { Type.SpecialType: SpecialType.System_Boolean }))
            {
                tabularRecordSpecIsStrict = (bool)constructorArguments[0].Value!;

                return true;
            }

            tabularRecordSpecIsStrict = default;

            return false;
        }

        private static bool TypeHasSuitableConstructor(INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.InstanceConstructors
                .Where(static x => (x.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal) && (x.Parameters.Length == 0))
                .Any();
        }
    }
}
