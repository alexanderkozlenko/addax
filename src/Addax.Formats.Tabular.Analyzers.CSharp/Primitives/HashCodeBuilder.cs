// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Analyzers.CSharp.Primitives
{
    internal struct HashCodeBuilder
    {
        private const uint s_offsetBasis = 0x811c9dc5;
        private const uint s_prime = 0x01000193;

        private uint _hash;

        private HashCodeBuilder(uint offsetBasis)
        {
            _hash = offsetBasis;
        }

        public static HashCodeBuilder Create()
        {
            return new HashCodeBuilder(s_offsetBasis);
        }

        public void Add<T>(T value)
        {
            unchecked
            {
                var data = (uint)(value?.GetHashCode() ?? 0);

                for (var i = 0; i < sizeof(uint); i++)
                {
                    _hash = (_hash ^ (data & 0xff)) * s_prime;

                    data >>= 8;
                }
            }
        }

        public int ToHashCode()
        {
            unchecked
            {
                return (int)_hash;
            }
        }
    }
}
