using Unity.Entities;
using Unity.Transforms;

namespace GalacticBoundStudios.Utilities
{
    public struct FollowParentData : IComponentData
    {
        public Entity parentEntity;
        public LocalTransform offset;
    }
}