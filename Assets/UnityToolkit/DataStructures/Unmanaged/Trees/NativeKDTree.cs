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

namespace GalacticBoundStudios.DataScribes.Managed.Trees
{
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

        // Holds all of the points in the tree
        public NativeArray<float3> points;
        // Holds the indices of the points in the tree
        private NativeArray<int> permutation;
        // The number of nodes in the tree
        public int count;
        // The nodes in the tree
        private NativeList<Node> nodes;
        // The number of nodes in the tree
        private int nodeCount;
        // The number of points that a leaf node can hold
        private int leafCapacity;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
        internal DisposeSentinel m_DisposeSentinel;
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS

        public NativeKDTree(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (allocator <= Allocator.None)
            {
                throw new ArgumentException("Allocator must be Temp, TempJob, or Persistent.", nameof(allocator));
            }

            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 1, allocator);
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS

            points = new NativeArray<float3>(0, allocator);
            permutation = new NativeArray<int>(0, allocator);
            nodes = new NativeList<Node>(0, allocator);

            count = 0;
            nodeCount = 0;
            leafCapacity = 32;
        }

        public void SetPoints(NativeArray<float3> points, bool rebuild = true)
        {
            this.points = points;

            if (rebuild)
            {
                Rebuild();
            }
        }

        public void AddPoints(NativeArray<float3> points, bool rebuild = true)
        {
            this.points.ResizeArray(this.points.Length + points.Length);
            this.permutation.ResizeArray(this.points.Length);

            int startIndex = this.points.Length - points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                this.points[startIndex + i] = points[i];
            }

            count++;

            if (rebuild)
            {
                Rebuild();
            }
        }

        public void Rebuild()
        {
            NativeQueue<int> buildQueue = new NativeQueue<int>(Allocator.TempJob);

            ResetPermutation();

            // Reset the node count
            nodeCount = 0;
            // Determine the expected number of nodes
            int nodeCountEstimate = 4 * (int) math.ceil(points.Length / (float) leafCapacity + 1) + 1;
            // Resize the nodes array if necessary
            if (nodes.Capacity < nodeCountEstimate)
            {
                nodes.Resize(nodeCountEstimate, NativeArrayOptions.UninitializedMemory);
            }

            // Create the root node
            int rootNodeIndex = GetNode();
            Node curNode = nodes[rootNodeIndex];
            curNode.bounds = MakeRootBounds();
            curNode.start = 0;
            curNode.end = points.Length;
            nodes[rootNodeIndex] = curNode;

            // Add the root node to the build queue
            buildQueue.Enqueue(rootNodeIndex);

            while (!buildQueue.IsEmpty())
            {
                int index = buildQueue.Dequeue();
                SplitNode(index, out int posNodeIndex, out int negNodeIndex, buildQueue);
            }

            buildQueue.Dispose();
        }

        private void SplitNode(int parentIndex, out int posNodeIndex, out int negNodeIndex, NativeQueue<int> buildQueue)
        {
            Node parent = nodes[parentIndex];
            KDBounds parentBounds = parent.bounds;
            float3 parentBoundsSize = parentBounds.size;

            // Find the axis where the bounds are the largest
            int splitAxis = 0;
            float axisSize = parentBoundsSize.x;

            if (axisSize < parentBoundsSize.y)
            {
                splitAxis = 1;
                axisSize = parentBoundsSize.y;
            }
            if (axisSize < parentBoundsSize.z)
            {
                splitAxis = 2;
                axisSize = parentBoundsSize.z;
            }

            // Determine the axis min and max
            float axisMin = parentBounds.minBounds[splitAxis];
            float axisMax = parentBounds.maxBounds[splitAxis];

            float splitPivot = CalculateSplitPivot(parent.start, parent.end, axisMin, axisMax, splitAxis);

            // Update the parent node
            parent.partitionAxis = splitAxis;
            parent.partitionCoordinate = splitPivot;

            // Partition the points into the negative and positive children
            int splitIndex = Partition(parent.start, parent.end, splitPivot, splitAxis);

            // Create the negative child node
            float3 negativeMax = parentBounds.maxBounds;
            negativeMax[splitAxis] = splitPivot;
            KDBounds bounds = new KDBounds
            {
                minBounds = parentBounds.minBounds,
                maxBounds = negativeMax
            };

            negNodeIndex = GetNode();
            Node negNode = nodes[negNodeIndex];
            negNode.bounds = bounds;
            negNode.start = parent.start;
            negNode.end = splitIndex;
            nodes[negNodeIndex] = negNode;
            parent.negativeChildIndex = negNodeIndex;

            // Create the positive child node
            float3 positiveMin = parentBounds.minBounds;
            positiveMin[splitAxis] = splitPivot;
            bounds = parent.bounds;

            bounds.minBounds = positiveMin;
            posNodeIndex = GetNode();
            Node posNode = nodes[posNodeIndex];
            posNode.bounds = bounds;
            posNode.start = splitIndex;
            posNode.end = parent.end;
            nodes[posNodeIndex] = posNode;
            parent.positiveChildIndex = posNodeIndex;

            // Update the parent node
            nodes[parentIndex] = parent;

            if (ContinueSplitting(parent.start, splitIndex))
            {
                buildQueue.Enqueue(negNodeIndex);
            }
            if (ContinueSplitting(splitIndex, parent.end))
            {
                buildQueue.Enqueue(posNodeIndex);
            }
        }

        private bool ContinueSplitting(int start, int end)
        {
            return end - start > leafCapacity;
        }

        private int Partition(int start, int end, float pivotPoint, int axis)
        {
            int leftIndex = start - 1;
            int rightIndex = end;

            for (;;)
            {
                do
                {
                    leftIndex++;
                } while (leftIndex < rightIndex && points[permutation[leftIndex]][axis] < pivotPoint);

                do
                {
                    rightIndex--;
                } while (leftIndex < rightIndex && points[permutation[rightIndex]][axis] >= pivotPoint);

                if (leftIndex < rightIndex)
                {
                    int temp = permutation[leftIndex];
                    permutation[leftIndex] = permutation[rightIndex];
                    permutation[rightIndex] = temp;
                }
                else
                {
                    return leftIndex;
                }
            }
        }

        private float CalculateSplitPivot(int start, int end, float axisMin, float axisMax, int splitAxis)
        {
            float midPoint = (axisMin + axisMax) * 0.5f;

            bool foundNegative = false;
            bool foundPositive = false;

            for (int i = start; i < end; i++)
            {
                float point = points[permutation[i]][splitAxis];

                if (point < midPoint)
                {
                    foundNegative = true;
                }
                else
                {
                    foundPositive = true;
                }

                if (foundNegative && foundPositive)
                {
                    return midPoint;
                }
            }

            if (foundNegative)
            {
                float negativeMax = float.MinValue;

                for (int i = start; i < end; i++)
                {
                    negativeMax = math.max(negativeMax, points[permutation[i]][splitAxis]);
                }

                return negativeMax;
            }
            else
            {
                float positiveMin = float.MaxValue;

                for (int i = start; i < end; i++)
                {
                    positiveMin = math.min(positiveMin, points[permutation[i]][splitAxis]);
                }

                return positiveMin;
            }
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
            if (nodeCount >= nodes.Length)
            {
                nodes.Resize((int)(nodes.Length * 1.5), NativeArrayOptions.UninitializedMemory);
            }
            ResetNode(nodeCount);
            return nodeCount++;
        }

        private void ResetNode(int nodeIndex)
        {
            nodes[nodeIndex] = new Node
            {
                partitionAxis = -1,
                partitionCoordinate = 0,
                negativeChildIndex = -1,
                positiveChildIndex = -1,
                start = -1,
                end = -1
            };
        }

        private KDBounds MakeRootBounds()
        {
            float3 min = float.MaxValue;
            float3 max = float.MinValue;

            int evenCount = points.Length >> 1;

            for (int i = 0; i < evenCount; i += 2)
            {
                int j = i + 1;

                if (points[i].x > points[j].x)
                {
                    min.x = math.min(min.x, points[j].x);
                    max.x = math.max(max.x, points[i].x);
                }
                else
                {
                    min.x = math.min(min.x, points[i].x);
                    max.x = math.max(max.x, points[j].x);
                }

                if (points[i].y > points[j].y)
                {
                    min.y = math.min(min.y, points[j].y);
                    max.y = math.max(max.y, points[i].y);
                }
                else
                {
                    min.y = math.min(min.y, points[i].y);
                    max.y = math.max(max.y, points[j].y);
                }

                if (points[i].z > points[j].z)
                {
                    min.z = math.min(min.z, points[j].z);
                    max.z = math.max(max.z, points[i].z);
                }
                else
                {
                    min.z = math.min(min.z, points[i].z);
                    max.z = math.max(max.z, points[j].z);
                }
            }

            if ((points.Length & 1) == 1)
            {
                min = math.min(min, points[points.Length - 1]);
                max = math.max(max, points[points.Length - 1]);
            }

            return new KDBounds
            {
                minBounds = min,
                maxBounds = max
            };                
        }

        public void QueryRadius(float3 point, float radius, NativeList<int> result)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

            if (count == 0)
            {
                return;
            }

            float radiusSquared = radius * radius;

            // Use a queue to traverse the tree
            NativeQueue<int> queue = new NativeQueue<int>(Allocator.TempJob);
            queue.Enqueue(0);

            while (!queue.IsEmpty())
            {
                int nodeIndex = queue.Dequeue();
                Node node = nodes[nodeIndex];

                if (node.IsLeaf)
                {
                    Assert.IsTrue(node.end - node.start <= leafCapacity);
                    for (int i = node.start; i < node.end; i++)
                    {
                        float3 pointInTree = points[permutation[i]];
                        float distanceSquared = math.lengthsq(pointInTree - point);

                        if (distanceSquared <= radiusSquared)
                        {
                            result.Add(permutation[i]);
                        }
                    }
                }
                else
                {
                    int partitionAxis = node.partitionAxis;
                    float partitionCoord = node.partitionCoordinate;

                    float3 closestPoint = node.bounds.ClosestPoint(point);

                    if ((closestPoint[partitionAxis] - partitionCoord) < 0)
                    {
                        queue.Enqueue(node.negativeChildIndex);

                        float sqrDist = math.lengthsq(closestPoint - point);

                        if (nodes[node.positiveChildIndex].Count != 0 && sqrDist <= radiusSquared)
                        {
                            queue.Enqueue(node.positiveChildIndex);
                        }
                    }
                    else
                    {
                        queue.Enqueue(node.positiveChildIndex);

                        float sqrDist = math.lengthsq(closestPoint - point);

                        if (nodes[node.negativeChildIndex].Count != 0 && sqrDist <= radiusSquared)
                        {
                            queue.Enqueue(node.negativeChildIndex);
                        }
                    }
                }
            }

            queue.Dispose();
        }

        public void QueryKNearest(float3 point, int k, NativeList<int> result)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

            // Use a max heap so that the farthest point is at the top and can be dropped
            NativeMaxHeap<float, int> heap = new NativeMaxHeap<float, int>(Allocator.TempJob, false);

            // Use a queue to traverse the tree
            NativeQueue<int> queue = new NativeQueue<int>(Allocator.Temp);
            queue.Enqueue(0);
            while (queue.Count > 0)
            {
                int nodeIndex = queue.Dequeue();
                Node node = nodes[nodeIndex];

                if (node.IsLeaf)
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        float3 pointInTree = points[permutation[i]];
                        float distanceSquared = math.lengthsq(pointInTree - point);

                        // Just add the item to the heap, it will drop the largest item if it is full
                        heap.Add(distanceSquared, permutation[i]);
                    }
                }
                else
                {
                    float3 leftClosestPoint = nodes[node.negativeChildIndex].bounds.ClosestPoint(point);
                    float3 rightClosestPoint = nodes[node.positiveChildIndex].bounds.ClosestPoint(point);

                    float leftNodeDist = math.lengthsq(leftClosestPoint - point);
                    float rightNodeDist = math.lengthsq(rightClosestPoint - point);

                    if (heap.IsEmpty || leftNodeDist < heap.PeekMax().Item1)
                    {
                        queue.Enqueue(node.negativeChildIndex);
                    }
                    if (heap.IsEmpty || rightNodeDist < heap.PeekMax().Item1)
                    {
                        queue.Enqueue(node.positiveChildIndex);
                    }
                }
            }

            // Copy the results from the heap to the result list
            while (!heap.IsEmpty)
            {
                result.Add(heap.PopMax().Item2);
            }

            heap.Dispose();
            queue.Dispose();
        }

        public void QueryInterval(float3 min, float3 max, NativeList<int> result)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS

            // Use a queue to traverse the tree
            NativeQueue<int> queue = new NativeQueue<int>(Allocator.Temp);
            queue.Enqueue(0);

            while (!queue.IsEmpty())
            {
                int nodeIndex = queue.Dequeue();
                Node node = nodes[nodeIndex];

                if (node.IsLeaf)
                {
                    for (int i = node.start; i < node.end; i++)
                    {
                        float3 pointInTree = points[permutation[i]];

                        if (pointInTree.x >= min.x && pointInTree.x <= max.x &&
                            pointInTree.y >= min.y && pointInTree.y <= max.y &&
                            pointInTree.z >= min.z && pointInTree.z <= max.z)
                        {
                            result.Add(permutation[i]);
                        }
                    }
                }
                else
                {
                    if (nodes[node.negativeChildIndex].bounds.maxBounds.x >= min.x && nodes[node.negativeChildIndex].bounds.minBounds.x <= max.x &&
                        nodes[node.negativeChildIndex].bounds.maxBounds.y >= min.y && nodes[node.negativeChildIndex].bounds.minBounds.y <= max.y &&
                        nodes[node.negativeChildIndex].bounds.maxBounds.z >= min.z && nodes[node.negativeChildIndex].bounds.minBounds.z <= max.z)
                    {
                        queue.Enqueue(node.negativeChildIndex);
                    }

                    if (nodes[node.positiveChildIndex].bounds.maxBounds.x >= min.x && nodes[node.positiveChildIndex].bounds.minBounds.x <= max.x &&
                        nodes[node.positiveChildIndex].bounds.maxBounds.y >= min.y && nodes[node.positiveChildIndex].bounds.minBounds.y <= max.y &&
                        nodes[node.positiveChildIndex].bounds.maxBounds.z >= min.z && nodes[node.positiveChildIndex].bounds.minBounds.z <= max.z)
                    {
                        queue.Enqueue(node.positiveChildIndex);
                    }
                }
            }

            queue.Dispose();
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif // ENABLE_UNITY_COLLECTIONS_CHECKS

            points.Dispose();
            permutation.Dispose();
            nodes.Dispose();
        }
    }
}

#endif // UNITY_STANDALONE