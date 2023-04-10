// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

internal enum TabularStreamParsingStatus
{
    NeedMoreData,
    FoundFieldSeparation,
    FoundRecordSeparation,
    FoundInvalidData,
}
