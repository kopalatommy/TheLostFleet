using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace GalacticBoundStudios.MeshMania
{
    public partial class DrawMeshSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<DrawMeshTag>();
        }

        protected override void OnUpdate()
        {
            Debug.Log("Drawing Meshes");

            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            foreach ((RefRO<MeshData> meshData, Entity entity) in SystemAPI.Query<RefRO<MeshData>>().WithEntityAccess())
            {
                Debug.Log("Drawing Mesh on entity");

                DrawMesh(entity, meshData, ref ecb);

                ecb.RemoveComponent<DrawMeshTag>(entity);
            }

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }

        protected void DrawMesh(Entity entity, RefRO<MeshData> meshData, ref EntityCommandBuffer ecb)
        {
            MeshIndexData meshIndex = EntityManager.GetComponentData<MeshIndexData>(entity);

            if (!TryFindMesh(in meshIndex, out UnityObjectRef<Mesh> mesh, entity))
            {
                Debug.Log("Failed to find mesh");
                return;
            }

            Debug.Log("Found mesh id. Drawing mesh");

            Vector3[] vertsArr = new Vector3[meshData.ValueRO.vertices.Length];
            int[] trisArr = new int[meshData.ValueRO.triangles.Length];
            Color[] colorsArr = new Color[meshData.ValueRO.colors.Length];

            Debug.Log("Number of verts: " + vertsArr.Length);
            Debug.Log("Number of tris: " + trisArr.Length);

            for (int i = 0; i < meshData.ValueRO.vertices.Length; i++)
            {
                vertsArr[i] = meshData.ValueRO.vertices[i];
            }
            for (int i = 0; i < meshData.ValueRO.triangles.Length; i++)
            {
                trisArr[i] = meshData.ValueRO.triangles[i];

                if (trisArr[i] >= vertsArr.Length)
                {
                    Debug.Log("Triangle index out of bounds: " + trisArr[i]);
                }
            }
            for (int i = 0; i < meshData.ValueRO.colors.Length; i++)
            {
                colorsArr[i] = new Color(meshData.ValueRO.colors[i].x, meshData.ValueRO.colors[i].y, meshData.ValueRO.colors[i].z, meshData.ValueRO.colors[i].w);
            }

            mesh.Value.Clear();
            mesh.Value.vertices = vertsArr;
            mesh.Value.triangles = trisArr;
            mesh.Value.colors = colorsArr;

            mesh.Value.RecalculateNormals();
            mesh.Value.RecalculateBounds();
            mesh.Value.Optimize();

            meshData.ValueRO.vertices.Dispose();
            meshData.ValueRO.triangles.Dispose();
            meshData.ValueRO.colors.Dispose();
        }

        protected bool TryFindMesh(in MeshIndexData meshIndex, out UnityObjectRef<Mesh> mesh, Entity entity)
        {
            UnityObjectRef<Mesh>[] meshes = EntityManager.GetSharedComponentManaged<RenderMeshArray>(entity).MeshReferences;

            for (int i = 0; i < meshes.Length; i++)
            {
                if (meshes[i].Value.GetInstanceID() == meshIndex.index)
                {
                    mesh = meshes[i].Value;
                    return true;
                }
            }

            mesh = default;
            return false;
        }
    }
}