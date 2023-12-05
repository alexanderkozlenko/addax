// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Specifies the type of a tabular value converter. This class cannot be inherited.</summary>
/// <typeparam name="T">The type of the converter. Must be derived from the <see cref="TabularConverter{T}" /> class and have an accessible parameterless constructor.</typeparam>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class TabularConverterAttribute<T> : Attribute
    where T : class
{
    /// <summary>Initializes a new instance of the <see cref="TabularConverterAttribute{T}" /> class.</summary>
    public TabularConverterAttribute()
    {
    }
}
