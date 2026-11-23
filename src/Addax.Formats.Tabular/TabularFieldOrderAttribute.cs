// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Specifies the order of a tabular field. This class cannot be inherited.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class TabularFieldOrderAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="TabularFieldOrderAttribute" /> class, specifying the zero-based position of the field in a record.</summary>
    /// <param name="order">The zero-based position of the field in a record. Must be unique within the record type.</param>
    public TabularFieldOrderAttribute(int order)
    {
    }
}
