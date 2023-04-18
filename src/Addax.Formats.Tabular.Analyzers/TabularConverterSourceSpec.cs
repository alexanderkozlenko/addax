// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;

namespace Addax.Formats.Tabular.Analyzers;

internal readonly struct TabularConverterSourceSpec
{
    private readonly ImmutableArray<TabularRecordSpec> _recordSpecs;

    public TabularConverterSourceSpec(ImmutableArray<TabularRecordSpec> recordSpecs)
    {
        _recordSpecs = recordSpecs;
    }

    public ImmutableArray<TabularRecordSpec> RecordSpecs
    {
        get
        {
            return _recordSpecs;
        }
    }
}
