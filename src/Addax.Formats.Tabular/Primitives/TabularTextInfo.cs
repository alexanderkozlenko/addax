// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Primitives;

internal readonly struct TabularTextInfo
{
    public readonly int CharsRequired;
    public readonly bool HasQuoting;

    public TabularTextInfo(int charsRequired, bool hasQuoting)
    {
        CharsRequired = charsRequired;
        HasQuoting = hasQuoting;
    }
}
