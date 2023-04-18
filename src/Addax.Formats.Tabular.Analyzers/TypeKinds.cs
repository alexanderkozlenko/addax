// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Analyzers;

[Flags]
internal enum TypeKinds : byte
{
    None = 0,
    IsReferenceType = 1,
    IsNullableValueType = 2,
}
