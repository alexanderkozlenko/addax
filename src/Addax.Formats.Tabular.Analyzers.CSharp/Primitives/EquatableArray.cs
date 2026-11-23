// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Addax.Formats.Tabular.Analyzers.CSharp.Primitives
{
    internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IEnumerable<T>
        where T : IEquatable<T>
    {
        private readonly ImmutableArray<T> _items;

        public EquatableArray(ImmutableArray<T> items)
        {
            _items = items;
        }

        public bool Equals(EquatableArray<T> other)
        {
            var items1 = _items.AsSpan();
            var items2 = other._items.AsSpan();

            if (items1.Length != items2.Length)
            {
                return false;
            }

            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < items1.Length; i++)
            {
                if (!comparer.Equals(items1[i], items2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return (obj is EquatableArray<T> other) && Equals(other);
        }

        public override int GetHashCode()
        {
            var builder = HashCodeBuilder.Create();

            foreach (var item in _items.AsSpan())
            {
                builder.Add(item);
            }

            return builder.ToHashCode();
        }

        public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right)
        {
            return !left.Equals(right);
        }

        public ImmutableArray<T>.Enumerator GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public T this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        public int Count
        {
            get
            {
                return _items.Length;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _items.IsEmpty;
            }
        }
    }
}
