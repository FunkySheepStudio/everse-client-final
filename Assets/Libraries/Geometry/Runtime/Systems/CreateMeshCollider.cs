using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;
using FunkySheep.Geometry.Components.Tags;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;

namespace FunkySheep.Geometry
{
    public partial class CreateMeshCollider : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
            .WithStructuralChanges()
            .WithAll<ApplyMeshCollider>()
            .ForEach((Entity E, in RenderMesh RM) => {

                NativeArray<BlobAssetReference<Unity.Physics.Collider>> BlobCollider = new NativeArray<BlobAssetReference<Unity.Physics.Collider>>(1, Allocator.TempJob);
                NativeArray<float3> NVerts = new NativeArray<Vector3>(RM.mesh.vertices, Allocator.TempJob).Reinterpret<float3>();
                NativeArray<int> NTris = new NativeArray<int>(RM.mesh.triangles, Allocator.TempJob);

                CreateMeshColliderJob CMCJ = new CreateMeshColliderJob { MeshVerts = NVerts, MeshTris = NTris, BlobCollider = BlobCollider };
                CMCJ.Run();

                EntityManager.AddComponentData(E, new PhysicsCollider { Value = BlobCollider[0] });
                NVerts.Dispose();
                NTris.Dispose();
                BlobCollider.Dispose();

                EntityManager.RemoveComponent<ApplyMeshCollider>(E);

            }).Run();
        }
    }

    [BurstCompile]
    public struct CreateMeshColliderJob : IJob
    {

        [ReadOnly] public NativeArray<float3> MeshVerts;
        [ReadOnly] public NativeArray<int> MeshTris;

        public NativeArray<BlobAssetReference<Unity.Physics.Collider>> BlobCollider;

        public void Execute()
        {

            NativeArray<float3> CVerts = new NativeArray<float3>(MeshVerts.Length, Allocator.Temp);
            NativeArray<int3> CTris = new NativeArray<int3>(MeshTris.Length / 3, Allocator.Temp);

            for (int i = 0; i < MeshVerts.Length; i++) { CVerts[i] = MeshVerts[i]; }
            int ii = 0;
            for (int j = 0; j < MeshTris.Length; j += 3)
            {
                CTris[ii++] = new int3(MeshTris[j], MeshTris[j + 1], MeshTris[j + 2]);
            }

            BlobCollider[0] = Unity.Physics.MeshCollider.Create(CVerts, CTris);
            CVerts.Dispose();
            CTris.Dispose();

        }
    }
}
