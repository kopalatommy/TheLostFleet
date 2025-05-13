using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace GalacticBoundStudios.MeshMania
{
    public struct AddMeshFlag : IComponentData
    {

    }

    public partial class AddMeshSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<AddMeshFlag>();
        }

        protected override void OnUpdate()
        {
            foreach ((AddMeshFlag flag, Entity entity) in SystemAPI.Query<AddMeshFlag>().WithEntityAccess())
            {
                Mesh mesh = new Mesh();
                mesh.vertices = new[] { Vector3.up, Vector3.left, Vector3.right };
                mesh.triangles = new[] { 0, 1, 2 };

                // Create a RenderMeshDescription using the convenience constructor
                // with named parameters.
                RenderMeshDescription desc = new RenderMeshDescription(
                    shadowCastingMode: ShadowCastingMode.Off,
                    receiveShadows: false,
                    renderingLayerMask: 1);

                // Create an array of mesh and material required for runtime rendering.
                RenderMeshArray renderMeshArray = new RenderMeshArray(new Material[] { new Material(Shader.Find("HexTech/HexGridHighlightURPv2")) }, new Mesh[] { mesh });

                RenderMeshUtility.AddComponents(
                    entity,
                    EntityManager,
                    desc,
                    renderMeshArray,
                    MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

                EntityManager.AddComponentData(entity, new MeshIndexData
                {
                    index = mesh.GetInstanceID()
                });

                EntityManager.RemoveComponent<AddMeshFlag>(entity);
            }

        }
    }
}