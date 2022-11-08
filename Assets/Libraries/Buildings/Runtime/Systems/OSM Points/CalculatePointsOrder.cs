using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Earth.Components;
using FunkySheep.Geometry.Components.Tags;
using Unity.Collections;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(CalculatePointsCenter))]
    public partial class CalculatePointsOrder : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<GPSCoordinates> gPSCoordinates, in Building building, in SetPointsOrder setPointsOrder) =>
            {
                int maxPointIndex = 0;
                for (int i = 0; i < gPSCoordinates.Length; i++)
                {
                    if (math.distance(building.center, gPSCoordinates[maxPointIndex].Value.Value) < math.distance(building.center, gPSCoordinates[i].Value.Value))
                    {
                        maxPointIndex = i;
                    }
                }

                NativeArray<GPSCoordinates> tempPoints = new NativeArray<GPSCoordinates>(gPSCoordinates.Length, Allocator.Temp);
                tempPoints.CopyFrom(gPSCoordinates.AsNativeArray());
                gPSCoordinates.Clear();

                for (int i = 0; i < tempPoints.Length; i++)
                {
                    gPSCoordinates.Add(tempPoints[(i + maxPointIndex) % tempPoints.Length]);
                }

                buffer.SetComponentEnabled<SetPointsOrder>(entity, false);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
