using Unity.Entities;
using Unity.Physics;

namespace GalacticBoundStudios.HexTech.Testing
{
    public struct HexTechInputData : IComponentData
    {
        // This data is used to determine where the camera is looking
        public RaycastInput cameraRayInput;
    }
}