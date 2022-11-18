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

                int2 tilePosition = new int2
                {
                    x = mapPosition.Value.x - (int)math.floor(initialMapPosition.Value.x),
                    y = (int)math.floor(initialMapPosition.Value.y) - mapPosition.Value.y
                };

                for (int i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].Value.g == 173 && pixels[i].Value.b == 209 && pixels[i].Value.a == 158 && i % 4 == 0)
                    {
                        float3 position = new float3
                        {
                            x = (tilePosition.x * tileSize.Value) + (i % 256) * tileSize.Value / 256,
                            z = (tilePosition.y * tileSize.Value) + (i / 256) * tileSize.Value / 256
                        };

                        float? height = Terrain.Utils.GetHeight(position);
                        if (height == null)
                        {
                            return;
                        }
                        position.y = height.Value;


                        Entity tree = buffer.Instantiate(prefabs[0]);
                        buffer.AddComponent(tree, new Components.Tags.Tree { });
                        buffer.SetComponent<Translation>(tree, new Translation { Value = position });

                        buffer.RemoveComponent<Components.Tags.Prefab>(tree);
                    }
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
