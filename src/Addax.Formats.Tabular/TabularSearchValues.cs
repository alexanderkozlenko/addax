// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;

namespace Addax.Formats.Tabular;

internal sealed class TabularSearchValues
{
    public readonly SearchValues<char> ValuesT0;
    public readonly SearchValues<char> ValuesT0DQ;
    public readonly SearchValues<char> ValuesT0T1DQ;
    public readonly SearchValues<char> ValuesQ;
    public readonly SearchValues<char> ValuesQE;

    public TabularSearchValues(string lineTerminator, char delimiter, char quoteSymbol, char escapeSymbol)
    {
        Debug.Assert(lineTerminator is not null);

        ValuesT0 =
            SearchValues.Create(stackalloc[] { lineTerminator[0] });
        ValuesT0DQ =
            SearchValues.Create(stackalloc[] { lineTerminator[0], delimiter, quoteSymbol });
        ValuesT0T1DQ =
            SearchValues.Create(stackalloc[] { lineTerminator[0], lineTerminator[^1], delimiter, quoteSymbol });
        ValuesQ =
            SearchValues.Create(stackalloc[] { quoteSymbol });
        ValuesQE =
            SearchValues.Create(stackalloc[] { quoteSymbol, escapeSymbol });
    }
}
