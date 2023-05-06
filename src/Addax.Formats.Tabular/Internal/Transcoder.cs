// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Text;

namespace Addax.Formats.Tabular.Internal;

internal static class Transcoder
{
    public static void Convert(Encoding encoding, Decoder decoder, ReadOnlySpan<byte> bytes, IBufferWriter<char> writer)
    {
        while (!bytes.IsEmpty)
        {
            var bytesFragment = bytes[..Math.Min(bytes.Length, 0x00100000)];
            var charsBuffer = writer.GetSpan(encoding.GetMaxCharCount(bytesFragment.Length));

            decoder.Convert(bytesFragment, charsBuffer, flush: false, out var bytesUsed, out var charsUsed, out _);
            bytes = bytes[bytesUsed..];
            writer.Advance(charsUsed);
        }
    }

    public static void Flush(Encoding encoding, Decoder decoder, IBufferWriter<char> writer)
    {
        var charsBuffer = writer.GetSpan(encoding.GetMaxCharCount(byteCount: 0));

        decoder.Convert(bytes: ReadOnlySpan<byte>.Empty, charsBuffer, flush: true, out _, out var charsUsed, out var completed);

        Debug.Assert(completed);

        writer.Advance(charsUsed);
    }

    public static void Convert(Encoding encoding, Encoder encoder, ReadOnlySpan<char> chars, IBufferWriter<byte> writer)
    {
        while (!chars.IsEmpty)
        {
            var charsFragment = chars[..Math.Min(chars.Length, 0x00100000)];
            var bytesBuffer = writer.GetSpan(encoding.GetMaxByteCount(charsFragment.Length));

            encoder.Convert(charsFragment, bytesBuffer, flush: false, out var charsUsed, out var bytesUsed, out _);
            chars = chars[charsUsed..];
            writer.Advance(bytesUsed);
        }
    }

    public static void Flush(Encoding encoding, Encoder encoder, IBufferWriter<byte> writer)
    {
        var bytesBuffer = writer.GetSpan(encoding.GetMaxByteCount(charCount: 0));

        encoder.Convert(chars: ReadOnlySpan<char>.Empty, bytesBuffer, flush: true, out _, out var bytesUsed, out var completed);

        Debug.Assert(completed);

        writer.Advance(bytesUsed);
    }
}
