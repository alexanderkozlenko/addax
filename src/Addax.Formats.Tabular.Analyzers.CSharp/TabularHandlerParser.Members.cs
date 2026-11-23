// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal partial class TabularHandlerParser
    {
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

        private static ImmutableArray<ISymbol> GetFieldsAndProperties(ITypeSymbol type)
        {
            var membersBuilder = ImmutableArray.CreateBuilder<ISymbol>();

            while (type != null)
            {
                var members = type.GetMembers();

                foreach (var member in members)
                {
                    if ((member is IFieldSymbol) || (member is IPropertySymbol))
                    {
                        membersBuilder.Add(member);
                    }
                }

                type = type.BaseType;
            }

            return membersBuilder.ToImmutable();
        }

        private bool TypeMemberIsUsable(INamedTypeSymbol type, ISymbol member, ITypeSymbol memberType, out string issue)
        {
            if ((member is IFieldSymbol field) && field.IsConst)
            {
                issue = "the member is a constant";

                return false;
            }

            if (member.IsStatic)
            {
                issue = "the member is static";

                return false;
            }

            if (member is IPropertySymbol property)
            {
                if (property.IsIndexer)
                {
                    issue = "the member is an indexer";

                    return false;
                }

                if (!property.ExplicitInterfaceImplementations.IsDefaultOrEmpty)
                {
                    issue = "the member is an explicit interface implementation";

                    return false;
                }
            }

            if (!_compilation.IsSymbolAccessibleWithin(member, _compilation.Assembly))
            {
                issue = "the member is not accessible within the assembly";

                return false;
            }

            if (TryGetHidingMember(type, member, out var hidingMember))
            {
                issue = $"the member is hidden by '{hidingMember.ToDisplayString()}'";

                return false;
            }

            if (TryGetPointerType(memberType, out var pointerType))
            {
                issue = SymbolEqualityComparer.Default.Equals(pointerType, memberType) ?
                    $"the member type '{memberType.ToDisplayString()}' is a pointer type" :
                    $"the member type '{memberType.ToDisplayString()}' contains the pointer type '{pointerType.ToDisplayString()}'";

                return false;
            }

            if (memberType.IsRefLikeType)
            {
                issue = $"the member type '{memberType.ToDisplayString()}' is a ref struct";

                return false;
            }

            issue = null;

            return true;
        }

        private bool TryGetHidingMember(INamedTypeSymbol type, ISymbol member, out ISymbol hidingMember)
        {
            while (type != null)
            {
                foreach (var candidate in type.GetMembers(member.Name))
                {
                    if (SymbolEqualityComparer.Default.Equals(candidate, member))
                    {
                        hidingMember = null;

                        return false;
                    }

                    if (!_compilation.IsSymbolAccessibleWithin(candidate, _compilation.Assembly))
                    {
                        continue;
                    }

                    if (candidate is IPropertySymbol candidateProperty)
                    {
                        var overriddenProperty = candidateProperty.OverriddenProperty;

                        while (overriddenProperty != null)
                        {
                            if (SymbolEqualityComparer.Default.Equals(overriddenProperty, member))
                            {
                                hidingMember = null;

                                return false;
                            }

                            overriddenProperty = overriddenProperty.OverriddenProperty;
                        }
                    }

                    hidingMember = candidate;

                    return true;
                }

                type = type.BaseType;
            }

            hidingMember = null;

            return false;
        }

        private static bool TryGetPointerType(ITypeSymbol type, out ITypeSymbol pointerType)
        {
            if ((type.TypeKind == TypeKind.Pointer) || (type.TypeKind == TypeKind.FunctionPointer))
            {
                pointerType = type;

                return true;
            }

            if (type is IArrayTypeSymbol arrayType)
            {
                return TryGetPointerType(arrayType.ElementType, out pointerType);
            }

            if (type is INamedTypeSymbol namedType)
            {
                while (namedType != null)
                {
                    foreach (var typeArgument in namedType.TypeArguments)
                    {
                        if (TryGetPointerType(typeArgument, out pointerType))
                        {
                            return true;
                        }
                    }

                    namedType = namedType.ContainingType;
                }
            }

            pointerType = null;

            return false;
        }

        private bool TypeMemberIsGettable(ISymbol member)
        {
            if (member is IFieldSymbol)
            {
                return true;
            }

            if (member is IPropertySymbol property)
            {
                while (property != null)
                {
                    if (property.GetMethod != null)
                    {
                        return _compilation.IsSymbolAccessibleWithin(property.GetMethod, _compilation.Assembly);
                    }

                    property = property.OverriddenProperty;
                }
            }

            return false;
        }

        private bool TypeMemberIsSettable(ISymbol member)
        {
            if (member is IFieldSymbol field)
            {
                return !field.IsReadOnly;
            }

            if (member is IPropertySymbol property)
            {
                while (property != null)
                {
                    if (property.SetMethod != null)
                    {
                        return _compilation.IsSymbolAccessibleWithin(property.SetMethod, _compilation.Assembly);
                    }

                    property = property.OverriddenProperty;
                }
            }

            return false;
        }

        private static (ITypeSymbol Type, string Name, bool IsSupported, TabularTypeModifier Modifier) GetValueTypeInfo(ISymbol member)
        {
            var valueType = default(ITypeSymbol);

            if (member is IFieldSymbol field)
            {
                valueType = field.Type;
            }
            else if (member is IPropertySymbol property)
            {
                valueType = property.Type;
            }
            else
            {
                throw new NotSupportedException();
            }

            var valueTypeModifier = TabularTypeModifier.None;

            if ((valueType is INamedTypeSymbol namedType) && (namedType.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T))
            {
                valueTypeModifier = TabularTypeModifier.Nullable;
                valueType = namedType.TypeArguments[0];
            }
            else if (valueType.IsReferenceType && (valueType.NullableAnnotation == NullableAnnotation.Annotated))
            {
                valueTypeModifier = TabularTypeModifier.Annotation;
            }

            var valueTypeName = valueType.ToDisplayString(s_displayFormat);
            var valueTypeIsSupported = SpecialTypeIsSupported(valueType.SpecialType) || s_supportedTypeNames.Contains(valueTypeName);

            return (valueType, valueTypeName, valueTypeIsSupported, valueTypeModifier);
        }

        private static bool SpecialTypeIsSupported(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_Byte:
                case SpecialType.System_Char:
                case SpecialType.System_DateTime:
                case SpecialType.System_Decimal:
                case SpecialType.System_Double:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_SByte:
                case SpecialType.System_Single:
                case SpecialType.System_String:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}
