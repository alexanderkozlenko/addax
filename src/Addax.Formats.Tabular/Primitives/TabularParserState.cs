// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Primitives;

internal struct TabularParserState
{
    public int CharsParsed;
    public int CharsEscaped;
    public TabularParsingArea CurrentArea;
    public bool FoundQuoting;
    public bool FoundAnnotation;
}
