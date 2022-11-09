using Unity.Entities;
using FunkySheep.Geometry;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(CalculatePointsCoordinates))]
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
            .WithNone<Vertices>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
