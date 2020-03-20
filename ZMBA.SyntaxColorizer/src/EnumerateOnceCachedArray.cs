using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections;

namespace ZMBA.SyntaxColorizer {
  public class EnumerateOnceCachedArray<T> : IEnumerable<T> {
    private static ConcurrentQueue<EnumerateOnceCachedArray<T>> _cache = new ConcurrentQueue<EnumerateOnceCachedArray<T>>();

    public static EnumerateOnceCachedArray<T> Take(int suggested_capacity = 512) {
      if (_cache.TryDequeue(out var inst)) {
        inst._disposed = false;
        return inst; 
      }
      return new EnumerateOnceCachedArray<T>(suggested_capacity);
    }

    public static void Return(ref EnumerateOnceCachedArray<T> inst) {
      inst.Clear();
      inst._disposed = true;
      _cache.Enqueue(inst);
      inst = null;
    }

    private T[] _items;
    private int _size;
    private bool _disposed = false;

    public int Count => _size;

    public EnumerateOnceCachedArray(int capacity) {
      _items = new T[capacity];
      _size = 0;
    }

    public void Add(T item) {
      if (_size == _items.Length) { Grow(_items.Length + 1);  }
      _items[_size++] = item;
    }

    public void Clear() {
      if (_size > 0) {
        Array.Clear(_items, 0, _size);
        _size = 0;
      }
    }

    private void Grow(int minimum) {
      int value = _items.Length * 2;
      T[] array = new T[value];
      Array.Copy(_items, 0, array, 0, _size);
      _items = array;
    }

    public IEnumerator<T> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator {
      private EnumerateOnceCachedArray<T> list;
      private int index;
      private T current;

      public T Current => this.current; 
      object IEnumerator.Current => this.current;

      internal Enumerator(EnumerateOnceCachedArray<T> list) {
        this.list = list;
        index = 0;
        current = default;
      }
    
      public void Dispose() {
        EnumerateOnceCachedArray<T>.Return(ref list);
      }
    
      public bool MoveNext() {
        if (index < list._size) {
          current = list._items[index];
          index++;
          return true;
        }
        return false;
      }

      void IEnumerator.Reset() {
        index = 0;
        current = default(T);
      }
    }
  }
}
