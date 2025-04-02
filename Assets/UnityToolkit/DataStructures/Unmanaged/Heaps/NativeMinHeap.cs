using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Rendering;

namespace GalacticBoundStudios.DataScribes.Unmanaged
{
    [BurstCompile]
    public struct NativeMinHeap<TKey, TValue> : System.IDisposable, System.Collections.IEnumerable, System.Collections.Generic.IEnumerable<KeyValuePair<TKey,TValue>> where TKey : unmanaged, System.IComparable<TKey> where TValue : unmanaged
    {
        public struct BuildTreeJob : IJob
        {
            public NativeMinHeap<TKey, TValue> heap;

            public void Execute()
            {
                heap.Rebuild();
            }
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
        {
            private NativeMinHeap<TKey, TValue> minHeap;
            private NativeMinHeap<TKey, TValue> queue;

            private KeyValuePair<TKey, TValue> current;

            public KeyValuePair<TKey, TValue> Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return current;
                }
            }

            object IEnumerator.Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return Current;
                }
            }

            public Enumerator(ref NativeMinHeap<TKey, TValue> minHeap)
            {
                queue = new NativeMinHeap<TKey, TValue>(Allocator.Persistent, false, minHeap.Count);
                for (int i = 0; i < minHeap.Count; i++)
                {
                    queue.Add(minHeap.keys[i], minHeap.values[i]);
                }

                this.minHeap = minHeap;
                current = default;
            }

            public void Dispose()
            {
                queue.Dispose();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe bool MoveNext()
            {
                if (queue.IsEmpty)
                {
                    current = default;
                    return false;
                }
                else
                {
                    (TKey key, TValue value) = queue.PopMin();
                    current = new KeyValuePair<TKey, TValue>(key, value);
                    return true;
                }
            }

            public void Reset()
            {
                queue.Clear();
                for (int i = 0; i < minHeap.Count; i++)
                {
                    queue.Add(minHeap.keys[i], minHeap.values[i], true);
                }
            }
        }

        public bool IsEmpty => count == 0;
        public bool IsFull => count == values.Length;
        public int Count => count;

        private int count;
        private NativeArray<TKey> keys;
        private NativeArray<TValue> values;
        // This flag determines if the heap can grow when at capacity
        private bool canGrow;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
        internal DisposeSentinel m_DisposeSentinel;
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS

        public NativeMinHeap(Allocator allocator, bool canGrow=true, int capacity=16)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (allocator <= Allocator.None)
            {
                throw new System.ArgumentException("Allocator must be Temp, TempJob, or Persistent.", nameof(allocator));
            }

            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 1, allocator);
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS

            this.canGrow = canGrow;

            count = 0;
            keys = new NativeArray<TKey>(capacity, allocator);
            values = new NativeArray<TValue>(capacity, allocator);
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS
            keys.Dispose();
            values.Dispose();
        }

        // This will make an internal copy of the provided arrays
        public void SetPoints(NativeArray<TKey> keys, NativeArray<TValue> values, bool buildNow=true)
        {
            if (keys.Length != values.Length)
            {
                throw new System.ArgumentException("Keys and values must have the same length.");
            }

            keys.ResizeArray(keys.Length);
            values.ResizeArray(values.Length);
            count = keys.Length;

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i];
                values[i] = values[i];
            }

            if (buildNow)
            {
                Rebuild();
            }
        }

        public void Add(TKey key, TValue value, bool buildNow=true)
        {
            if (count == keys.Length)
            {
                if (canGrow)
                {
                    keys.ResizeArray(keys.Length * 2);
                    values.ResizeArray(values.Length * 2);
                }
                else
                {
                    // Drop the smallest item
                    if (key.CompareTo(keys[0]) > 0)
                    {
                        keys[0] = key;
                        values[0] = value;
                        Heapify(0);
                    }
                    return;
                }
            }

            keys[count] = key;
            values[count] = value;
            count++;

            // Rebuild the heap
            if (buildNow)
            {
                Rebuild();
            }
        }

        public void Rebuild()
        {
            for (int i = HeapUtils.Parent(count - 1); i >= 0; i--)
            {
                Heapify(i);
            }
        }

        public (TKey, TValue) PeekMin()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty.");
            }

            return (keys[0], values[0]);
        }

        public (TKey, TValue) PopMin()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty.");
            }

            TKey minKey = keys[0];
            TValue minValue = values[0];

            keys[0] = keys[count - 1];
            values[0] = values[count - 1];
            count--;

            Heapify(0);

            return (minKey, minValue);
        }

        public void Clear(bool removeArrays=true)
        {
            for (int i = 0; i < count; i++)
            {
                keys[i] = default;
                values[i] = default;
            }

            count = 0;
        }

        public bool Contains(TKey key)
        {
            if (IsEmpty)
            {
                return false;
            }

            NativeQueue<int> toCheck = new NativeQueue<int>(Allocator.Persistent);
            toCheck.Enqueue(0);

            while (toCheck.Count > 0)
            {
                int index = toCheck.Dequeue();

                if (key.Equals(keys[index]))
                {
                    toCheck.Dispose();
                    return true;
                }

                int left = HeapUtils.LeftChild(index);
                int right = HeapUtils.RightChild(index);

                if (left < count && keys[left].CompareTo(key) <= 0)
                {
                    toCheck.Enqueue(left);
                }
                if (right < count && keys[right].CompareTo(key) <= 0)
                {
                    toCheck.Enqueue(right);
                }
            }

            toCheck.Dispose();
            return false;
        }

        public (TKey, TValue) MaxItem()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty.");
            }

            int maxIndex = 0;

            // for (int i = 1; i < count; i++)
            // {
            //     if (keys[i].CompareTo(keys[minIndex]) < 0)
            //     {
            //         minIndex = i;
            //     }
            // }

            for (int i = count / 2; i < count; i++)
            {
                if (keys[i].CompareTo(keys[maxIndex]) > 0)
                {
                    maxIndex = i;
                }
            }

            return (keys[maxIndex], values[maxIndex]);
        }

        public (TKey, TValue) TakeMax()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty.");
            }

            int maxIndex = 0;
            for (int i = 0; i < count; i++)
            {
                if (keys[i].CompareTo(keys[maxIndex]) > 0)
                {
                    maxIndex = i;
                }
            }

            Swap(maxIndex, count - 1);
            count--;
            Heapify(maxIndex);

            return (keys[count], values[count]);
        }

        private void Heapify(int index)
        {
            int smallest = index;
            int left = HeapUtils.LeftChild(index);
            int right = HeapUtils.RightChild(index);

            if (left < count && keys[left].CompareTo(keys[smallest]) < 0)
            {
                smallest = left;
            }

            if (right < count && keys[right].CompareTo(keys[smallest]) < 0)
            {
                smallest = right;
            }

            if (smallest != index)
            {
                Swap(index, smallest);
                Heapify(smallest);
            }
        }

        private void Swap(int indexA, int indexB)
        {
            TKey tempKey = keys[indexA];
            TValue tempValue = values[indexA];

            keys[indexA] = keys[indexB];
            values[indexA] = values[indexB];

            keys[indexB] = tempKey;
            values[indexB] = tempValue;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(ref this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}