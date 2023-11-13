// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    [Flags]
    internal enum TypeMemberAccess
    {
        None = 0,
        Read = 1,
        Write = 2,
    }
}
