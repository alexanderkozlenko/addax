// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Runtime.InteropServices;

namespace Addax.Formats.Tabular;

public partial class TabularStringFactory
{
    private sealed partial class StringTable
    {
        // The implementation is based on System.Collections.Generic.HashSet<T>.

        private int[] _buckets = new int[3];
        private Entry[] _entries = new Entry[3];
        private int _count;
        private int _freeList = -1;
        private int _freeCount;

        public unsafe string GetOrAdd(ReadOnlySpan<char> buffer, delegate*<ReadOnlySpan<char>, string> factory)
        {
            var buckets = _buckets;
            var entries = _entries;
            var collisionCount = 0U;
            var hashCode = GetHashCode(buffer);

            ref var bucket = ref buckets[(uint)hashCode % (uint)buckets.Length];

            var i = bucket - 1;

            while (i >= 0)
            {
                ref var entry = ref entries[i];

                if ((entry.HashCode == hashCode) && Equals(entry.Value, buffer))
                {
                    return entry.Value;
                }

                i = entry.Next;
                collisionCount += 1;

                if (collisionCount > (uint)entries.Length)
                {
                    break;
                }
            }

            var value = factory(buffer);
            var index = 0;

            if (_freeCount > 0)
            {
                index = _freeList;
                _freeCount -= 1;
                _freeList = -3 - entries[_freeList].Next;
            }
            else
            {
                var count = _count;

                if (count == entries.Length)
                {
                    Resize();

                    buckets = _buckets;
                    entries = _entries;
                    bucket = ref buckets[(uint)hashCode % (uint)buckets.Length];
                }

                index = count;
                _count = count + 1;
                entries = _entries;
            }

            {
                ref var entry = ref entries[index];

                entry.HashCode = hashCode;
                entry.Next = bucket - 1;
                entry.Value = value;
                bucket = index + 1;
            }

            return value;
        }

        public unsafe string GetOrAdd(in ReadOnlySequence<char> buffer, delegate*<in ReadOnlySequence<char>, string> factory)
        {
            var buckets = _buckets;
            var entries = _entries;
            var collisionCount = 0U;
            var hashCode = GetHashCode(buffer);

            ref var bucket = ref buckets[(uint)hashCode % (uint)buckets.Length];

            var i = bucket - 1;

            while (i >= 0)
            {
                ref var entry = ref entries[i];

                if ((entry.HashCode == hashCode) && Equals(entry.Value, buffer))
                {
                    return entry.Value;
                }

                i = entry.Next;
                collisionCount += 1;

                if (collisionCount > (uint)entries.Length)
                {
                    break;
                }
            }

            var value = factory(buffer);
            var index = 0;

            if (_freeCount > 0)
            {
                index = _freeList;
                _freeCount -= 1;
                _freeList = -3 - entries[_freeList].Next;
            }
            else
            {
                var count = _count;

                if (count == entries.Length)
                {
                    Resize();

                    buckets = _buckets;
                    entries = _entries;
                    bucket = ref buckets[(uint)hashCode % (uint)buckets.Length];
                }

                index = count;
                _count = count + 1;
                entries = _entries;
            }

            {
                ref var entry = ref entries[index];

                entry.HashCode = hashCode;
                entry.Next = bucket - 1;
                entry.Value = value;
                bucket = index + 1;
            }

            return value;
        }

        private void Resize()
        {
            var count = _count;
            var size = GetSize(count);
            var buckets = new int[size];
            var entries = new Entry[size];

            Array.Copy(_entries!, entries, count);

            for (var i = 0; i < count; i++)
            {
                ref var entry = ref entries[i];

                if (entry.Next >= -1)
                {
                    ref var bucket = ref buckets[(uint)entry.HashCode % (uint)size];

                    entry.Next = bucket - 1;
                    bucket = i + 1;
                }
            }

            _buckets = buckets;
            _entries = entries;
        }

        public void Clear()
        {
            var count = _count;

            if (count is not 0)
            {
                Array.Clear(_buckets!);
                Array.Clear(_entries!, 0, count);

                _count = 0;
                _freeList = -1;
                _freeCount = 0;
            }
        }

        private static bool Equals(string value, ReadOnlySpan<char> buffer)
        {
            return value.AsSpan().SequenceEqual(buffer);
        }

        private static bool Equals(string value, in ReadOnlySequence<char> buffer)
        {
            if (value.Length != buffer.Length)
            {
                return false;
            }

            var valueSpan = value.AsSpan();

            foreach (var segment in buffer)
            {
                if (!valueSpan[..segment.Length].SequenceEqual(segment.Span))
                {
                    return false;
                }

                valueSpan = valueSpan[segment.Length..];
            }

            return true;
        }

        private static int GetHashCode(ReadOnlySpan<char> buffer)
        {
            var hashCode = new HashCode();

            hashCode.AddBytes(MemoryMarshal.Cast<char, byte>(buffer));

            return hashCode.ToHashCode();
        }

        private static int GetHashCode(in ReadOnlySequence<char> buffer)
        {
            var hashCode = new HashCode();

            foreach (var segment in buffer)
            {
                hashCode.AddBytes(MemoryMarshal.Cast<char, byte>(segment.Span));
            }

            return hashCode.ToHashCode();
        }

        private static int GetSize(int count)
        {
            var size = 2 * count;

            if (((uint)size > 0x7fffffc3) && (count < 0x7fffffc3))
            {
                return 0x7fffffc3;
            }

            for (var i = size | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i) && (((i - 1) % 101) is not 0))
                {
                    return i;
                }
            }

            return size;

            static bool IsPrime(int value)
            {
                if ((value & 1) is 0)
                {
                    return value is 2;
                }

                var maximum = (int)Math.Sqrt(value);

                for (var i = 3; i <= maximum; i += 2)
                {
                    if ((value % i) is 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
