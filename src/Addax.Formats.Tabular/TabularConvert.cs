// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular;

internal static class TabularConvert
{
    public static bool TryParseBase16(ReadOnlySpan<char> source, [NotNullWhen(true)] out byte[]? value)
    {
        if (source.Length % 2 == 0)
        {
            var buffer = GC.AllocateUninitializedArray<byte>(source.Length / 2);

            if (Convert.FromHexString(source, buffer, out _, out _) == OperationStatus.Done)
            {
                value = buffer;

                return true;
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
