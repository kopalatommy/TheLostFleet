using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace GalacticBoundStudios.HexTech.Testing
{
    public partial class HexTechInputDataCollectorSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            RequireForUpdate<HexTechInputData>();
        }

        protected override void OnUpdate()
        {

        }

        
    }
}