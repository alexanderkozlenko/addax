// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Specifies the name of a tabular field. This class cannot be inherited.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class TabularFieldNameAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="TabularFieldNameAttribute" /> class, specifying the name of the field in the header.</summary>
    /// <param name="name">The name of the field in the header.</param>
    public TabularFieldNameAttribute(string name)
    {
    }
}
