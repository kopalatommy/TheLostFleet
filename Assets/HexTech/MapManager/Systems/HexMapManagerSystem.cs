using GalacticBoundStudios.HexTech.MapGeneration;
using Unity.Entities;

namespace GalacticBoundStudios.HexTech
{
    public partial struct HexMapManagerSystem : ISystem
    {
        void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<HexMapTransformData>();
        }

        void OnDestroy(ref SystemState state)
        {

        }

        void OnUpdate(ref SystemState state)
        {

        }
    }
}