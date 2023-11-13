// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;

namespace Addax.Formats.Tabular.Buffers;

internal readonly ref struct RegionWriter<T>
{
    private readonly Span<T> _target;
    private readonly ref int _offset;

    public RegionWriter(Span<T> target, ref int offset)
    {
        _target = target;
        _offset = ref offset;
    }

    public bool TryWrite(T value)
    {
        Debug.Assert(_offset >= 0);

        if (_offset < _target.Length)
        {
            _target[_offset++] = value;

            return true;
        }

        return false;
    }

    public Span<T> UnusedRegion
    {
        get
        {
            return _target.Slice(_offset);
        }
    }
}
