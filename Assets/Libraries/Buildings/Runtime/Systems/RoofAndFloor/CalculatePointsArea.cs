using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Geometry.Components.Tags;
using FunkySheep.Geometry.Components;
using UnityEngine;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(CalculatePointsCoordinates))]
    [UpdateBefore(typeof(CreateVertexList))]
    public partial class CalculatePointsArea : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<Points> points, in SetPointsArea setPointsArea) =>
            {
                float area = 0;

                for (int i = 0; i < points.Length; i++)
                {
                    area += Vector2.Distance(points[i].ToXY(), points[(i + 1) % points.Length].ToXY());
                }

                building.area = area;

                buffer.RemoveComponent<SetPointsArea>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
