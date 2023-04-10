// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Analyzers;

[Flags]
internal enum TypeMemberAccessTypes : byte
{
    None = 0,
    Read = 1,
    Write = 2,
}
