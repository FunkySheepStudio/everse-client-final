using FunkySheep.Images.Components;
using FunkySheep.Maps.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FunkySheep.Trees.Systems
{
    public partial class Spawn : SystemBase
    {
        EntityQuery query;
        protected override void OnUpdate()
        {
            query = GetEntityQuery(ComponentType.ReadOnly<Components.Tags.Prefab>());
            NativeArray<Entity> prefabs = query.ToEntityArray(Allocator.TempJob);

            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, in DynamicBuffer<Pixels> pixels, in Components.MapPosition mapPosition) =>
            {
                InitialMapPosition initialMapPosition;
                if (!TryGetSingleton<InitialMapPosition>(out initialMapPosition))
                    return;

                TileSize tileSize;
                if (!TryGetSingleton<TileSize>(out tileSize))
                    return;

                int2 tilePosition = mapPosition.Value - (int2)math.floor(initialMapPosition.Value);

                for (int i = 0; i < pixels.Length; i++)
                {
                    float3 position = new float3
                    {
                        x = (tilePosition.x + (i / 256)) * tileSize.Value,
                        z = (-(tilePosition.y + ((i % 256) / 256)) * tileSize.Value)
                    };

                    float? height = Terrain.Utils.GetHeight(position);
                    if (height == null)
                    {
                        return;
                    }
                    position.y = height.Value;


                    Entity tree = buffer.Instantiate(prefabs[0]);

                    float4x4 transform = float4x4.TRS(
                            position,
                            quaternion.identity,
                            new float3(1, 1, 1)
                        );
                    buffer.AddComponent(entity, new LocalToWorld
                    {
                        Value = transform
                    });
                }

                buffer.RemoveComponent<Pixels>(entity);
            })
            .WithNativeDisableParallelForRestriction(prefabs)
            .WithDisposeOnCompletion(prefabs)
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .WithoutBurst()
            .Run();
        }
    }
}
