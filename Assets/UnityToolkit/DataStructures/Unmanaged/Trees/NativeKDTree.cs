#if UNITY_STANDALONE

using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering;
using Unity.Burst;
using GalacticBoundStudios.DataScribes.Unmanaged;
using Unity.Assertions;
using System.Linq;

namespace GalacticBoundStudios.DataScribes.Managed.Trees
{
    /// <summary>
    /// Defines the strategy used to build the k-d tree.
    /// </summary>
    public enum BuildStrategy
    {
        /// <summary>
        /// Splits nodes at the geometric center of their bounds. Faster to build, but can result in an unbalanced tree. Best for uniformly distributed data.
        /// </summary>
        Midpoint,
        /// <summary>
        /// Splits nodes at the median point. Slower to build, but results in a perfectly balanced tree for faster queries. Best for clustered or non-uniform data.
        /// </summary>
        Median
    }
    
    [BurstCompile]
    [NativeContainer]
    public struct NativeKDTree : IDisposable
    {
        public struct BuildTreeJob : IJob
        {
            public NativeKDTree tree;

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

            public float3 ClosestPoint(float3 point)
            {
                return math.clamp(point, minBounds, maxBounds);
            }
        }

        public struct Node
        {
            public int Count => end - start;
            public bool IsLeaf => partitionAxis == -1;

            // What is the value of this node on the partition axis?
            public float partitionCoordinate;
            // What index of the dimension array is this node's partition coordinate?
            public int partitionAxis;

            // Index of the child nodes
            public int negativeChildIndex;
            public int positiveChildIndex;

            // Index of points this node contains
            public int start;
            public int end;

            public KDBounds bounds;
        }

        public bool IsCreated => points.IsCreated;
        public int Count => points.Length;

        // Holds all of the points in the tree (owned by this struct)
        [NativeDisableContainerSafetyRestriction]
        private NativeArray<float3> points;
        // Holds the indices of the points in the tree
        [NativeDisableContainerSafetyRestriction]
        private NativeArray<int> permutation;
        // The nodes in the tree
        [NativeDisableContainerSafetyRestriction]
        private NativeList<Node> nodes;
        // The number of nodes in the tree
        private int nodeCount;
        // The number of points that a leaf node can hold
        private int leafCapacity;
        // The allocator used for the native collections
        private Allocator allocator;
        // The strategy for building the tree
        private BuildStrategy buildStrategy;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
        internal DisposeSentinel m_DisposeSentinel;
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS

        /// <summary>
        /// Creates a new NativeKDTree.
        /// </summary>
        /// <param name="allocator">The allocator to use for the tree's native collections.</param>
        /// <param name="strategy">The strategy to use for building the tree. Midpoint is faster to build, Median provides faster queries.</param>
        /// <param name="initialCapacity">The initial capacity of the point array.</param>
        /// <param name="leafCapacity">The maximum number of points a leaf node can hold.</param>
        public NativeKDTree(Allocator allocator, BuildStrategy strategy = BuildStrategy.Midpoint, int initialCapacity = 0, int leafCapacity = 32)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (allocator <= Allocator.None)
            {
                throw new ArgumentException("Allocator must be Temp, TempJob, or Persistent.", nameof(allocator));
            }
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 1, allocator);
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS

