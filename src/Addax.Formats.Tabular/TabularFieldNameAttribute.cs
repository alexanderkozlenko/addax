// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1019

namespace Addax.Formats.Tabular;

/// <summary>Specifies the name of a tabular field. This class cannot be inherited.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class TabularFieldNameAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="TabularFieldNameAttribute" /> class with the specified name.</summary>
    /// <param name="name">The name of the field.</param>
    public TabularFieldNameAttribute(string name)
    {
    }
}
