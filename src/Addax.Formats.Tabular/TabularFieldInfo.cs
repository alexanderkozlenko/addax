// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

internal readonly struct TabularFieldInfo(int charsUsed, int charsEscaped, TabularSeparator separator, bool hasQuoting, bool isAnnotation)
{
    public readonly int CharsUsed = charsUsed;
    public readonly int CharsEscaped = charsEscaped;
    public readonly TabularSeparator Separator = separator;
    public readonly bool HasQuoting = hasQuoting;
    public readonly bool IsAnnotation = isAnnotation;
}
