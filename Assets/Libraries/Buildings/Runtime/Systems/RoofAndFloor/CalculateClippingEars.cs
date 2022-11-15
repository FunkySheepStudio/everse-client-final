using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Geometry;
using FunkySheep.Geometry.Components;
using FunkySheep.Collections;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(RoofSystemGroup))]
    [UpdateAfter(typeof(CreateVertexList))]
    public partial class CalculateClippingEars : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, DynamicBuffer<Vertices> vertices, DynamicBuffer<Ears> ears, DynamicBuffer<Points> points) =>
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    float2 prev = points[vertices[Utils.ClampListIndex(i - 1, vertices.Length)].Value].ToXY();
                    float2 current = points[vertices[Utils.ClampListIndex(i, vertices.Length)].Value].ToXY();
                    float2 next = points[vertices[Utils.ClampListIndex(i + 1, vertices.Length)].Value].ToXY();

                    if (EarClipping.IsVertexEar(prev, current, next, vertices, points))
                    {
                        ears.Add(new Ears
                        {
                            Value = i
                        });
                        return;
                    }
                }
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
