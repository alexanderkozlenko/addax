﻿// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular.Analyzers;

internal partial class TabularConverterEmitter
{
    private static void BuildModuleInitializerSource(StringBuilder builder, int converterCount, CancellationToken cancellationToken)
    {
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine();
        builder.AppendLine("using Tabular = global::Addax.Formats.Tabular;");
        builder.AppendLine();
        builder.AppendLine("namespace Addax.Formats.Tabular.Converters;");
        builder.AppendLine();
        builder.AppendLine($"[global::System.CodeDom.Compiler.GeneratedCode(\"{_assemblyName.Name}\", \"{_assemblyName.Version}\")]");
        builder.AppendLine("file static class ModuleInitializer");
        builder.AppendLine("{");
        builder.AppendLine("    [global::System.Runtime.CompilerServices.ModuleInitializer]");
        builder.AppendLine("    public static void Initialize()");
        builder.AppendLine("    {");

        for (var i = 0; i < converterCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            builder.AppendLine($"        TryAddTabularRecordConverter<Tabular::Converters.GeneratedRecordConverter{i}>();");
        }

        builder.AppendLine("    }");
        builder.AppendLine();
        builder.AppendLine("    private static void TryAddTabularRecordConverter<T>()");
        builder.AppendLine("        where T : Tabular::TabularRecordConverter, new()");
        builder.AppendLine("    {");
        builder.AppendLine("        var converter = new T();");
        builder.AppendLine();
        builder.AppendLine("        Tabular::TabularRecordConverterRegistry.Shared.TryAdd(converter.RecordType, converter);");
        builder.AppendLine("    }");
        builder.AppendLine("}");
    }

