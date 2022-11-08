using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Buildings.Components;
using FunkySheep.Buildings.Components.Tags;
using FunkySheep.Earth.Components;
using FunkySheep.Maps.Components;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;
using System.Linq;

namespace FunkySheep.Buildings.Systems
{
    public partial class CalculatePointsCoordinates : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<GPSCoordinates> gPSCoordinates) =>
            {
                InitialMapPosition initialMapPosition;
                if (!TryGetSingleton<InitialMapPosition>(out initialMapPosition))
                    return;

                TileSize tileSize;
                if (!TryGetSingleton<TileSize>(out tileSize))
                    return;

                ZoomLevel zoomLevel;
                if (!TryGetSingleton<ZoomLevel>(out zoomLevel))
                    return;

                if (gPSCoordinates.Length < 3)
                {
                    buffer.DestroyEntity(entity);
                    return;
                }

                // Discard the change if the building is on a terrain that do not exist
                for (int i = 0; i < gPSCoordinates.Length - 1; i++)
                {
                    float3 point = Earth.Utils.GpsToMapRealRelative(gPSCoordinates[i].Value.Value, zoomLevel.Value, initialMapPosition.Value);
                    float? height = Terrain.Utils.GetHeight(point);
                    if (height == null)
                    {
                        return;
                    }
                }

                DynamicBuffer<Points> points = buffer.AddBuffer<Points>(entity);

                for (int i = 0; i < gPSCoordinates.Length; i++)
                {
                    float3 point = Earth.Utils.GpsToMapRealRelative(gPSCoordinates[i].Value.Value, zoomLevel.Value, initialMapPosition.Value) * tileSize.Value;
                    float? height = Terrain.Utils.GetHeight(point);
                    if (height != null)
                    {
                        point.y = height.Value;
                        points.Add(new Points
                        {
                            Value = point
                        });

                        if (i == 0)
                        {
                            building.minHeight = point.y;
                            building.maxHeight = point.y;
                        } else if (point.y < building.minHeight)
                        {
                            building.minHeight = point.y;
                        } else if (point.y > building.maxHeight)
                        {
                            building.maxHeight = point.y;
                        }

                        building.center += point;
                    }
                }

                building.center /= points.Length;

                buffer.RemoveComponent<GPSCoordinates>(entity);
                buffer.SetComponentEnabled<Walls>(entity, true);
            })
            .WithoutBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .Run();

        }

        public void OnDrawGizmos()
        {
            Entities.ForEach((in Walls wall, in DynamicBuffer<Points> points) =>
            {
                Gizmos.color = Color.red;

                for (int i = 0; i < points.Length; i++)
                {
                    Gizmos.DrawLine(points[i].Value, points[(i + 1) % points.Length].Value);
                }
            })
            .WithoutBurst()
            .Run();
        }
    }
}
