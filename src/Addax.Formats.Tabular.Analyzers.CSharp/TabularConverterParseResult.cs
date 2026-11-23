// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Addax.Formats.Tabular.Analyzers.CSharp.Primitives;
using Microsoft.CodeAnalysis;

namespace Addax.Formats.Tabular.Analyzers.CSharp
{
    internal readonly struct TabularConverterParseResult : IEquatable<TabularConverterParseResult>
    {
        public readonly EquatableArray<TabularConverterMapping> ConverterMappings;
        public readonly ImmutableArray<Diagnostic> Diagnostics;

        public TabularConverterParseResult(
            EquatableArray<TabularConverterMapping> converterMappings,
            ImmutableArray<Diagnostic> diagnostics)
        {
            Debug.Assert(!diagnostics.IsDefault);

            ConverterMappings = converterMappings;
            Diagnostics = diagnostics;
        }

        public bool Equals(TabularConverterParseResult other)
        {
            return
                (ConverterMappings == other.ConverterMappings) &&
                (Diagnostics == other.Diagnostics);
        }

        public override bool Equals(object obj)
        {
            return (obj is TabularConverterParseResult other) && Equals(other);
        }

        public override int GetHashCode()
        {
            var builder = HashCodeBuilder.Create();

            builder.Add(ConverterMappings);
            builder.Add(Diagnostics);

            return builder.ToHashCode();
        }
    }
}
