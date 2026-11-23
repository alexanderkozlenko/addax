// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularRecordParseResult : IEquatable<TabularRecordParseResult>
    {
        public readonly TabularRecordMapping? RecordMapping;
        public readonly ImmutableArray<Diagnostic> Diagnostics;

        public TabularRecordParseResult(
            TabularRecordMapping? recordMapping,
            ImmutableArray<Diagnostic> diagnostics)
        {
            Debug.Assert(!diagnostics.IsDefault);

            RecordMapping = recordMapping;
            Diagnostics = diagnostics;
        }

        public bool Equals(TabularRecordParseResult other)
        {
            return
                (RecordMapping == other.RecordMapping) &&
                (Diagnostics == other.Diagnostics);
        }

        public override bool Equals(object obj)
        {
            return (obj is TabularRecordParseResult other) && Equals(other);
        }

        public override int GetHashCode()
        {
            var builder = HashCodeBuilder.Create();

            builder.Add(RecordMapping);
            builder.Add(Diagnostics);

            return builder.ToHashCode();
        }
    }
}
