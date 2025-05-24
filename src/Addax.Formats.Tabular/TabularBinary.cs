// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular;

internal static class TabularBinary
{
    public static bool TryFormatBase16(ReadOnlySpan<byte> value, Span<char> destination, out int charsWritten)
    {
        if (destination.Length > value.Length * 2)
        {
            // BUG: .NET 9.0.0 requires 'destination.Length == value.Length * 2'

            destination = destination.Slice(0, value.Length * 2);
        }

        return Convert.TryToHexStringLower(value, destination, out charsWritten);
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
            var bufferSize = source.Length / 2;

            if ((uint)bufferSize <= 256)
            {
                var buffer = (stackalloc byte[256]);

                if (Convert.FromHexString(source, buffer, out _, out var bytesWritten) == OperationStatus.Done)
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

                if (Convert.FromHexString(source, buffer.AsSpan(), out _, out var bytesWritten) == OperationStatus.Done)
                {
                    var bufferUsed = buffer.AsSpan(bytesWritten);

                    value = GC.AllocateUninitializedArray<byte>(bytesWritten);
                    bufferUsed.CopyTo(value);

                    return true;
                }
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
            var buffer = (stackalloc byte[256]);

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
