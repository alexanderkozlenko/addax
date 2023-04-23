// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Text;

namespace Addax.Formats.Tabular.Internal;

internal static class Transcoder
{
    public static void Convert(Encoding encoding, Decoder decoder, in ReadOnlySequence<byte> bytes, IBufferWriter<char> writer, bool flush)
    {
        var reader = new SequenceReader<byte>(bytes);
        var completed = false;

        while (!reader.End)
        {
            var bytesFragment = reader.UnreadSpan[..Math.Min(reader.UnreadSpan.Length, 0x00100000)];
            var charsBuffer = writer.GetSpan(encoding.GetMaxCharCount(bytesFragment.Length));

            decoder.Convert(bytesFragment, charsBuffer, flush: false, out var bytesUsed, out var charsUsed, out completed);
            reader.Advance(bytesUsed);
            writer.Advance(charsUsed);
        }

        if (!completed && flush)
        {
            var charsBuffer = writer.GetSpan(encoding.GetMaxCharCount(byteCount: 0));

            decoder.Convert(bytes: ReadOnlySpan<byte>.Empty, charsBuffer, flush: true, out _, out var charsUsed, out completed);

            Debug.Assert(completed);

            writer.Advance(charsUsed);
        }
    }

    public static void Convert(Encoding encoding, Encoder encoder, in ReadOnlySequence<char> chars, IBufferWriter<byte> writer, bool flush)
    {
        var reader = new SequenceReader<char>(chars);
        var completed = false;

        while (!reader.End)
        {
            var charsFragment = reader.UnreadSpan[..Math.Min(reader.UnreadSpan.Length, 0x00100000)];
            var bytesBuffer = writer.GetSpan(encoding.GetMaxByteCount(charsFragment.Length));

            encoder.Convert(charsFragment, bytesBuffer, flush: false, out var charsUsed, out var bytesUsed, out completed);
            reader.Advance(charsUsed);
            writer.Advance(bytesUsed);
        }

        if (!completed && flush)
        {
            var bytesBuffer = writer.GetSpan(encoding.GetMaxByteCount(charCount: 0));

            encoder.Convert(chars: ReadOnlySpan<char>.Empty, bytesBuffer, flush: true, out _, out var bytesUsed, out completed);

            Debug.Assert(completed);

            writer.Advance(bytesUsed);
        }
    }
}
