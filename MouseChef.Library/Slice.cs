using System;
using System.Collections;
using System.Collections.Generic;

namespace MouseChef
{
    /// <summary>
    /// Provides a view into a sub-range of another list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Slice<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> _source;
        private readonly int _start;
        private readonly int _count;

        public Slice(IReadOnlyList<T> source, int start, int count)
        {
            _source = source;
            _start = start;
            _count = Math.Min(count, _source.Count - start);
        }

        private class Enumerator : IEnumerator<T>
        {
            private readonly Slice<T> _parent;
            private int _index;

            public Enumerator(Slice<T> parent, int index)
            {
                _parent = parent;
                _index = index;
            }

            public void Dispose() { }

            public bool MoveNext() => ++_index < _parent.Count;

            public void Reset() => _index = 0;

            public T Current => _parent[_index];

            object IEnumerator.Current => Current;
        }

        public IEnumerator<T> GetEnumerator() => new Enumerator(this, index: 0);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _count;
        public T this[int index] => _source[_start + index];
    }

    public static class SliceExtensions
    {
        public static IReadOnlyList<T> Slice<T>(this IReadOnlyList<T> source, int start = 0, int count = int.MaxValue)
            => new Slice<T>(source, start, count);
    }
}
