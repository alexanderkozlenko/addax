// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Numerics;
using System.Text;

namespace Addax.Formats.Tabular;

internal partial class TabularConverterResolver
{
    private TabularConverter<BFloat16>? _bfloat16;
    private TabularConverter<BigInteger>? _bigInteger;
    private TabularConverter<bool>? _boolean;
    private TabularConverter<byte>? _byte;
    private TabularConverter<char>? _char;
    private TabularConverter<DateOnly>? _dateOnly;
    private TabularConverter<DateTime>? _dateTime;
    private TabularConverter<DateTimeOffset>? _dateTimeOffset;
    private TabularConverter<decimal>? _decimal;
    private TabularConverter<Decimal32>? _decimal32;
    private TabularConverter<Decimal64>? _decimal64;
    private TabularConverter<Decimal128>? _decimal128;
    private TabularConverter<double>? _double;
    private TabularConverter<Guid>? _guid;
    private TabularConverter<Half>? _half;
    private TabularConverter<short>? _int16;
    private TabularConverter<int>? _int32;
    private TabularConverter<long>? _int64;
    private TabularConverter<Int128>? _int128;
    private TabularConverter<Rune>? _rune;
    private TabularConverter<sbyte>? _sbyte;
    private TabularConverter<float>? _single;
    private TabularConverter<TimeOnly>? _timeOnly;
    private TabularConverter<TimeSpan>? _timeSpan;
    private TabularConverter<ushort>? _uint16;
    private TabularConverter<uint>? _uint32;
    private TabularConverter<ulong>? _uint64;
    private TabularConverter<UInt128>? _uint128;
    private TabularConverter<Uri>? _uri;

    public TabularConverter<BFloat16> BFloat16
    {
        get
        {
            return _bfloat16 ?? Resolve(ref _bfloat16);
        }
    }

    public TabularConverter<BigInteger> BigInteger
    {
        get
        {
            return _bigInteger ?? Resolve(ref _bigInteger);
        }
    }

    public TabularConverter<bool> Boolean
    {
        get
        {
            return _boolean ?? Resolve(ref _boolean);
        }
    }

    public TabularConverter<byte> Byte
    {
        get
        {
            return _byte ?? Resolve(ref _byte);
        }
    }

    public TabularConverter<char> Char
    {
        get
        {
            return _char ?? Resolve(ref _char);
        }
    }

    public TabularConverter<DateOnly> DateOnly
    {
        get
        {
            return _dateOnly ?? Resolve(ref _dateOnly);
        }
    }

    public TabularConverter<DateTime> DateTime
    {
        get
        {
            return _dateTime ?? Resolve(ref _dateTime);
        }
    }

    public TabularConverter<DateTimeOffset> DateTimeOffset
    {
        get
        {
            return _dateTimeOffset ?? Resolve(ref _dateTimeOffset);
        }
    }

    public TabularConverter<decimal> Decimal
    {
        get
        {
            return _decimal ?? Resolve(ref _decimal);
        }
    }

    public TabularConverter<Decimal32> Decimal32
    {
        get
        {
            return _decimal32 ?? Resolve(ref _decimal32);
        }
    }

    public TabularConverter<Decimal64> Decimal64
    {
        get
        {
            return _decimal64 ?? Resolve(ref _decimal64);
        }
    }

    public TabularConverter<Decimal128> Decimal128
    {
        get
        {
            return _decimal128 ?? Resolve(ref _decimal128);
        }
    }

    public TabularConverter<double> Double
    {
        get
        {
            return _double ?? Resolve(ref _double);
        }
    }

    public TabularConverter<Guid> Guid
    {
        get
        {
            return _guid ?? Resolve(ref _guid);
        }
    }

    public TabularConverter<Half> Half
    {
        get
        {
            return _half ?? Resolve(ref _half);
        }
    }

    public TabularConverter<short> Int16
    {
        get
        {
            return _int16 ?? Resolve(ref _int16);
        }
    }

    public TabularConverter<int> Int32
    {
        get
        {
            return _int32 ?? Resolve(ref _int32);
        }
    }

    public TabularConverter<long> Int64
    {
        get
        {
            return _int64 ?? Resolve(ref _int64);
        }
    }

    public TabularConverter<Int128> Int128
    {
        get
        {
            return _int128 ?? Resolve(ref _int128);
        }
    }

    public TabularConverter<Rune> Rune
    {
        get
        {
            return _rune ?? Resolve(ref _rune);
        }
    }

    public TabularConverter<sbyte> SByte
    {
        get
        {
            return _sbyte ?? Resolve(ref _sbyte);
        }
    }

    public TabularConverter<float> Single
    {
        get
        {
            return _single ?? Resolve(ref _single);
        }
    }

    public TabularConverter<TimeOnly> TimeOnly
    {
        get
        {
            return _timeOnly ?? Resolve(ref _timeOnly);
        }
    }

    public TabularConverter<TimeSpan> TimeSpan
    {
        get
        {
            return _timeSpan ?? Resolve(ref _timeSpan);
        }
    }

    public TabularConverter<ushort> UInt16
    {
        get
        {
            return _uint16 ?? Resolve(ref _uint16);
        }
    }

    public TabularConverter<uint> UInt32
    {
        get
        {
            return _uint32 ?? Resolve(ref _uint32);
        }
    }

    public TabularConverter<ulong> UInt64
    {
        get
        {
            return _uint64 ?? Resolve(ref _uint64);
        }
    }

    public TabularConverter<UInt128> UInt128
    {
        get
        {
            return _uint128 ?? Resolve(ref _uint128);
        }
    }

    public TabularConverter<Uri> Uri
    {
        get
        {
            return _uri ?? Resolve(ref _uri);
        }
    }
}