            this.points = new NativeArray<float3>(initialCapacity, allocator);
            this.permutation = new NativeArray<int>(initialCapacity, allocator);
            this.nodes = new NativeList<Node>(initialCapacity, allocator);
            this.nodeCount = 0;
            this.leafCapacity = leafCapacity;
            this.allocator = allocator;
            this.buildStrategy = strategy;
        }

        public void SetPoints(NativeArray<float3> newPoints, bool rebuild = true)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            // Ensure our internal array is the correct size
            if (!this.points.IsCreated || this.points.Length != newPoints.Length)
            {
                if(this.points.IsCreated) this.points.Dispose();
                this.points = new NativeArray<float3>(newPoints.Length, allocator);
            }

            // Copy the data
            NativeArray<float3>.Copy(newPoints, this.points);

            if (rebuild)
            {
                Rebuild();
            }
        }
        
        public void AddPoints(NativeArray<float3> newPoints, bool rebuild = true)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            int oldLength = this.points.Length;
            int newLength = oldLength + newPoints.Length;
            
            // Resize by creating a new array and copying the old data
            var newPointsArray = new NativeArray<float3>(newLength, allocator);
            if(oldLength > 0) NativeArray<float3>.Copy(this.points, newPointsArray, oldLength);
            if(this.points.IsCreated) this.points.Dispose();
            this.points = newPointsArray;
            
            // Copy the new data into the resized array
            NativeArray<float3>.Copy(newPoints, 0, this.points, oldLength, newPoints.Length);
            
            if (rebuild)
            {
                Rebuild();
            }
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

            // Reset the node count
            nodeCount = 0;
            // Determine the expected number of nodes
            int nodeCountEstimate = 2 * (int)math.ceil(points.Length / (float)leafCapacity) + 1;
            // Resize the nodes array if necessary
            if (nodes.Capacity < nodeCountEstimate)
            {
                nodes.Capacity = nodeCountEstimate;
            }
            nodes.Clear();

            // Create the root node
            int rootNodeIndex = GetNode();
            Node rootNode = new Node
            {
                bounds = MakeRootBounds(),
                start = 0,
                end = points.Length,
                partitionAxis = -1
            };
            nodes[rootNodeIndex] = rootNode;

            // Use a queue for iterative (non-recursive) build
            NativeQueue<int> buildQueue = new NativeQueue<int>(Allocator.Temp);
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

            // If the node is too small, make it a leaf
            if (parent.Count <= leafCapacity)
            {
                return;
            }

            KDBounds parentBounds = parent.bounds;
            float3 parentBoundsSize = parentBounds.size;

            // Find the axis where the bounds are the largest
            int splitAxis = 0;
            if (parentBoundsSize.y > parentBoundsSize.x) splitAxis = 1;
            if (parentBoundsSize.z > parentBoundsSize[splitAxis]) splitAxis = 2;

            int splitIndex;
            float splitPivot;

            if (buildStrategy == BuildStrategy.Median)
            {
                // For Median strategy, we find the actual median point and split there.
                // This balances the tree perfectly.
                int medianIndex = parent.start + parent.Count / 2;
                Select(parent.start, parent.end - 1, medianIndex, splitAxis);
                splitIndex = medianIndex;
                splitPivot = points[permutation[splitIndex]][splitAxis];
            }
            else
            {
                // For Midpoint strategy, we split at the geometric center of the node.
                splitPivot = (parentBounds.minBounds[splitAxis] + parentBounds.maxBounds[splitAxis]) * 0.5f;
                splitIndex = Partition(parent.start, parent.end, splitPivot, splitAxis);
            }

            // If the partition is not successful (all points on one side), make this a leaf to prevent infinite loops.
            if (splitIndex == parent.start || splitIndex == parent.end)
            {
                return;
            }

            // Update the parent node since it is no longer a leaf
            parent.partitionAxis = splitAxis;
            parent.partitionCoordinate = splitPivot;

            // Create the negative child node
            float3 negativeMax = parentBounds.maxBounds;
            negativeMax[splitAxis] = splitPivot;
            int negNodeIndex = GetNode();
            nodes[negNodeIndex] = new Node
            {
                bounds = new KDBounds { minBounds = parentBounds.minBounds, maxBounds = negativeMax },
                start = parent.start,
                end = splitIndex,
                partitionAxis = -1
            };
            parent.negativeChildIndex = negNodeIndex;

            // Create the positive child node
            float3 positiveMin = parentBounds.minBounds;
            positiveMin[splitAxis] = splitPivot;
            int posNodeIndex = GetNode();
            nodes[posNodeIndex] = new Node
            {
                bounds = new KDBounds { minBounds = positiveMin, maxBounds = parentBounds.maxBounds },
                start = splitIndex,
                end = parent.end,
                partitionAxis = -1
            };
            parent.positiveChildIndex = posNodeIndex;

            // Update the parent node in the list
            nodes[parentIndex] = parent;

            // Enqueue children to continue splitting
            buildQueue.Enqueue(negNodeIndex);
            buildQueue.Enqueue(posNodeIndex);
        }

        /// <summary>
        /// Partitions the permutation array for the Midpoint build strategy.
        /// </summary>
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
        
        /// <summary>
        /// Reorders the permutation array slice so that the element at the k-th position is the one that would be in that position in a sorted array.
        /// This is used for the Median build strategy.
        /// </summary>
        private void Select(int left, int right, int k, int axis)
        {
            while (left < right)
            {
                int pivotIndex = PartitionForSelect(left, right, axis);
                if (k == pivotIndex) return;

                if (k < pivotIndex)
                {
                    right = pivotIndex - 1;
                }
                else
                {
                    left = pivotIndex + 1;
                }
            }
        }
        
        /// <summary>
        /// A partition function used by the Select method (based on Lomuto partition scheme).
        /// </summary>
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
            for (int i = 0; i < permutation.Length; i++)
            {
                permutation[i] = i;
            }
        }

        private int GetNode()
        {
            // Reset the node before returning its index
            nodes.Add(new Node
            {
                partitionAxis = -1, // Indicates a leaf node
                negativeChildIndex = -1,
                positiveChildIndex = -1
            });
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

        public void QueryRadius(float3 point, float radius, NativeList<int> result)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            if (points.Length == 0) return;

            float radiusSquared = radius * radius;

            NativeQueue<int> queue = new NativeQueue<int>(Allocator.Temp);
            queue.Enqueue(0); // Start with the root node

            while (queue.TryDequeue(out int nodeIndex))
            {
                Node node = nodes[nodeIndex];

                if (node.IsLeaf)
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        if (math.lengthsq(points[permutation[i]] - point) <= radiusSquared)
                        {
                            result.Add(permutation[i]);
                        }
                    }
                }
                else
                {
                    int partitionAxis = node.partitionAxis;
                    float pointCoord = point[partitionAxis];
                    float partitionCoord = node.partitionCoordinate;

                    int firstChild = (pointCoord < partitionCoord) ? node.negativeChildIndex : node.positiveChildIndex;
                    int secondChild = (pointCoord < partitionCoord) ? node.positiveChildIndex : node.negativeChildIndex;

                    // Always traverse the first, more likely child's branch
                    queue.Enqueue(firstChild);

                    // Only traverse the second child's branch if the query sphere crosses the splitting plane
                    float distToPlaneSq = (pointCoord - partitionCoord) * (pointCoord - partitionCoord);
                    if (distToPlaneSq <= radiusSquared)
                    {
                        queue.Enqueue(secondChild);
                    }
                }
            }
            queue.Dispose();
        }

        public void QueryKNearest(float3 point, int k, NativeList<int> result, bool sortResult = true)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            if (points.Length == 0 || k <= 0) return;

            // Use a max heap so that the farthest point is always at the top and can be dropped
            var heap = new NativeMaxHeap<float, int>(Allocator.Temp, false, k);
            
            NativeQueue<int> queue = new NativeQueue<int>(Allocator.Temp);
            queue.Enqueue(0); // Start with the root node

            while (queue.TryDequeue(out int nodeIndex))
            {
                Node node = nodes[nodeIndex];

                if (node.IsLeaf)
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        float distSq = math.lengthsq(points[permutation[i]] - point);
                        heap.Add(distSq, permutation[i]);
                    }
                }
                else
                {
                    float pointCoord = point[node.partitionAxis];
                    float partitionCoord = node.partitionCoordinate;
                    
                    int firstChild = (pointCoord < partitionCoord) ? node.negativeChildIndex : node.positiveChildIndex;
                    int secondChild = (pointCoord < partitionCoord) ? node.positiveChildIndex : node.negativeChildIndex;
                    
                    // Always check the closer child's subtree first.
                    queue.Enqueue(firstChild);
                    
                    // Only check the second child if its bounds are potentially closer
                    // than the farthest neighbor we've found so far.
                    float distToPlaneSq = (pointCoord - partitionCoord) * (pointCoord - partitionCoord);
                    if (heap.Count < k || distToPlaneSq < heap.PeekMax().Item1)
                    {
                        queue.Enqueue(secondChild);
                    }
                }
            }

            // Copy the results from the heap to the result list
            while (heap.TryPopMax(out var item))
            {
                result.Add(item.Item2);
            }

            if (sortResult)
            {
                result.Sort();
            }

            heap.Dispose();
            queue.Dispose();
        }

        public void QueryInterval(float3 queryMin, float3 queryMax, NativeList<int> result)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            if (points.Length == 0) return;

            NativeQueue<int> queue = new NativeQueue<int>(Allocator.Temp);
            queue.Enqueue(0); // Start with root node

            while(queue.TryDequeue(out int nodeIndex))
            {
                Node node = nodes[nodeIndex];

                if (node.IsLeaf)
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        float3 p = points[permutation[i]];
                        // Check if the point is within the query bounds
                        if (math.all(p >= queryMin) && math.all(p <= queryMax))
                        {
                            result.Add(permutation[i]);
                        }
                    }
                }
                else
                {
                    // Check if negative child overlaps with the query interval
                    if (BoundsOverlap(nodes[node.negativeChildIndex].bounds, queryMin, queryMax))
                    {
                        queue.Enqueue(node.negativeChildIndex);
                    }
                    
                    // Check if positive child overlaps with the query interval
                    if (BoundsOverlap(nodes[node.positiveChildIndex].bounds, queryMin, queryMax))
                    {
                        queue.Enqueue(node.positiveChildIndex);
                    }
                }
            }
            queue.Dispose();
        }

        private bool BoundsOverlap(KDBounds nodeBounds, float3 queryMin, float3 queryMax)
        {
            // Check for non-overlap on each axis. If there's no overlap on any axis,
            // the bounds do not intersect.
            return math.all(nodeBounds.minBounds <= queryMax) && math.all(nodeBounds.maxBounds >= queryMin);
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
            if(points.IsCreated) points.Dispose();
            if(permutation.IsCreated) permutation.Dispose();
            if(nodes.IsCreated) nodes.Dispose();
        }
    }
}

#endif // UNITY_STANDALONE
