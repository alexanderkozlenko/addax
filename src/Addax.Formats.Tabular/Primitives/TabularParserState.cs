// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Primitives;

internal struct TabularParserState
{
    public int Position;
    public TabularParsingArea Area;

    public TabularParserState()
    {
    }
}