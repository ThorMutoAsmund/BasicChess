using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessEngines
{
    //public class Zobrist<T>
    //{
    //    public UInt64 Key { get; set; }
    //    public double Value { get; set; }

    //    public override int GetHashCode()
    //    {
    //        return (int)this.Key;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        return (obj as Zobrist<T>).Key == this.Key;
    //    }
    //}

    public class ZobristHashTree<T> where T: class
    {
        private UInt64[] keys;
        private T[] values;
        private UInt64 size;

        public ZobristHashTree(UInt64 arraySize = 500000)
        {
            this.size = arraySize;
            this.keys = new UInt64[arraySize];
            this.values = new T[arraySize];
            for (UInt64 i = 0; i < arraySize; ++i)
            {
                this.values[i] = default(T);
            }
        }

        private UInt64 GetIndexHash(UInt64 hash)
        {
            return hash % this.size;
        }

        public bool TryGetValue(UInt64 key, out T value)
        {
            var indexHash = GetIndexHash(key);
            if (this.values[indexHash] == default(T) || this.keys[indexHash] != key)
            {
                value = default(T);
                return false;
            }
            else
            {
                value = this.values[indexHash];
                return true;
            }
        }

        public void SetValue(UInt64 key, T value)
        {
            var indexHash = GetIndexHash(key);
            this.keys[indexHash] = key;
            this.values[indexHash] = value;
        }


        public void Remove(UInt64 key)
        {
            var indexHash = GetIndexHash(key);
            this.values[indexHash] = default(T);
        }
        /*
        public T this[UInt64 key]
        {
            get
            {
                lock (this)
                {
                    T value = default(T);
                    if (this.TryGetValue(key, out value))
                    {
                        return value;
                    }
                }
                return default(T);
            }
            set
            {
                lock (this)
                {
                    this.Add(key, value);
                }
            }
        }

        public bool ContainsKey(UInt64 key)
        {
            return this.Contains(new Zobrist<T>() { Key = key });
        }*/
    }

}
