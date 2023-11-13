// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

[Flags]
internal enum TabularParsingMode
{
    None = 0,
    StartOfRecord = 1,
    EndOfStream = 2,
}
