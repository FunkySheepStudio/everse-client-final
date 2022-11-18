using FunkySheep.Buildings.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Geometry.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using FunkySheep.LevelOfDetail.Components.Tags;

namespace FunkySheep.Buildings.Systems
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(WallsSystemGroup))]
    public partial class SetWallsStructure : SystemBase
    {
        EntityQuery query;
        protected override void OnUpdate()
        {
            query = GetEntityQuery(ComponentType.ReadOnly<FunkySheep.Buildings.Components.Tags.Prefab>());
            NativeArray<Entity> prefabs = query.ToEntityArray(Allocator.TempJob);

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in DynamicBuffer<Points> points, in Building building, in LodOk lodOk) =>
            {
                if (prefabs.Length > 0)
                {
                    float buildingHeight = building.maxHeight + building.perimeter;

                    for (int i = 0; i < points.Length; i++)
                    {
                        // Min and max point on terrain
                        float maxPointHeight;
                        float minPointHeight;
                        if (points[i].Value.y < points[(i + 1) % points.Length].Value.y)
                        {
                            minPointHeight = points[i].Value.y;
                            maxPointHeight = points[(i + 1) % points.Length].Value.y;
                        }
                        else
                        {
                            minPointHeight = points[(i + 1) % points.Length].Value.y;
                            maxPointHeight = points[i].Value.y;
                        }

                        // Wall width
                        float wallWidth = math.distance(
                            points[i].Value * new float3(1, 0, 1),
                            points[(i + 1) % points.Length].Value * new float3(1, 0, 1)
                        );

                        // Rotation
                        Quaternion LookAtRotation = Quaternion.identity;
                        Quaternion LookAtRotationOnly_Y;
                        float3 relativePos = points[i].Value - points[(i + 1) % points.Length].Value;
                        if (!relativePos.Equals(float3.zero))
                            LookAtRotation = Quaternion.LookRotation(relativePos);
                        LookAtRotationOnly_Y = Quaternion.Euler(0, LookAtRotation.eulerAngles.y, 0);

                        // Grid
                        int gridSizeX = (int)math.floor(wallWidth);
                        int gridSizeZ = (int)math.floor(buildingHeight - maxPointHeight);

                        // Create the first floor
                        float3 position = new float3
                        {
                            x = (points[i].Value.x + points[(i + 1) % points.Length].Value.x) / 2,
                            y = minPointHeight,
                            z = (points[i].Value.z + points[(i + 1) % points.Length].Value.z) / 2
                        };

                        float4x4 transform = float4x4.TRS(
                            position,
                            LookAtRotationOnly_Y,
                            new float3(wallWidth, maxPointHeight - minPointHeight + 20, wallWidth)
                        );

                        Entity firstFloor = buffer.Instantiate(prefabs[0]);
                        buffer.AddComponent<Parent>(firstFloor, new Parent { Value = entity });
                        buffer.SetComponent<LocalToWorld>(firstFloor, new LocalToWorld
                        {
                            Value = transform
                        });

                        // Create the last floor
                        float lastFloorHeight = buildingHeight - maxPointHeight - gridSizeZ;

                        position.y = gridSizeZ + maxPointHeight;
                        transform = float4x4.TRS(
                            position,
                            LookAtRotationOnly_Y,
                            new float3(wallWidth, lastFloorHeight, wallWidth)
                        );
                        Entity lastFloor = buffer.Instantiate(prefabs[0]);
                        buffer.AddComponent<Parent>(lastFloor, new Parent { Value = entity });
                        buffer.SetComponent<LocalToWorld>(lastFloor, new LocalToWorld
                        {
                            Value = transform
                        });

                        // Fill with windows
                        for (int z = 0; z < gridSizeZ; z += 20)
                        {
                            position.y = maxPointHeight + z;

                            transform = float4x4.TRS(
                                position,
                                LookAtRotationOnly_Y,
                                new float3(wallWidth, 20, wallWidth)
                            );
                            Entity floor = buffer.Instantiate(prefabs[0]);
                            buffer.AddComponent<Parent>(floor, new Parent { Value = entity });
                            buffer.SetComponent<LocalToWorld>(floor, new LocalToWorld
                            {
                                Value = transform
                            });
                        }
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
