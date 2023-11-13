// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;

namespace Addax.Formats.Tabular;

internal sealed class TabularSearchValues(string lineTerminator, char delimiter, char quoteSymbol, char escapeSymbol)
{
    public readonly SearchValues<char> ValuesT0 =
        SearchValues.Create(stackalloc[] { lineTerminator[0] });
    public readonly SearchValues<char> ValuesT0DQ =
        SearchValues.Create(stackalloc[] { lineTerminator[0], delimiter, quoteSymbol });
    public readonly SearchValues<char> ValuesT0T1DQ =
        SearchValues.Create(stackalloc[] { lineTerminator[0], lineTerminator[^1], delimiter, quoteSymbol });
    public readonly SearchValues<char> ValuesQ =
        SearchValues.Create(stackalloc[] { quoteSymbol });
    public readonly SearchValues<char> ValuesQE =
        SearchValues.Create(stackalloc[] { quoteSymbol, escapeSymbol });
    public readonly SearchValues<char> ValuesE =
        SearchValues.Create(stackalloc[] { escapeSymbol });
}
