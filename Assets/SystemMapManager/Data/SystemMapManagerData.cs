using System;
using GalacticBoundStudios.HexTech.MapGeneration;
using Unity.Entities;

namespace GalacticBoundStudios.EchoesOfTheFarRim.SystemMap
{
    public struct SystemMapEnableFlag : IComponentData
    {
        public Entity emptyEntityPrefab;
    }

    public struct SystemMapMaterialOverrideData : ISharedComponentData, IEquatable<SystemMapMaterialOverrideData>
    {
        public MaterialOverrideAsset Value;

        public bool Equals(SystemMapMaterialOverrideData other)
        {
            return Value != null && Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value == null ? 0 : Value.GetHashCode();
        }
    }
}