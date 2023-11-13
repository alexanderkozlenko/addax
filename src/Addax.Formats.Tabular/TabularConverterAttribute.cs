// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1019

namespace Addax.Formats.Tabular;

/// <summary>Specifies the type of a tabular value converter. This class cannot be inherited.</summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class TabularConverterAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="TabularConverterAttribute" /> class with the specified converter type.</summary>
    /// <param name="converterType">The type of the converter. Must be derived from the <see cref="TabularConverter{T}" /> class and have an accessible parameterless constructor.</param>
    public TabularConverterAttribute(Type converterType)
    {
    }
}
