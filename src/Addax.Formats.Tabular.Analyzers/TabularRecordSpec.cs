// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Immutable;

namespace Addax.Formats.Tabular.Analyzers;

internal readonly struct TabularRecordSpec
{
    private readonly ImmutableDictionary<int, TabularFieldSpec> _fieldSpecs;
    private readonly string _typeName;
    private readonly bool _schemaIsStrict;

    public TabularRecordSpec(string typeName, bool schemaIsStrict, ImmutableDictionary<int, TabularFieldSpec> fieldSpecs)
    {
        _typeName = typeName;
        _schemaIsStrict = schemaIsStrict;
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
            return _schemaIsStrict;
        }
    }

    public ImmutableDictionary<int, TabularFieldSpec> FieldSpecs
    {
        get
        {
            return _fieldSpecs;
        }
    }
}
