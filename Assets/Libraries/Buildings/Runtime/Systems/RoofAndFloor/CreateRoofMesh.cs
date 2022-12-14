using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using FunkySheep.Geometry;
using FunkySheep.Geometry.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Buildings.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(RoofSystemGroup))]
    [UpdateAfter(typeof(TriangulateRoof))]
    public partial class CreateRoofMesh : SystemBase
    {
        public Material material;

        protected override void OnCreate()
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, in Building building, in DynamicBuffer<Triangles> triangles, in DynamicBuffer<Points> points) =>
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                NativeArray<Points> flatPoints = new NativeArray<Points>(points.Length, Allocator.Temp);
                NativeArray<Uvs> uvs = new NativeArray<Uvs>(points.Length, Allocator.Temp);

                Vector3 minPoint = new float3
                {
                    x = points[0].Value.x,
                    y = 0,
                    z = points[0].Value.z
                };

                Vector3 maxPoint = minPoint;

                for (int i = 0; i < points.Length; i++)
                {
                    flatPoints[i] = new Points
                    {
                        Value = new float3
                        {
                            x = points[i].Value.x,
                            y = building.maxHeight + building.perimeter,
                            z = points[i].Value.z
                        }
                    };

                    if (Vector3.Distance(minPoint, flatPoints[i].Value) > Vector3.Distance(minPoint, maxPoint))
                        maxPoint = flatPoints[i].Value;
                }

                for (int i = 0; i < flatPoints.Length; i++)
                {
                    uvs[i] = new Uvs
                        {
                            Value = new Vector3(
                                (flatPoints[i].Value.x - minPoint.x) / (maxPoint.x - minPoint.x),
                                (flatPoints[i].Value.y - minPoint.y) / (maxPoint.y - minPoint.y),
                                0
                            )
                        };
                }
                Entity roofEntity = entityManager.CreateEntity();
                Mesh mesh = new Mesh();
                mesh.indexFormat = IndexFormat.UInt32;
                mesh.Clear();
                mesh.SetVertices(flatPoints.Reinterpret<Vector3>());
                mesh.SetIndices(triangles.AsNativeArray(), MeshTopology.Triangles, 0);
                mesh.SetUVs(0, uvs);
                mesh.RecalculateNormals();

                var desc = new RenderMeshDescription(
                    shadowCastingMode: ShadowCastingMode.Off,
                    receiveShadows: false);

                // Create an array of mesh and material required for runtime rendering.
                var renderMeshArray = new RenderMeshArray(new Material[] { material }, new Mesh[] { mesh });

                //var prototype = entityManager.CreateEntity();

                // Call AddComponents to populate base entity with the components required
                // by Entities Graphics
                RenderMeshUtility.AddComponents(
                    roofEntity,
                    entityManager,
                    desc,
                    renderMeshArray,
                    MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0));

                entityManager.AddComponent<Parent>(roofEntity);
                entityManager.SetComponentData<Parent>(roofEntity, new Parent { Value = entity });


                entityManager.AddComponent<LocalToWorld>(roofEntity);
                entityManager.SetComponentData<LocalToWorld>(roofEntity, new LocalToWorld
                {
                    Value = Matrix4x4.identity
                });

                flatPoints.Dispose();
                uvs.Dispose();

                entityManager.RemoveComponent<Triangles>(entity);
                entityManager.AddComponent<RoofMeshCreated>(entity);
            })
            .WithNone<Vertices>()
            .WithNone<Ears>()
            .WithStructuralChanges()
            .Run();
        }
    }
}
