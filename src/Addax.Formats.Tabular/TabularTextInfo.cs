// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

internal readonly struct TabularTextInfo(int charsRequired, bool hasUnsafeChars)
{
    public readonly int CharsRequired = charsRequired;
    public readonly bool HasUnsafeChars = hasUnsafeChars;
}
