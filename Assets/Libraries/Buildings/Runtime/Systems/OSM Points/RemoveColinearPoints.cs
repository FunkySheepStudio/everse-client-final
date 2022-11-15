using FunkySheep.Buildings.Components;
using Unity.Entities;
using FunkySheep.Earth.Components;
using FunkySheep.Geometry.Components.Tags;
using FunkySheep.Geometry.Components;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(OSMSystemGroup))]
    public partial class RemoveCollinearPoints : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref DynamicBuffer<GPSCoordinates> gPSCoordinates, in Building building, in SetRemoveColinearPoints setRemoveColinearPoints) =>
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

                if (gPSCoordinates.Length < 3)
                {
                    buffer.DestroyEntity(entity);
                    return;
                }

                buffer.RemoveComponent<SetRemoveColinearPoints>(entity);
            })
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
