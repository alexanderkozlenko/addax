// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Analyzers;

[Flags]
internal enum TypeKinds : byte
{
    None,
    IsReferenceType,
    IsNullableValueType,
}
