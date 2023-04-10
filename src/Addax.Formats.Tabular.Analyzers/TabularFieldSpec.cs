// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Analyzers;

internal readonly struct TabularFieldSpec
{
    private readonly string _typeMemberName;
    private readonly TypeMemberAccessTypes _typeMemberAccessTypes;
    private readonly TabularFieldType _fieldType;
    private readonly bool _typeMemberIsNullable;

    public TabularFieldSpec(string typeMemberName, bool typeMemberIsNullable, TypeMemberAccessTypes typeMemberAccessTypes, TabularFieldType fieldType)
    {
        _typeMemberName = typeMemberName;
        _typeMemberIsNullable = typeMemberIsNullable;
        _typeMemberAccessTypes = typeMemberAccessTypes;
        _fieldType = fieldType;
    }

    public string TypeMemberName
    {
        get
        {
            return _typeMemberName;
        }
    }

    public bool TypeMemberIsNullable
    {
        get
        {
            return _typeMemberIsNullable;
        }
    }

    public TypeMemberAccessTypes TypeMemberAccessTypes
    {
        get
        {
            return _typeMemberAccessTypes;
        }
    }

    public TabularFieldType FieldType
    {
        get
        {
            return _fieldType;
        }
    }
}
