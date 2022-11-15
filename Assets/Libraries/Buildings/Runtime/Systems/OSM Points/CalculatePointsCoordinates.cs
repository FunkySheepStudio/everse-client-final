using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Buildings.Components;
using FunkySheep.Buildings.Components.Barriers;
using FunkySheep.Earth.Components;
using FunkySheep.Maps.Components;
using UnityEngine;
using FunkySheep.Geometry.Components;
using FunkySheep.Terrain.Components.Tags;

namespace FunkySheep.Buildings.Systems
{
    [UpdateInGroup(typeof(OSMSystemGroup))]
    [UpdateAfter(typeof(CalculatePointsCounterClockwise))]
    public partial class CalculatePointsCoordinates : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, EntityCommandBuffer buffer, ref Building building, in DynamicBuffer<GPSCoordinates> gPSCoordinates, in SetCalculatePointsCoordinates setCalculatePointsCoordinates) =>
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
                    }
                }
                buffer.RemoveComponent<GPSCoordinates>(entity);
                buffer.RemoveComponent<SetCalculatePointsCoordinates>(entity);
            })
            .WithoutBurst()
            .WithDeferredPlaybackSystem<EndSimulationEntityCommandBufferSystem>()
            .Run();

        }

        public void OnDrawGizmos()
        {
            /*Entities.ForEach((in DynamicBuffer<Points> points, in Building building, in OsmPointsCalculationOver osmPointsCalculationOver) =>
            {
                for (int i = 0; i < points.Length; i++)
                {
                    if (i == 0)
                    {
                        Gizmos.color = Color.blue;
                    } else
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawCube(points[i].Value, Vector3.one * 3);


                    //Gizmos.DrawLine(points[i].Value, points[(i + 1) % points.Length].Value);
                }
            })
            .WithoutBurst()
            .Run();*/
        }
    }
}
