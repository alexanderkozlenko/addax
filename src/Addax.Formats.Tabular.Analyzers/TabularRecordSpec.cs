// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;

namespace Addax.Formats.Tabular.Analyzers;

internal readonly struct TabularRecordSpec
{
    private readonly ImmutableDictionary<int, TabularFieldSpec>? _fieldSpecs;
    private readonly string _typeName;
    private readonly bool _isStrict;

    public TabularRecordSpec(string typeName, bool isStrict, ImmutableDictionary<int, TabularFieldSpec>? fieldSpecs)
    {
        _typeName = typeName;
        _isStrict = isStrict;
        _fieldSpecs = fieldSpecs;
    }

    public string TypeName
    {
        get
        {
            return _typeName;
        }
    }

    public bool IsStrict
    {
        get
        {
            return _isStrict;
        }
    }

    public ImmutableDictionary<int, TabularFieldSpec>? FieldSpecs
    {
        get
        {
            return _fieldSpecs;
        }
    }
}
