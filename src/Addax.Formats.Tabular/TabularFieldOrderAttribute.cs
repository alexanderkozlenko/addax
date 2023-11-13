// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1019

namespace Addax.Formats.Tabular;

/// <summary>Specifies the order of a tabular field. This class cannot be inherited.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class TabularFieldOrderAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="TabularFieldOrderAttribute" /> class with the specified order.</summary>
    /// <param name="order">The order of the field. Must be greater than or equal to zero, must be unique within the record type.</param>
    public TabularFieldOrderAttribute(int order)
    {
    }
}
