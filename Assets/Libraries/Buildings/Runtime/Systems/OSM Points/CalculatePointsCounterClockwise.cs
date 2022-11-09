using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Earth.Components;
using FunkySheep.Geometry.Components.Tags;
using Unity.Collections;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(CalculatePointsOrder))]
    public partial class CalculatePointsCounterClockwise : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<GPSCoordinates> gPSCoordinates, in Building building, in SetPointsCounterClockWise setPointsCounterClockWise) =>
            {
                bool result = Geometry.utils.IsTriangleOrientedClockwise(
                    gPSCoordinates[gPSCoordinates.Length - 1].Value.Value,
                    gPSCoordinates[0].Value.Value,
                    gPSCoordinates[1].Value.Value
                );

                if (!result)
                {
                    NativeArray<GPSCoordinates> tempPoints = new NativeArray<GPSCoordinates>(gPSCoordinates.Length, Allocator.Temp);
                    tempPoints.CopyFrom(gPSCoordinates.AsNativeArray());
                    gPSCoordinates.Clear();

                    for (int i = tempPoints.Length - 1; i >= 0; i--)
                    {
                        gPSCoordinates.Add(tempPoints[i]);
                    }
                }

                buffer.RemoveComponent<SetPointsCounterClockWise>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
