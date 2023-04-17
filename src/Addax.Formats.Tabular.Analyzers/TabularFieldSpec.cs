// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Analyzers;

internal readonly struct TabularFieldSpec
{
    private readonly string _memberName;
    private readonly string _memberTypeName;
    private readonly string? _memberConverterTypeName;
    private readonly TypeKinds _memberTypeKinds;
    private readonly TypeMemberAccessTypes _memberAccessTypes;

    public TabularFieldSpec(string memberName, string memberTypeName, TypeKinds memberTypeKinds, TypeMemberAccessTypes memberAccessTypes, string? memberConverterTypeName)
    {
        _memberName = memberName;
        _memberTypeName = memberTypeName;
        _memberTypeKinds = memberTypeKinds;
        _memberAccessTypes = memberAccessTypes;
        _memberConverterTypeName = memberConverterTypeName;
    }

    public string MemberName
    {
        get
        {
            return _memberName;
        }
    }

    public string MemberTypeName
    {
        get
        {
            return _memberTypeName;
        }
    }

    public TypeKinds MemberTypeKinds
    {
        get
        {
            return _memberTypeKinds;
        }
    }

    public string? MemberConverterTypeName
    {
        get
        {
            return _memberConverterTypeName;
        }
    }

    public TypeMemberAccessTypes MemberAccessTypes
    {
        get
        {
            return _memberAccessTypes;
        }
    }

    public bool MemberHasConverter
    {
        get
        {
            return _memberConverterTypeName is not null;
        }
    }
}
