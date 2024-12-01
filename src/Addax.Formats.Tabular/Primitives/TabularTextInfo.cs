// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;

namespace Addax.Formats.Tabular.Primitives;

internal readonly struct TabularTextInfo
{
    public readonly int CharsRequired;
    public readonly bool HasUnsafeChars;

    public TabularTextInfo(int charsRequired, bool hasUnsafeChars)
    {
        Debug.Assert(charsRequired >= 0);
        Debug.Assert(charsRequired <= Array.MaxLength);

        CharsRequired = charsRequired;
        HasUnsafeChars = hasUnsafeChars;
    }
}
