using GalacticBoundStudios.DataScribes.Managed;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    [BurstCompile]
    public struct GeneratePathJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;
        public ComponentTypeHandle<HexTechCreatePathRequest> HexTechCreatePathRequestTypeHandle;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var requests = chunk.GetNativeArray(HexTechCreatePathRequestTypeHandle);
        }

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
        {
            throw new System.NotImplementedException();
        }
    }

    // Uses A* algorithm to calculate the path between 2 points
    [BurstCompile]
    public struct HexTechGeneratePathJob : IJobParallelFor
    {
        struct PathStep: System.IComparable<PathStep>
        {
            public HexCoord coord;
            public float currentCost;

            public int CompareTo(PathStep other)
            {
                return currentCost.CompareTo(other.currentCost);
            }
        }

        // This is how we know if a tile is walkable
        [ReadOnly]
        public NativeHashMap<HexCoord, HexTechWalkable> moveData;
        [ReadOnly]
        public NativeArray<HexTechCreatePathRequest> requests;

        // This is used to write the path to the target entity
        public EntityCommandBuffer ecb;

        // Queue of locations to process, priority queue based on the heuristic cost with
        // cheaper
        private NativePriorityQueue<PathStep> buildQueue;
        private NativeHashMap<HexCoord, HexCoord> pathSteps;

        public void Execute(int reqIndex)
        {
            buildQueue = new NativePriorityQueue<PathStep>(Allocator.TempJob);

            ecb.AddComponent(requests[reqIndex].entity, new HexTechPathData()
            {
                path = ProcessRequest(requests[reqIndex])
            });

            buildQueue.Dispose();
            pathSteps.Dispose();
        }

        private NativeList<HexCoord> ProcessRequest(in HexTechCreatePathRequest req)
        {
            buildQueue.Add(new PathStep()
            {
                coord = req.startPos,
                currentCost = 0
            });

            while (!buildQueue.IsEmpty)
            {
                PathStep currentStep = buildQueue.Dequeue();

                if (currentStep.coord.Equals(req.endPos)) {
                    return RebuildPath(req.endPos, req.startPos);
                }
                // If the current coord has already been accessed, then skip this node
                // Based on how the algorithm works, the first time that a coord is accessed
                // will be the shortest dist
                if (pathSteps.ContainsKey(currentStep.coord)) {
                    continue;
                }

                NativeArray<HexCoord> neighbors = HexMath.Neighbors(currentStep.coord);

                // Add each neighbor that has not yet been reached to the build queue
                for (int i = 0; i < 6; i++) {
                    // If the path steps map already contains a def for the coord, it has already been added. So skip it
                    if (pathSteps.ContainsKey(neighbors[i])) {
                        continue;
                    }

                    // The cost to reach this location
                    float gCost = currentStep.currentCost;
                    // The estimated cost to reach the end position
                    float hCost = HexMath.Distance(neighbors[i], req.endPos);
                    // The total cost of the node
                    float fCost = gCost + hCost;

                    pathSteps.Add(neighbors[i], currentStep.coord);
                    buildQueue.Add(new PathStep()
                    {
                        coord = neighbors[i],
                        currentCost = fCost
                    });
                }
            }
            return new NativeList<HexCoord>(Allocator.Persistent);
        }

        private NativeList<HexCoord> RebuildPath(HexCoord currentCoord, HexCoord startPos)
        {
            // Create as persistent b/c this will be given to the unit
            NativeList<HexCoord> path = new NativeList<HexCoord>(Allocator.Persistent);

            path.Add(currentCoord);

            while (!currentCoord.Equals(startPos)) {
                currentCoord = pathSteps[currentCoord];
                path.Add(currentCoord);
            }

            return path;
        }
    }
}