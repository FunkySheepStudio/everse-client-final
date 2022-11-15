using FunkySheep.Buildings.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Geometry.Components;
using FunkySheep.Geometry.Components.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FunkySheep.Buildings.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(WallsSystemGroup))]
    public partial class CreateWalls : SystemBase
    {
        EntityQuery query;
        protected override void OnUpdate()
        {
            query = GetEntityQuery(ComponentType.ReadOnly<FunkySheep.Buildings.Components.Tags.Prefab>());
            NativeArray<Entity> prefabs = query.ToEntityArray(Allocator.TempJob);

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in DynamicBuffer<Points> points, in Building building) =>
            {
                if (prefabs.Length > 0)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        float3 relativePos;
                        Quaternion LookAtRotation = Quaternion.identity;
                        Quaternion LookAtRotationOnly_Y;
                        float4x4 transform;

                        Entity wall = buffer.Instantiate(prefabs[0]);
                        buffer.AddComponent<Parent>(wall, new Parent { Value = entity });

                        float3 position;
                        if (points[i].Value.y < points[(i + 1) % points.Length].Value.y)
                        {
                            position.y = points[i].Value.y;
                        }
                        else
                        {
                            position.y = points[(i + 1) % points.Length].Value.y;
                        }

                        position.x = (points[i].Value.x + points[(i + 1) % points.Length].Value.x) / 2;
                        position.z = (points[i].Value.z + points[(i + 1) % points.Length].Value.z) / 2;

                        float wallWidth = math.distance(
                            points[i].Value * new float3(1, 0, 1),
                            points[(i + 1) % points.Length].Value * new float3(1, 0, 1)
                        );

                        relativePos = points[i].Value - points[(i + 1) % points.Length].Value;
                        if (!relativePos.Equals(float3.zero))
                            LookAtRotation = Quaternion.LookRotation(relativePos);
                        LookAtRotationOnly_Y = Quaternion.Euler(0, LookAtRotation.eulerAngles.y, 0);
                        //buffer.RemoveComponent<LocalToWorldTransform>(wall);
                        transform = float4x4.TRS(
                            position,
                            LookAtRotationOnly_Y,
                            new float3(wallWidth, building.maxHeight - position.y + building.perimeter, wallWidth)
                        );
                        buffer.SetComponent<LocalToWorld>(wall, new LocalToWorld
                        {
                            Value = transform
                        });
                    }
                }

                buffer.AddComponent<WallsCreated>(entity);
            })
            .WithNone<WallsCreated>()
            .WithNativeDisableParallelForRestriction(prefabs)
            .WithDisposeOnCompletion(prefabs)
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .ScheduleParallel();
        }
    }
}
