using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Geometry.Components;
using Unity.Mathematics;
using Unity.Transforms;
using FunkySheep.Earth.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(OSMSystemGroup))]
    [UpdateAfter(typeof(CalculatePointsPerimeter))]
    public partial class SetBuildingWorldPosition : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in DynamicBuffer<Points> points, in Building building) =>
            {
                float3 position = new float3 { };
                for (int i = 0; i < points.Length; i++)
                {
                    position += points[i].Value;
                }
                position /= points.Length;
                buffer.AddComponent<LocalToWorld>(entity);

                float4x4 transform = float4x4.TRS(
                            position,
                            quaternion.identity,
                            new float3(1, 1, 1)
                        );

                buffer.SetComponent<LocalToWorld>(entity, new LocalToWorld
                {
                    Value = transform
                });

                buffer.AddComponent<OsmPointsCalculationOver>(entity);
            })
            .WithNone<GPSCoordinates>()
            .WithNone<OsmPointsCalculationOver>()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
