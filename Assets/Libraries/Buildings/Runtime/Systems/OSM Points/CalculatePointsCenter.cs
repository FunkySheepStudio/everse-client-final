using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Earth.Components;
using FunkySheep.Geometry.Components.Tags;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(RemoveCollinearPoints))]
    public partial class CalculatePointsCenter : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<GPSCoordinates> gPSCoordinates, in SetPointsCenter setPointsCenter) =>
            {
                double2 center = new double2();
                for (int i = 0; i < gPSCoordinates.Length; i++)
                {
                    center += gPSCoordinates[i].Value.Value;
                }

                center /= gPSCoordinates.Length;

                building.center = center;

                buffer.RemoveComponent<SetPointsCenter>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
