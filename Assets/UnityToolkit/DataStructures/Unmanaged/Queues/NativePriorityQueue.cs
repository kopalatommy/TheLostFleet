using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

namespace GalacticBoundStudios.DataScribes.Managed
{
    [BurstCompile]
    [NativeContainer]
    public struct NativePriorityQueue<T> : INativeDisposable where T : unmanaged, IComparable<T>
    {
        private NativeList<T> _heap;

        public bool IsEmpty => _heap.Length == 0;

        public NativePriorityQueue(Allocator allocator)
        {
            _heap = new NativeList<T>(allocator);
        }

        public void Add(T item)
        {
            _heap.Add(item);
            ShiftUp(_heap.Length - 1);
        }

        public T Dequeue()
        {
            if (_heap.Length == 0) {
                throw new InvalidOperationException("Queue is empty");
            }

            T result = _heap[0];
            _heap[0] = _heap[_heap.Length - 1];
            _heap.RemoveAtSwapBack(_heap.Length - 1);
            SiftDown(0);

            return result;
        }

        private void ShiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_heap[parent].CompareTo(_heap[index]) <= 0)
                    break;

                Swap(parent, index);
                index = parent;
            }
        }

        private void SiftDown(int index)
        {
            int lastIndex = _heap.Length - 1;

            while (true)
            {
                int leftChild = 2 * index + 1;
                int rightChild = 2 * index + 2;
                int smallest = index;

                if (leftChild <= lastIndex && _heap[leftChild].CompareTo(_heap[smallest]) < 0)
                    smallest = leftChild;

                if (rightChild <= lastIndex && _heap[rightChild].CompareTo(_heap[smallest]) < 0)
                    smallest = rightChild;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int indexA, int indexB)
        {
            T temp = _heap[indexA];
            _heap[indexA] = _heap[indexB];
            _heap[indexB] = temp;
        }

        public void Dispose()
        {
            if (_heap.IsCreated)
                _heap.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            if (_heap.IsCreated) {
                return _heap.Dispose(inputDeps);
            }

            return inputDeps;
        }
    }
}