using FunkySheep.Earth.Types;
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Buildings.Types
{
    [Serializable]
    public class TileData
    {
        public int2 position;
        public JsonOsmWays waysRoot;
        public JsonOsmRelations relationsRoot;

        public TileData(int2 position)
        {
            this.position = position;
        }

        public void SpawnWaysEntities()
        {
            EntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer buffer = ecbSystem.CreateCommandBuffer();

            for (int i = 0; i < waysRoot.elements.Length; i++)
            {
                Entity building = buffer.CreateEntity();
                buffer.SetName(building, new Unity.Collections.FixedString64Bytes("Building - " + waysRoot.elements[i].id.ToString()));
                buffer.AddComponent<FunkySheep.Buildings.Components.Building>(building);
                buffer.AddComponent<FunkySheep.Buildings.Components.Tags.Walls>(building);
                buffer.SetComponentEnabled<FunkySheep.Buildings.Components.Tags.Walls>(building, false);

                DynamicBuffer<Earth.Components.GPSCoordinates> gPSCoordinates = buffer.AddBuffer<Earth.Components.GPSCoordinates>(building);

                for (int j = 0; j < waysRoot.elements[i].geometry.Length; j++)
                {
                    if (!waysRoot.elements[i].geometry[(j + 1) % waysRoot.elements[i].geometry.Length].Equals(waysRoot.elements[i].geometry[j]))
                    {
                        FunkySheep.Earth.Components.GPSCoordinate gpsCoordinate = new FunkySheep.Earth.Components.GPSCoordinate
                        {
                            Value = new double2
                            {
                                x = waysRoot.elements[i].geometry[j].lat,
                                y = waysRoot.elements[i].geometry[j].lon
                            }
                        };

                        gPSCoordinates.Add(new Earth.Components.GPSCoordinates
                        {
                            Value = gpsCoordinate
                        });
                    }
                }
            }
        }

        public void SpawnRelationsEntities()
        {
        }
    }
}
