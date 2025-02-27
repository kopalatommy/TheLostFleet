#if UNITY_STANDALONE

using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace GalacticBoundStudios.DataScribes.Managed.Trees
{
    [NativeContainer]
    public struct KDTree_Array
    {
        public struct BuildTreeJob : IJob
        {
            public KDTree_Array tree;

            private NativeQueue<int> buildQueue;

            public void Execute()
            {
                buildQueue = new NativeQueue<int>(Allocator.TempJob);

                ResetPermutation();

                // Reset the node count
                tree.nodeCount = 0;
                // Determine the expected number of nodes
                int nodeCountEstimate = 4 * (int) math.ceil(tree.points.Length / (float) tree.leafCapacity + 1) + 1;
                // Resize the nodes array if necessary
                if (tree.nodes.Capacity < nodeCountEstimate)
                {
                    tree.nodes.Resize(nodeCountEstimate, NativeArrayOptions.UninitializedMemory);
                }

                // Create the root node
                int rootNodeIndex = GetNode();
                Node curNode = tree.nodes[rootNodeIndex];
                curNode.bounds = MakeRootBounds();
                curNode.start = 0;
                curNode.end = tree.points.Length;
                tree.nodes[rootNodeIndex] = curNode;

                // Add the root node to the build queue
                buildQueue.Enqueue(rootNodeIndex);

                while (!buildQueue.IsEmpty())
                {
                    int index = buildQueue.Dequeue();
                    SplitNode(index, out int posNodeIndex, out int negNodeIndex);
                    buildQueue.Enqueue(posNodeIndex);
                    buildQueue.Enqueue(negNodeIndex);
                }

                buildQueue.Dispose();
            }

            private void SplitNode(int parentIndex, out int posNodeIndex, out int negNodeIndex)
            {
                Node parent = tree.nodes[parentIndex];
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
                Node negNode = tree.nodes[negNodeIndex];
                negNode.bounds = bounds;
                negNode.start = parent.start;
                negNode.end = splitIndex;
                tree.nodes[negNodeIndex] = negNode;
                parent.negativeChildIndex = negNodeIndex;

                // Create the positive child node
                float3 positiveMin = parentBounds.minBounds;
                positiveMin[splitAxis] = splitPivot;
                bounds = parent.bounds;

                bounds.minBounds = positiveMin;
                posNodeIndex = GetNode();
                Node posNode = tree.nodes[posNodeIndex];
                posNode.bounds = bounds;
                posNode.start = splitIndex;
                posNode.end = parent.end;
                tree.nodes[posNodeIndex] = posNode;
                parent.positiveChildIndex = posNodeIndex;

                // Update the parent node
                tree.nodes[parentIndex] = parent;

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
                return end - start > tree.leafCapacity;
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
                    } while (leftIndex < rightIndex && tree.points[tree.permutation[leftIndex]][axis] < pivotPoint);

                    do
                    {
                        rightIndex--;
                    } while (leftIndex < rightIndex && tree.points[tree.permutation[rightIndex]][axis] >= pivotPoint);

                    if (leftIndex < rightIndex)
                    {
                        int temp = tree.permutation[leftIndex];
                        tree.permutation[leftIndex] = tree.permutation[rightIndex];
                        tree.permutation[rightIndex] = temp;
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
                    float point = tree.points[tree.permutation[i]][splitAxis];

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
                        negativeMax = math.max(negativeMax, tree.points[tree.permutation[i]][splitAxis]);
                    }

                    return negativeMax;
                }
                else
                {
                    float positiveMin = float.MaxValue;

                    for (int i = start; i < end; i++)
                    {
                        positiveMin = math.min(positiveMin, tree.points[tree.permutation[i]][splitAxis]);
                    }

                    return positiveMin;
                }
            }

            private void ResetPermutation()
            {
                for (int i = 0; i < tree.permutation.Length; i++)
                {
                    tree.permutation[i] = i;
                }
            }

            private int GetNode()
            {
                if (tree.nodeCount >= tree.nodes.Length)
                {
                    tree.nodes.Resize((int)(tree.nodes.Length * 1.5), NativeArrayOptions.UninitializedMemory);
                }
                ResetNode(tree.nodeCount);
                return tree.nodeCount++;
            }

            private void ResetNode(int nodeIndex)
            {
                tree.nodes[nodeIndex] = new KDTree_Array.Node
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

                int evenCount = tree.points.Length >> 1;

                for (int i = 0; i < evenCount; i += 2)
                {
                    int j = i + 1;

                    if (tree.points[i].x > tree.points[j].x)
                    {
                        min.x = math.min(min.x, tree.points[j].x);
                        max.x = math.max(max.x, tree.points[i].x);
                    }
                    else
                    {
                        min.x = math.min(min.x, tree.points[i].x);
                        max.x = math.max(max.x, tree.points[j].x);
                    }

                    if (tree.points[i].y > tree.points[j].y)
                    {
                        min.y = math.min(min.y, tree.points[j].y);
                        max.y = math.max(max.y, tree.points[i].y);
                    }
                    else
                    {
                        min.y = math.min(min.y, tree.points[i].y);
                        max.y = math.max(max.y, tree.points[j].y);
                    }

                    if (tree.points[i].z > tree.points[j].z)
                    {
                        min.z = math.min(min.z, tree.points[j].z);
                        max.z = math.max(max.z, tree.points[i].z);
                    }
                    else
                    {
                        min.z = math.min(min.z, tree.points[i].z);
                        max.z = math.max(max.z, tree.points[j].z);
                    }
                }

                if ((tree.points.Length & 1) == 1)
                {
                    min = math.min(min, tree.points[tree.points.Length - 1]);
                    max = math.max(max, tree.points[tree.points.Length - 1]);
                }

                return new KDBounds
                {
                    minBounds = min,
                    maxBounds = max
                };                
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

        public KDTree_Array(Allocator allocator)
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

            int startIndex = this.points.Length - points.Length;
            for (int i = 0; i < points.Length; i++)
            {
                this.points[startIndex + i] = points[i];
            }

            if (rebuild)
            {
                Rebuild();
            }
        }

        public void Rebuild()
        {
            BuildTreeJob buildTreeJob = new BuildTreeJob
            {
                tree = this
            };

            buildTreeJob.Schedule().Complete();
        }
    }
}

#endif // UNITY_STANDALONE