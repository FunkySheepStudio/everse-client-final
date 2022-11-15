using Unity.Entities;
using FunkySheep.Geometry;
using FunkySheep.Geometry.Components;
using FunkySheep.Buildings.Components.Barriers;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(RoofSystemGroup))]
    public partial class CreateVertexList : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, DynamicBuffer<Points> points) =>
            {
                // Set the vertex array
                DynamicBuffer<Vertices> vertices = buffer.AddBuffer<Vertices>(entity);
                DynamicBuffer<Ears> ears = buffer.AddBuffer<Ears>(entity);
                DynamicBuffer<Triangles> triangles = buffer.AddBuffer<Triangles>(entity);

                for (int i = 0; i < points.Length; i++)
                {
                    vertices.Add(new Vertices
                    {
                        Value = i
                    });
                }
            })
            .WithNone<RoofMeshCreated>()
            .WithNone<Vertices>()
            .WithNone<Ears>()
            .WithNone<Triangles>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
