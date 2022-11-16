using FunkySheep.LevelOfDetail.Components.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FunkySheep.LevelOfDetail
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class CheckDistance : SystemBase
    {
        EntityQuery query;
        protected override void OnUpdate()
        {
            query = GetEntityQuery(
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<CurrentPosition>()
                );


            NativeArray<LocalToWorld> currentPositions = query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in Components.Distance distance, in LocalToWorld localToWorld) =>
            {
                for (int i = 0; i < currentPositions.Length; i++)
                {
                    if (math.distance(localToWorld.Position, currentPositions[i].Position) < distance.Value)
                    {
                        buffer.AddComponent<LodOk>(entity);
                        return;
                    }
                }
            })
            .WithNone<LodOk>()
            .WithNativeDisableParallelForRestriction(currentPositions)
            .WithDisposeOnCompletion(currentPositions)
            .WithDeferredPlaybackSystem<EndInitializationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
