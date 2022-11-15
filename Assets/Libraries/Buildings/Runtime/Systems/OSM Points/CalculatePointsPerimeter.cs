using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Geometry.Components.Tags;
using FunkySheep.Geometry.Components;
using UnityEngine;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(OSMSystemGroup))]
    [UpdateAfter(typeof(CalculatePointsCoordinates))]
    public partial class CalculatePointsPerimeter : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<Points> points, in SetPointsPerimeter setPointsPerimeter) =>
            {
                float perimeter = 0;

                for (int i = 0; i < points.Length; i++)
                {
                    perimeter += Vector2.Distance(points[i].ToXY(), points[(i + 1) % points.Length].ToXY());
                }

                building.perimeter = perimeter;

                buffer.RemoveComponent<SetPointsPerimeter>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
