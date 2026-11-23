// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;

namespace Addax.Formats.Tabular.Primitives;

internal sealed class TabularSearchValues
{
    public readonly SearchValues<char> ValueStopChars;
    public readonly SearchValues<char> CharsRequiringQuotes;
    public readonly SearchValues<char> EscapableChars;

    public TabularSearchValues(string lineTerminator, char delimiter, char quoteChar, char escapeChar)
    {
        ValueStopChars =
            SearchValues.Create(stackalloc[] { lineTerminator[0], delimiter, quoteChar });
        CharsRequiringQuotes =
            SearchValues.Create(stackalloc[] { lineTerminator[0], lineTerminator[^1], delimiter, quoteChar });
        EscapableChars = escapeChar != quoteChar ?
            SearchValues.Create(stackalloc[] { quoteChar, escapeChar }) :
            SearchValues.Create(stackalloc[] { quoteChar });
    }
}
