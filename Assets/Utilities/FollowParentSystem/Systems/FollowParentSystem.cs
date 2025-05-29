using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GalacticBoundStudios.Utilities
{
    public partial struct FollowParentSystem : ISystem
    {
        private EntityQuery query;

        private void OnCreate(ref SystemState state)
        {
            query = new EntityQueryBuilder(Allocator.Temp).WithAll<FollowParentData>().Build(ref state);

            state.RequireForUpdate(query);
        }

        private void OnUpdate(ref SystemState state)
        {
            NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);

            Debug.Log("FollowParentSystem.Update: " + entities.Length);

            for (int i = 0; i < entities.Length; i++)
            {
                UpdatePosition(entities[i], state.EntityManager.GetComponentData<FollowParentData>(entities[i]), ref state);
            }

            entities.Dispose();
        }

        private void UpdatePosition(Entity targetEntity, FollowParentData followParentData, ref SystemState state)
        {
            // If the entity to follow doesn't exist, exit and remove data
            if (!state.EntityManager.Exists(followParentData.parentEntity))
            {
                state.EntityManager.RemoveComponent<FollowParentData>(targetEntity);
                return;
            }

            LocalTransform parentTransform = state.EntityManager.GetComponentData<LocalTransform>(followParentData.parentEntity);
            // LocalTransform targetTransform = state.EntityManager.GetComponentData<LocalTransform>(targetEntity);

            state.EntityManager.SetComponentData<LocalTransform>(targetEntity, new LocalTransform
            {
                Scale = parentTransform.Scale * followParentData.offset.Scale,
                Position = parentTransform.Position + followParentData.offset.Position,
                Rotation = math.mul(parentTransform.Rotation, followParentData.offset.Rotation)
            });
        }
    }
}