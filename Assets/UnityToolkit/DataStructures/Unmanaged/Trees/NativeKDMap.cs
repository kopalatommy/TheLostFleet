#if UNITY_STANDALONE

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;
using GalacticBoundStudios.DataScribes.Unmanaged; // Assuming your NativeMaxHeap is here
using Unity.Assertions;
using UnityEngine.Rendering;
using UnityEngine;

namespace GalacticBoundStudios.DataScribes.Managed.Trees
{
    [BurstCompile]
    [NativeContainer]
    public struct NativeKDMap<T> : IDisposable where T : unmanaged
    {
        public struct BuildTreeJob : IJob
        {
            public NativeKDMap<T> tree;

            public void Execute()
            {
                tree.Rebuild();
            }
        }

        public struct KDBounds
        {
            public float3 minBounds;
            public float3 maxBounds;
            public float3 size => maxBounds - minBounds;
            public float3 ClosestPoint(float3 point) => math.clamp(point, minBounds, maxBounds);
        }

        public struct Node
        {
            public int Count => end - start;
            public bool IsLeaf => partitionAxis == -1;
            public float partitionCoordinate;
            public int partitionAxis;
            public int negativeChildIndex;
            public int positiveChildIndex;
            public int start;
            public int end;
            public KDBounds bounds;
        }

        public bool IsCreated => points.IsCreated;
        public int Count => points.Length;

        [NativeDisableContainerSafetyRestriction]
        private NativeArray<float3> points;
        
        [NativeDisableContainerSafetyRestriction]
        private NativeArray<T> values; // The values associated with each point

        [NativeDisableContainerSafetyRestriction]
        private NativeArray<int> permutation;

        [NativeDisableContainerSafetyRestriction]
        private NativeList<Node> nodes;
        
        private int nodeCount;
        private readonly int leafCapacity;
        private readonly Allocator allocator;
        private readonly BuildStrategy buildStrategy;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
        internal DisposeSentinel m_DisposeSentinel;
#endif

        public NativeKDMap(Allocator allocator, BuildStrategy strategy = BuildStrategy.Midpoint, int initialCapacity = 0, int leafCapacity = 32)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (allocator <= Allocator.None) throw new ArgumentException("Allocator must be Temp, TempJob, or Persistent.", nameof(allocator));
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 1, allocator);
#endif
            this.nodeCount = 0;
            this.leafCapacity = leafCapacity;
            this.allocator = allocator;
            this.buildStrategy = strategy;

