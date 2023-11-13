// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Instructs the source generator to generate a record handler for the target type. This class cannot be inherited.</summary>
/// <remarks>An instance of the generated record handler will be automatically added to the <see cref="TabularRegistry.Handlers" /> shared collection, which is used to resolve the required handler for reading and writing records. Use the <see cref="TabularFieldOrderAttribute" /> attribute to map a type member to a tabular field, and optional attributes <see cref="TabularFieldNameAttribute" /> and <see cref="TabularConverterAttribute" /> to define a field name and apply a value converter.</remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class TabularRecordAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="TabularRecordAttribute" /> class.</summary>
    public TabularRecordAttribute()
    {
    }
}
