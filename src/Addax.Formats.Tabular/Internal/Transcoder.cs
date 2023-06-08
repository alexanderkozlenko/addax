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
            var byteCount = Math.Min(bytes.Length, 0x00100000);
            var byteFragment = bytes[..byteCount];
            var charCount = encoding.GetMaxCharCount(byteFragment.Length);
            var charBuffer = writer.GetSpan(charCount);

            decoder.Convert(byteFragment, charBuffer, flush: false, out var bytesUsed, out var charsUsed, out _);
            bytes = bytes[bytesUsed..];
            writer.Advance(charsUsed);
        }
    }

    public static void Flush(Encoding encoding, Decoder decoder, IBufferWriter<char> writer)
    {
        var byteFragment = ReadOnlySpan<byte>.Empty;
        var charCount = encoding.GetMaxCharCount(byteCount: 0);
        var charBuffer = writer.GetSpan();

        decoder.Convert(byteFragment, charBuffer, flush: true, out _, out var charsUsed, out var completed);
        writer.Advance(charsUsed);

        Debug.Assert(completed);
    }

    public static void Convert(Encoding encoding, Encoder encoder, ReadOnlySpan<char> chars, IBufferWriter<byte> writer)
    {
        while (!chars.IsEmpty)
        {
            var charCount = Math.Min(chars.Length, 0x00100000);
            var charFragment = chars[..charCount];
            var byteCount = encoding.GetMaxByteCount(charFragment.Length);
            var byteBuffer = writer.GetSpan(byteCount);

            encoder.Convert(charFragment, byteBuffer, flush: false, out var charsUsed, out var bytesUsed, out _);
            chars = chars[charsUsed..];
            writer.Advance(bytesUsed);
        }
    }

    public static void Flush(Encoding encoding, Encoder encoder, IBufferWriter<byte> writer)
    {
        var charFragment = ReadOnlySpan<char>.Empty;
        var byteCount = encoding.GetMaxByteCount(charCount: 0);
        var byteBuffer = writer.GetSpan(byteCount);

        encoder.Convert(charFragment, byteBuffer, flush: true, out _, out var bytesUsed, out var completed);
        writer.Advance(bytesUsed);

        Debug.Assert(completed);
    }
}