            this.points = new NativeArray<float3>(initialCapacity, allocator);
            this.values = new NativeArray<T>(initialCapacity, allocator); // Initialize values array
            this.permutation = new NativeArray<int>(initialCapacity, allocator);
            this.nodes = new NativeList<Node>(initialCapacity, allocator);
        }

        /// <summary>
        /// Sets the points and their corresponding values for the tree. This clears any existing data.
        /// </summary>
        public void SetData(NativeArray<float3> newPoints, NativeArray<T> newValues, bool rebuild = true)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            Assert.AreEqual(newPoints.Length, newValues.Length, "The number of points must match the number of values.");

            // Dispose and recreate arrays with the new size and data
            if (points.IsCreated)
            {
                points.Dispose();
            }
            if (values.IsCreated)
            {
                values.Dispose();
            }

            this.points = new NativeArray<float3>(newPoints, allocator);
            this.values = new NativeArray<T>(newValues, allocator);

            if (rebuild)
            {
                Rebuild();
            }
        }

        /// <summary>
        /// Adds the given points and corresponding values to the tree. This will always rebuild the tree.
        /// </summary>
        public void AddData(NativeArray<float3> newPoints, NativeArray<T> newValues)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            Assert.AreEqual(newPoints.Length, newValues.Length, "The number of points must match the number of values.");

            if (points.IsCreated && points.Length > 0)
            {
                // Create a new array to hold the combined data
                NativeArray<float3> updatedPoints = new NativeArray<float3>(points.Length + newPoints.Length, allocator);

                // Copy existing points
                points.CopyTo(updatedPoints.GetSubArray(0, points.Length));
                // Copy new points
                newPoints.CopyTo(updatedPoints.GetSubArray(points.Length, newPoints.Length));

                // Dispose the old array and assign the new one
                points.Dispose();
                points = updatedPoints; // Correctly assign the combined array
            }
            else
            {
                if (points.IsCreated) points.Dispose();
                points = new NativeArray<float3>(newPoints, allocator);
            }

            if (values.IsCreated && values.Length > 0)
            {
                // Create a new array for values
                NativeArray<T> updatedValues = new NativeArray<T>(values.Length + newValues.Length, allocator);
                
                // Copy existing values
                values.CopyTo(updatedValues.GetSubArray(0, values.Length));
                // Copy new values
                newValues.CopyTo(updatedValues.GetSubArray(values.Length, newValues.Length));
                
                // Dispose the old array and assign the new one
                values.Dispose();
                values = updatedValues; // Correctly assign the combined array
            }
            else
            {
                if (values.IsCreated) values.Dispose();
                values = new NativeArray<T>(newValues, allocator);
            }

            Rebuild();
        }

        public void Rebuild()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            if (points.Length == 0)
            {
                nodeCount = 0;
                nodes.Clear();
                return;
            }

            if (!permutation.IsCreated || permutation.Length != points.Length)
            {
                if(permutation.IsCreated) permutation.Dispose();
                permutation = new NativeArray<int>(points.Length, allocator);
            }

            ResetPermutation();

            nodeCount = 0;
            int nodeCountEstimate = 2 * (int)math.ceil(points.Length / (float)leafCapacity) + 1;
            if (nodes.Capacity < nodeCountEstimate) nodes.Capacity = nodeCountEstimate;
            nodes.Clear();

            int rootNodeIndex = GetNode();
            nodes[rootNodeIndex] = new Node
            {
                bounds = MakeRootBounds(),
                start = 0,
                end = points.Length,
                partitionAxis = -1
            };

            var buildQueue = new NativeQueue<int>(Allocator.Temp);
            buildQueue.Enqueue(rootNodeIndex);

            while (buildQueue.TryDequeue(out int index))
            {
                SplitNode(index, buildQueue);
            }
            
            buildQueue.Dispose();
        }

        private void SplitNode(int parentIndex, NativeQueue<int> buildQueue)
        {
            Node parent = nodes[parentIndex];
            if (parent.Count <= leafCapacity) return;

            KDBounds parentBounds = parent.bounds;
            float3 parentBoundsSize = parentBounds.size;

            int splitAxis = 0;
            if (parentBoundsSize.y > parentBoundsSize.x) splitAxis = 1;
            if (parentBoundsSize.z > parentBoundsSize[splitAxis]) splitAxis = 2;

            int splitIndex;
            float splitPivot;

            if (buildStrategy == BuildStrategy.Median)
            {
                int medianIndex = parent.start + parent.Count / 2;
                Select(parent.start, parent.end - 1, medianIndex, splitAxis);
                splitIndex = medianIndex;
                splitPivot = points[permutation[splitIndex]][splitAxis];
            }
            else
            {
                splitPivot = (parentBounds.minBounds[splitAxis] + parentBounds.maxBounds[splitAxis]) * 0.5f;
                splitIndex = Partition(parent.start, parent.end, splitPivot, splitAxis);
            }

            if (splitIndex == parent.start || splitIndex == parent.end) return;
            
            parent.partitionAxis = splitAxis;
            parent.partitionCoordinate = splitPivot;

            float3 negativeMax = parentBounds.maxBounds;
            negativeMax[splitAxis] = splitPivot;
            int negNodeIndex = GetNode();
            nodes[negNodeIndex] = new Node
            {
                bounds = new KDBounds { minBounds = parentBounds.minBounds, maxBounds = negativeMax },
                start = parent.start, end = splitIndex, partitionAxis = -1
            };
            parent.negativeChildIndex = negNodeIndex;

            float3 positiveMin = parentBounds.minBounds;
            positiveMin[splitAxis] = splitPivot;
            int posNodeIndex = GetNode();
            nodes[posNodeIndex] = new Node
            {
                bounds = new KDBounds { minBounds = positiveMin, maxBounds = parentBounds.maxBounds },
                start = splitIndex, end = parent.end, partitionAxis = -1
            };
            parent.positiveChildIndex = posNodeIndex;
            
            nodes[parentIndex] = parent;
            
            buildQueue.Enqueue(negNodeIndex);
            buildQueue.Enqueue(posNodeIndex);
        }

        private int Partition(int start, int end, float pivotPoint, int axis)
        {
            int left = start;
            for (int i = start; i < end; i++)
            {
                if (points[permutation[i]][axis] < pivotPoint)
                {
                    (permutation[i], permutation[left]) = (permutation[left], permutation[i]);
                    left++;
                }
            }
            return left;
        }

        private void Select(int left, int right, int k, int axis)
        {
            while (left < right)
            {
                int pivotIndex = PartitionForSelect(left, right, axis);
                if (k == pivotIndex) return;
                if (k < pivotIndex) right = pivotIndex - 1;
                else left = pivotIndex + 1;
            }
        }
        
        private int PartitionForSelect(int left, int right, int axis)
        {
            float pivotValue = points[permutation[right]][axis];
            int storeIndex = left;
            for (int i = left; i < right; i++)
            {
                if (points[permutation[i]][axis] < pivotValue)
                {
                    (permutation[storeIndex], permutation[i]) = (permutation[i], permutation[storeIndex]);
                    storeIndex++;
                }
            }
            (permutation[right], permutation[storeIndex]) = (permutation[storeIndex], permutation[right]);
            return storeIndex;
        }

        private void ResetPermutation()
        {
            for (int i = 0; i < permutation.Length; i++) permutation[i] = i;
        }

        private int GetNode()
        {
            nodes.Add(new Node { partitionAxis = -1, negativeChildIndex = -1, positiveChildIndex = -1 });
            return nodeCount++;
        }

        private KDBounds MakeRootBounds()
        {
            if (points.Length == 0) return default;
            float3 min = new float3(float.MaxValue);
            float3 max = new float3(float.MinValue);
            for (int i = 0; i < points.Length; i++)
            {
                min = math.min(min, points[i]);
                max = math.max(max, points[i]);
            }
            return new KDBounds { minBounds = min, maxBounds = max };
        }

        /// <summary>
        /// Finds all objects within a given radius of a query point.
        /// </summary>
        /// <param name="result">A list to which the found objects (T) will be added.</param>
        public void QueryRadius(float3 point, float radius, NativeList<T> result)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            if (points.Length == 0) return;

            float radiusSquared = radius * radius;
            var queue = new NativeQueue<int>(Allocator.Temp);
            queue.Enqueue(0);

            while (queue.TryDequeue(out int nodeIndex))
            {
                Node node = nodes[nodeIndex];
                if (node.IsLeaf)
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        if (math.lengthsq(points[permutation[i]] - point) <= radiusSquared)
                        {
                            result.Add(values[permutation[i]]); // Return the value, not the index
                        }
                    }
                }
                else
                {
                    float pointCoord = point[node.partitionAxis];
                    float distToPlaneSq = (pointCoord - node.partitionCoordinate) * (pointCoord - node.partitionCoordinate);
                    
                    int firstChild = (pointCoord < node.partitionCoordinate) ? node.negativeChildIndex : node.positiveChildIndex;
                    int secondChild = (pointCoord < node.partitionCoordinate) ? node.positiveChildIndex : node.negativeChildIndex;
                    
                    queue.Enqueue(firstChild);
                    if (distToPlaneSq <= radiusSquared) queue.Enqueue(secondChild);
                }
            }
            queue.Dispose();
        }

        /// <summary>
        /// Finds the K-nearest objects to a query point.
        /// </summary>
        /// <param name="result">A list to which the found objects (T) will be added. Note: The results are not guaranteed to be sorted by distance.</param>
        public void QueryKNearest(float3 point, int k, NativeList<T> result)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            if (points.Length == 0 || k <= 0) return;
            
            // The heap stores distance and the ORIGINAL index of the point/value
            var heap = new NativeMaxHeap<float, int>(Allocator.Temp, false, k);
            var queue = new NativeQueue<int>(Allocator.Temp);
            queue.Enqueue(0);

            while (queue.TryDequeue(out int nodeIndex))
            {
                Node node = nodes[nodeIndex];
                if (node.IsLeaf)
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        float distSq = math.lengthsq(points[permutation[i]] - point);
                        heap.Add(distSq, permutation[i]); // Store original index
                    }
                }
                else
                {
                    float pointCoord = point[node.partitionAxis];
                    float distToPlaneSq = (pointCoord - node.partitionCoordinate) * (pointCoord - node.partitionCoordinate);

                    int firstChild = (pointCoord < node.partitionCoordinate) ? node.negativeChildIndex : node.positiveChildIndex;
                    int secondChild = (pointCoord < node.partitionCoordinate) ? node.positiveChildIndex : node.negativeChildIndex;
                    
                    queue.Enqueue(firstChild);
                    if (heap.Count < k || distToPlaneSq < heap.PeekMax().Item1) queue.Enqueue(secondChild);
                }
            }
            
            while (heap.TryPopMax(out var item))
            {
                result.Add(values[item.Item2]); // Use the stored index to retrieve the value
            }

            heap.Dispose();
            queue.Dispose();
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            if (points.IsCreated)
            {
                points.Dispose();
            }
            if (values.IsCreated)
            {
                values.Dispose();
            }
            if (permutation.IsCreated)
            {
                permutation.Dispose();
            }
            if (nodes.IsCreated)
            {
                nodes.Dispose();
            }
        }
    }
}

#endif
