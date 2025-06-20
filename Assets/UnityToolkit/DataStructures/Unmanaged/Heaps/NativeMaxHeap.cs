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
    public struct NativeMaxHeap<TKey, TValue> : System.IDisposable, System.Collections.IEnumerable, System.Collections.Generic.IEnumerable<KeyValuePair<TKey,TValue>> where TKey : unmanaged, System.IComparable<TKey> where TValue : unmanaged
    {
        public struct BuildTreeJob : IJob
        {
            public NativeMaxHeap<TKey, TValue> heap;

            public void Execute()
            {
                heap.Rebuild();
            }
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable
        {
            private NativeMaxHeap<TKey, TValue> maxHeap;
            private NativeMaxHeap<TKey, TValue> queue;

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

            public Enumerator(ref NativeMaxHeap<TKey, TValue> maxHeap)
            {
                queue = new NativeMaxHeap<TKey, TValue>(Allocator.Persistent, false, maxHeap.Count);
                for (int i = 0; i < maxHeap.Count; i++)
                {
                    queue.Add(maxHeap.keys[i], maxHeap.values[i]);
                }

                this.maxHeap = maxHeap;
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
                    (TKey key, TValue value) = queue.PopMax();
                    current = new KeyValuePair<TKey, TValue>(key, value);
                    return true;
                }
            }

            public void Reset()
            {
                queue.Clear();
                for (int i = 0; i < maxHeap.Count; i++)
                {
                    queue.Add(maxHeap.keys[i], maxHeap.values[i]);
                }
                queue.Rebuild();
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

        public NativeMaxHeap(Allocator allocator, bool canGrow=true, int capacity=16)
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
                    // Drop the largest item
                    if (key.CompareTo(keys[0]) < 0)
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

        public (TKey, TValue) PeekMax()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty.");
            }

            return (keys[0], values[0]);
        }

        public (TKey, TValue) PopMax()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty.");
            }

            TKey maxKey = keys[0];
            TValue maxValue = values[0];

            keys[0] = keys[count - 1];
            values[0] = values[count - 1];
            count--;

            Heapify(0);

            return (maxKey, maxValue);
        }

        public bool TryPopMax(out (TKey, TValue) result)
        {
            if (IsEmpty)
            {
                result = default;
                return false;
            }
            else
            {
                result = PopMax();
                return true;
            }
        }

        public void Clear(bool removeArrays = true)
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

                if (left < count && keys[left].CompareTo(key) >= 0)
                {
                    toCheck.Enqueue(left);
                }
                if (right < count && keys[right].CompareTo(key) >= 0)
                {
                    toCheck.Enqueue(right);
                }
            }

            toCheck.Dispose();
            return false;
        }

        public (TKey, TValue) MinItem()
        {
            if (IsEmpty)
            {
                throw new System.InvalidOperationException("Heap is empty.");
            }

            int minIndex = 0;

            // for (int i = 1; i < count; i++)
            // {
            //     if (keys[i].CompareTo(keys[minIndex]) < 0)
            //     {
            //         minIndex = i;
            //     }
            // }

            for (int i = count / 2; i < count; i++)
            {
                if (keys[i].CompareTo(keys[minIndex]) < 0)
                {
                    minIndex = i;
                }
            }

            return (keys[minIndex], values[minIndex]);
        }
        
        private void Heapify(int index)
        {
            int largest = index;
            int left = HeapUtils.LeftChild(index);
            int right = HeapUtils.RightChild(index);

            if (left < count && keys[left].CompareTo(keys[largest]) > 0)
            {
                largest = left;
            }

            if (right < count && keys[right].CompareTo(keys[largest]) > 0)
            {
                largest = right;
            }

            if (largest != index)
            {
                Swap(index, largest);
                Heapify(largest);
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