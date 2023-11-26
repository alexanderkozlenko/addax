// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts binary data encoded with "base64" encoding from or to a sequence of characters.</summary>
public class TabularBase64BinaryConverter : TabularConverter<byte[]>
{
    internal static readonly TabularBase64BinaryConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularBase64BinaryConverter" /> class.</summary>
    public TabularBase64BinaryConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(byte[]? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return Convert.TryToBase64Chars(value, destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out byte[]? value)
    {
        var bufferSize = (int)Math.Ceiling(source.Length / 4.0) * 3;

        if ((uint)bufferSize <= 256)
        {
            var buffer = (Span<byte>)stackalloc byte[256];

            if (Convert.TryFromBase64Chars(source, buffer, out var bytesWritten))
            {
                var bufferUsed = buffer.Slice(0, bytesWritten);

                value = GC.AllocateUninitializedArray<byte>(bytesWritten);
                bufferUsed.CopyTo(value);

                return true;
            }
        }
        else
        {
            using var buffer = new ArrayBuffer<byte>(bufferSize);

            if (Convert.TryFromBase64Chars(source, buffer.AsSpan(), out var bytesWritten))
            {
                var bufferUsed = buffer.AsSpan(bytesWritten);

                value = GC.AllocateUninitializedArray<byte>(bytesWritten);
                bufferUsed.CopyTo(value);

                return true;
            }
        }

        value = default;

        return false;
    }
}
