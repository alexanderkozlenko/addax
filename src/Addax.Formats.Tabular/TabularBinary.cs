// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular;

internal static class TabularBinary
{
    public static bool TryFormatBase16(ReadOnlySpan<byte> value, Span<char> destination, out int charsWritten)
    {
        if (value.IsEmpty)
        {
            charsWritten = 0;

            return true;
        }

        if (destination.Length >= value.Length * 2)
        {
            var provider = CultureInfo.InvariantCulture;

            for (var i = 0; i < value.Length; i++)
            {
                value[i].TryFormat(destination, out _, "x2", provider);
                destination = destination.Slice(2);
            }

            charsWritten = value.Length * 2;

            return true;
        }

        charsWritten = 0;

        return false;
    }

    public static bool TryFormatBase64(ReadOnlySpan<byte> value, Span<char> destination, out int charsWritten)
    {
        return Convert.TryToBase64Chars(value, destination, out charsWritten);
    }

    public static bool TryParseBase16(ReadOnlySpan<char> source, [NotNullWhen(true)] out byte[]? value)
    {
        source = source.Trim();

        if (source.Length % 2 == 0)
        {
            try
            {
                value = Convert.FromHexString(source);

                return true;
            }
            catch (FormatException)
            {
            }
        }

        value = default;

        return false;
    }

    public static bool TryParseBase64(ReadOnlySpan<char> source, [NotNullWhen(true)] out byte[]? value)
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
