// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

public partial class TabularStringFactory
{
    private partial class StringTable
    {
        private struct Entry
        {
            public int HashCode;
            public int Next;
            public string Value;
        }
    }
}