    private static void BuildRecordConverterSource(StringBuilder builder, in TabularRecordSpec recordSpec, int converterIndex, CancellationToken cancellationToken)
    {
        var typeReadingFieldsCount = GetReadingFieldsCount(recordSpec);
        var typeWritingFieldsCount = GetWritingFieldsCount(recordSpec);
        var typeHasFieldConverters = TypeHasFieldConverters(recordSpec);

        var fieldIndices = GetSortedKeys(recordSpec.FieldSpecs);

        builder.AppendLine("// <auto-generated />");
        builder.AppendLine();
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        builder.AppendLine("using Tabular = global::Addax.Formats.Tabular;");
        builder.AppendLine();
        builder.AppendLine("namespace Addax.Formats.Tabular.Converters;");
        builder.AppendLine();
        builder.AppendLine($"[global::System.CodeDom.Compiler.GeneratedCode(\"{_assemblyName.Name}\", \"{_assemblyName.Version}\")]");
        builder.AppendLine($"internal sealed class GeneratedRecordConverter{converterIndex} : Tabular::TabularRecordConverter<{recordSpec.TypeName}>");
        builder.AppendLine("{");

        if (typeReadingFieldsCount > 0)
        {
            builder.AppendLine("    [global::System.Runtime.CompilerServices.AsyncMethodBuilder(typeof(global::System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder<>))]");
            builder.AppendLine($"    public override async global::System.Threading.Tasks.ValueTask<Tabular::TabularRecord<{recordSpec.TypeName}>> ReadRecordAsync(Tabular::TabularFieldReader reader, Tabular::TabularRecordReaderContext context, global::System.Threading.CancellationToken cancellationToken)");
            builder.AppendLine("    {");

            if (typeHasFieldConverters)
            {
                for (var i = 0; i < fieldIndices.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var fieldIndex = fieldIndices[i];
                    var fieldSpec = recordSpec.FieldSpecs[fieldIndex];

                    if (!fieldSpec.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write) || !fieldSpec.MemberHasConverter)
                    {
                        continue;
                    }

                    var converterTypeName = fieldSpec.MemberConverterTypeName;

                    builder.AppendLine($"        var converter{fieldIndex} = context.ConverterFactory.CreateFieldConverter<{converterTypeName}>();");
                }

                builder.AppendLine();
            }

            for (var i = 0; i < fieldIndices.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fieldIndex = fieldIndices[i];
                var fieldSpec = recordSpec.FieldSpecs[fieldIndex];

                if (!fieldSpec.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write))
                {
                    continue;
                }

                var fieldTypeName = fieldSpec.MemberTypeName;

                if (!fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsNullableValueType))
                {
                    builder.AppendLine($"        var value{fieldIndex} = default({fieldTypeName});");
                }
                else
                {
                    builder.AppendLine($"        var value{fieldIndex} = default({fieldTypeName}?);");
                }
            }

            builder.AppendLine();

            var commentCheckEmitted = false;

            for (var i = 0; i < fieldIndices.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fieldIndex = fieldIndices[i];
                var fieldSpec = recordSpec.FieldSpecs[fieldIndex];

                if (!fieldSpec.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write))
                {
                    continue;
                }

                var fieldsToSkip = i != 0 ? fieldIndex - fieldIndices[i - 1] - 1 : fieldIndex;

                if (fieldsToSkip > 0)
                {
                    builder.AppendLine($"        for (var i = 0; i < {fieldsToSkip}; i++)");
                    builder.AppendLine("        {");

                    if (!recordSpec.IsStrict)
                    {
                        builder.AppendLine("            if (!await reader.MoveNextFieldAsync(cancellationToken).ConfigureAwait(false))");
                        builder.AppendLine("            {");
                        builder.AppendLine($"                return Tabular::TabularRecord<{recordSpec.TypeName}>.FromContent(new()");
                        builder.AppendLine("                {");

                        for (var j = 0; j < i; j++)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var fieldIndexInner = fieldIndices[j];
                            var fieldSpecInner = recordSpec.FieldSpecs[fieldIndexInner];

                            if (!fieldSpecInner.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write))
                            {
                                continue;
                            }

                            builder.AppendLine($"                    {fieldSpecInner.MemberName} = value{fieldIndexInner},");
                        }

                        builder.AppendLine("                });");
                        builder.AppendLine("            }");
                    }
                    else
                    {
                        builder.AppendLine("            if (!await reader.MoveNextFieldAsync(cancellationToken).ConfigureAwait(false))");
                        builder.AppendLine("            {");
                        builder.AppendLine($"                throw new Tabular::TabularDataException(\"Unable to move to a field at index {fieldIndex}.\", reader.Position);");
                        builder.AppendLine("            }");
                    }

                    builder.AppendLine();

                    if (!commentCheckEmitted)
                    {
                        builder.AppendLine("            if (reader.FieldType is Tabular::TabularFieldType.Comment)");
                        builder.AppendLine("            {");
                        builder.AppendLine($"                return Tabular::TabularRecord<{recordSpec.TypeName}>.FromComment(context.ConsumeComments ? reader.GetString() : null);");
                        builder.AppendLine("            }");

                        commentCheckEmitted = true;
                    }

                    builder.AppendLine("        }");
                    builder.AppendLine();
                }

                if (!recordSpec.IsStrict)
                {
                    builder.AppendLine("        if (!await reader.ReadFieldAsync(cancellationToken).ConfigureAwait(false))");
                    builder.AppendLine("        {");
                    builder.AppendLine($"            return Tabular::TabularRecord<{recordSpec.TypeName}>.FromContent(new()");
                    builder.AppendLine("            {");

                    for (var j = 0; j < i; j++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var fieldIndexInner = fieldIndices[j];
                        var fieldSpecInner = recordSpec.FieldSpecs[fieldIndexInner];

                        if (!fieldSpecInner.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write))
                        {
                            continue;
                        }

                        builder.AppendLine($"                {fieldSpecInner.MemberName} = value{fieldIndexInner},");
                    }

                    builder.AppendLine("            });");
                    builder.AppendLine("        }");
                }
                else
                {
                    builder.AppendLine("        if (!await reader.ReadFieldAsync(cancellationToken).ConfigureAwait(false))");
                    builder.AppendLine("        {");
                    builder.AppendLine($"            throw new Tabular::TabularDataException(\"Unable to read a field at index {fieldIndex}.\", reader.Position);");
                    builder.AppendLine("        }");
                }

                builder.AppendLine();

                if (!commentCheckEmitted)
                {
                    builder.AppendLine("        if (reader.FieldType is Tabular::TabularFieldType.Comment)");
                    builder.AppendLine("        {");
                    builder.AppendLine($"            return Tabular::TabularRecord<{recordSpec.TypeName}>.FromComment(context.ConsumeComments ? reader.GetString() : null);");
                    builder.AppendLine("        }");
                    builder.AppendLine();

                    commentCheckEmitted = true;
                }

                if (!fieldSpec.MemberHasConverter)
                {
                    var memberTypeCode = GetTypeCode(fieldSpec.MemberTypeName) ?? $"<{fieldSpec.MemberTypeName}>";

                    if (!recordSpec.IsStrict)
                    {
                        if (fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsReferenceType) ||
                            fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsNullableValueType))
                        {
                            builder.AppendLine("        if (!reader.Value.IsEmpty)");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            if (reader.TryGet{memberTypeCode}(out var field{fieldIndex}))");
                            builder.AppendLine("            {");
                            builder.AppendLine($"                value{fieldIndex} = field{fieldIndex};");
                            builder.AppendLine("            }");
                            builder.AppendLine("        }");
                        }
                        else
                        {
                            builder.AppendLine($"        if (reader.TryGet{memberTypeCode}(out var field{fieldIndex}))");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            value{fieldIndex} = field{fieldIndex};");
                            builder.AppendLine("        }");
                        }
                    }
                    else
                    {
                        var memberTypeName = fieldSpec.MemberTypeName;

                        if (fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsReferenceType) ||
                            fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsNullableValueType))
                        {
                            builder.AppendLine("        if (!reader.Value.IsEmpty)");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            if (reader.TryGet{memberTypeCode}(out var field{fieldIndex}))");
                            builder.AppendLine("            {");
                            builder.AppendLine($"                value{fieldIndex} = field{fieldIndex};");
                            builder.AppendLine("            }");
                            builder.AppendLine("            else");
                            builder.AppendLine("            {");
                            builder.AppendLine($"                throw new Tabular::TabularDataException($\"Unable to get a field value at index {fieldIndex} of type '{{typeof({memberTypeName})}}'.\", reader.Position);");
                            builder.AppendLine("            }");
                            builder.AppendLine("        }");
                        }
                        else
                        {
                            builder.AppendLine($"        if (reader.TryGet{memberTypeCode}(out var field{fieldIndex}))");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            value{fieldIndex} = field{fieldIndex};");
                            builder.AppendLine("        }");
                            builder.AppendLine("        else");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            throw new Tabular::TabularDataException($\"Unable to get a field value at index {fieldIndex} of type '{{typeof({memberTypeName})}}'.\", reader.Position);");
                            builder.AppendLine("        }");
                        }
                    }
                }
                else
                {
                    if (!recordSpec.IsStrict)
                    {
                        if (fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsReferenceType) ||
                            fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsNullableValueType))
                        {
                            builder.AppendLine("        if (!reader.Value.IsEmpty)");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            if (reader.TryGet(converter{fieldIndex}, out var field{fieldIndex}))");
                            builder.AppendLine("            {");
                            builder.AppendLine($"                value{fieldIndex} = field{fieldIndex};");
                            builder.AppendLine("            }");
                            builder.AppendLine("        }");
                        }
                        else
                        {
                            builder.AppendLine($"        if (reader.TryGet(converter{fieldIndex}, out var field{fieldIndex}))");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            value{fieldIndex} = field{fieldIndex};");
                            builder.AppendLine("        }");
                        }
                    }
                    else
                    {
                        var memberTypeName = fieldSpec.MemberTypeName;

                        if (fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsReferenceType) ||
                            fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsNullableValueType))
                        {
                            builder.AppendLine("        if (!reader.Value.IsEmpty)");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            if (reader.TryGet(converter{fieldIndex}, out var field{fieldIndex}))");
                            builder.AppendLine("            {");
                            builder.AppendLine($"                value{fieldIndex} = field{fieldIndex};");
                            builder.AppendLine("            }");
                            builder.AppendLine("            else");
                            builder.AppendLine("            {");
                            builder.AppendLine($"                throw new Tabular::TabularDataException($\"Unable to get a field value at index {fieldIndex} of type '{{typeof({memberTypeName})}}'.\", reader.Position);");
                            builder.AppendLine("            }");
                            builder.AppendLine("        }");
                        }
                        else
                        {
                            builder.AppendLine($"        if (reader.TryGet(converter{fieldIndex}, out var field{fieldIndex}))");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            value{fieldIndex} = field{fieldIndex};");
                            builder.AppendLine("        }");
                            builder.AppendLine("        else");
                            builder.AppendLine("        {");
                            builder.AppendLine($"            throw new Tabular::TabularDataException($\"Unable to get a field value at index {fieldIndex} of type '{{typeof({memberTypeName})}}'.\", reader.Position);");
                            builder.AppendLine("        }");
                        }
                    }
                }

                builder.AppendLine();
            }

            builder.AppendLine($"        return Tabular::TabularRecord<{recordSpec.TypeName}>.FromContent(new()");
            builder.AppendLine("        {");

            for (var i = 0; i < fieldIndices.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fieldIndex = fieldIndices[i];
                var fieldSpec = recordSpec.FieldSpecs[fieldIndex];

                if (!fieldSpec.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write))
                {
                    continue;
                }

                builder.AppendLine($"            {fieldSpec.MemberName} = value{fieldIndex},");
            }

            builder.AppendLine("        });");
            builder.AppendLine("    }");
        }

        if (typeWritingFieldsCount > 0)
        {
            if (typeReadingFieldsCount > 0)
            {
                builder.AppendLine();
            }

            if (typeWritingFieldsCount > 1)
            {
                builder.AppendLine("    [global::System.Runtime.CompilerServices.AsyncMethodBuilder(typeof(global::System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder))]");
                builder.AppendLine($"    public override async global::System.Threading.Tasks.ValueTask WriteRecordAsync(Tabular::TabularFieldWriter writer, {recordSpec.TypeName} record, Tabular::TabularRecordWriterContext context, global::System.Threading.CancellationToken cancellationToken)");
            }
            else
            {
                builder.AppendLine($"    public override global::System.Threading.Tasks.ValueTask WriteRecordAsync(Tabular::TabularFieldWriter writer, {recordSpec.TypeName} record, Tabular::TabularRecordWriterContext context, global::System.Threading.CancellationToken cancellationToken)");
            }

            builder.AppendLine("    {");

            if (typeHasFieldConverters)
            {
                for (var i = 0; i < fieldIndices.Length; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var fieldIndex = fieldIndices[i];
                    var fieldSpec = recordSpec.FieldSpecs[fieldIndex];

                    if (!fieldSpec.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Write) || !fieldSpec.MemberHasConverter)
                    {
                        continue;
                    }

                    var converterTypeName = fieldSpec.MemberConverterTypeName;

                    builder.AppendLine($"        var converter{fieldIndex} = context.ConverterFactory.CreateFieldConverter<{converterTypeName}>();");
                }

                builder.AppendLine();
            }

            for (var i = 0; i < fieldIndices.Length; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fieldIndex = fieldIndices[i];
                var fieldSpec = recordSpec.FieldSpecs[fieldIndex];

                if (!fieldSpec.MemberAccessTypes.HasFlag(TypeMemberAccessTypes.Read))
                {
                    continue;
                }

                var memberTypeCode = GetTypeCode(fieldSpec.MemberTypeName);

                if (!fieldSpec.MemberHasConverter)
                {
                    if (fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsReferenceType) && (memberTypeCode != "String"))
                    {
                        builder.AppendLine($"        if (record.{fieldSpec.MemberName} is null)");
                        builder.AppendLine("        {");
                        builder.AppendLine("            writer.WriteString(null);");
                        builder.AppendLine("        }");
                        builder.AppendLine("        else");
                        builder.AppendLine("        {");
                        builder.AppendLine($"            writer.Write{memberTypeCode}(record.{fieldSpec.MemberName});");
                        builder.AppendLine("        }");
                    }
                    else if (fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsNullableValueType))
                    {
                        builder.AppendLine($"        if (record.{fieldSpec.MemberName} is null)");
                        builder.AppendLine("        {");
                        builder.AppendLine("            writer.WriteString(null);");
                        builder.AppendLine("        }");
                        builder.AppendLine("        else");
                        builder.AppendLine("        {");
                        builder.AppendLine($"            writer.Write{memberTypeCode}(record.{fieldSpec.MemberName}.Value);");
                        builder.AppendLine("        }");
                    }
                    else
                    {
                        builder.AppendLine($"        writer.Write{memberTypeCode}(record.{fieldSpec.MemberName});");
                    }
                }
                else
                {
                    if (fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsReferenceType) && (memberTypeCode != "String"))
                    {
                        builder.AppendLine($"        if (record.{fieldSpec.MemberName} is null)");
                        builder.AppendLine("        {");
                        builder.AppendLine("            writer.WriteString(null);");
                        builder.AppendLine("        }");
                        builder.AppendLine("        else");
                        builder.AppendLine("        {");
                        builder.AppendLine($"            writer.Write(record.{fieldSpec.MemberName}, converter{fieldIndex});");
                        builder.AppendLine("        }");
                    }
                    else if (fieldSpec.MemberTypeKinds.HasFlag(TypeKinds.IsNullableValueType))
                    {
                        builder.AppendLine($"        if (record.{fieldSpec.MemberName} is null)");
                        builder.AppendLine("        {");
                        builder.AppendLine("            writer.WriteString(null);");
                        builder.AppendLine("        }");
                        builder.AppendLine("        else");
                        builder.AppendLine("        {");
                        builder.AppendLine($"            writer.Write(record.{fieldSpec.MemberName}.Value, converter{fieldIndex});");
                        builder.AppendLine("        }");
                    }
                    else
                    {
                        builder.AppendLine($"        writer.Write(record.{fieldSpec.MemberName}, converter{fieldIndex});");
                    }
                }

                if (i < recordSpec.FieldSpecs.Count - 1)
                {
                    builder.AppendLine();
                    builder.AppendLine("        if (writer.UnflushedChars > context.FlushThreshold)");
                    builder.AppendLine("        {");
                    builder.AppendLine("            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);");
                    builder.AppendLine("        }");
                    builder.AppendLine();
                }
            }

            if (typeWritingFieldsCount == 1)
            {
                builder.AppendLine();
                builder.AppendLine("        return global::System.Threading.Tasks.ValueTask.CompletedTask;");
            }

            builder.AppendLine("    }");
        }

        builder.AppendLine("}");
    }
}
