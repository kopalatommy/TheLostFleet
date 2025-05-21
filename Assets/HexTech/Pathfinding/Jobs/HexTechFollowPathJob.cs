using GalacticBoundStudios.HexTech.MapGeneration;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GalacticBoundStudios.HexTech.PathFinding
{
    public partial struct HexTechFollowPathJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly]
        public HexMapTransformData mapConfig;

        // The amount of time since the last frame
        public float deltaTime;

        public void Execute(Entity e,
            ref DynamicBuffer<HexTechMapPath> pathBuffer,
            ref LocalTransform localTransform)
        {
            if (!pathBuffer.IsEmpty)
            {
                HexCoord targetPos = pathBuffer[0].Value;

                Debug.Log("Moving entity to " + targetPos + " DeltaTime: " + deltaTime);

                float2 pos = HexMath.HexToPixel(targetPos, in mapConfig);
                float3 worldPos = new float3(pos.x, 0, pos.y);

                float3 dir = worldPos - localTransform.Position;

                Debug.Log("Distance to point: " + math.length(dir));
                // If close to point, snap to the position
                if (math.length(dir) < deltaTime)
                {
                    LocalTransform newTransform = localTransform;
                    newTransform.Position = worldPos;
                    ecb.SetComponent(e.Index, e, newTransform);

                    pathBuffer.RemoveAt(0);
                }
                else
                {
                    dir = math.normalize(dir) * deltaTime;

                    LocalTransform newTransform = localTransform;
                    newTransform.Position += dir;
                    ecb.SetComponent(e.Index, e, newTransform);
                }


            }

            if (pathBuffer.IsEmpty)
            {
                Debug.Log("Entity finished path");
                ecb.RemoveComponent<HexTechMapPath>(e.Index, e);
            }
        }
    }
}