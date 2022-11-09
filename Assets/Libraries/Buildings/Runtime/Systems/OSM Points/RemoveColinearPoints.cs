using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Earth.Components;
using FunkySheep.Geometry.Components.Tags;

namespace FunkySheep.Buildings.Systems
{
    public partial class RemoveCollinearPoints : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<GPSCoordinates> gPSCoordinates, in Building building, in RemoveColinearPoints removeColinearPoints) =>
            {
                for (int i = 0; i < gPSCoordinates.Length; i++)
                {
                    if (Geometry.utils.IsCollinear(
                        gPSCoordinates[i].Value.Value,
                        gPSCoordinates[(i + 1) % gPSCoordinates.Length].Value.Value,
                        gPSCoordinates[(i + 2) % gPSCoordinates.Length].Value.Value
                        ))
                    {
                        gPSCoordinates.RemoveAt((i + 1) % gPSCoordinates.Length);
                    }
                }

                buffer.RemoveComponent<RemoveColinearPoints>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
