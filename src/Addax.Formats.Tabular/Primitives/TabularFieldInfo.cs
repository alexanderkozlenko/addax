// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;

namespace Addax.Formats.Tabular.Primitives;

internal readonly struct TabularFieldInfo
{
    public TabularFieldInfo(int charsUsed, int charsEscaped, TabularSeparator separator, bool hasQuoting, bool isAnnotation)
    {
        Debug.Assert(charsUsed >= 0);
        Debug.Assert(charsUsed <= Array.MaxLength);
        Debug.Assert(charsEscaped >= 0);
        Debug.Assert(charsEscaped <= charsUsed);

        CharsUsed = charsUsed;
        CharsEscaped = charsEscaped;
        Separator = separator;
        HasQuoting = hasQuoting;
        IsAnnotation = isAnnotation;
    }

    public readonly int CharsUsed;
    public readonly int CharsEscaped;
    public readonly TabularSeparator Separator;
    public readonly bool HasQuoting;
    public readonly bool IsAnnotation;
}
