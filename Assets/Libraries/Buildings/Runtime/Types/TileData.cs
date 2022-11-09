using System;
using Unity.Entities;
using Unity.Mathematics;
using FunkySheep.Geometry.Components.Tags;
using FunkySheep.Buildings.Components.Barriers;

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
                SpawnBuilding(buffer, waysRoot.elements[i]);
            }
        }

        void SpawnBuilding(EntityCommandBuffer buffer, JsonOsmWay way)
        {
            Entity building = buffer.CreateEntity();
            buffer.AddComponent<FunkySheep.Buildings.Components.Building>(building);
            buffer.AddComponent<RemoveColinearPoints>(building);
            buffer.AddComponent<SetPointsCenter>(building);
            buffer.AddComponent<SetPointsOrder>(building);
            buffer.AddComponent<SetPointsCounterClockWise>(building);
            buffer.AddComponent<SetPointsArea>(building);

            DynamicBuffer<Earth.Components.GPSCoordinates> gPSCoordinates = buffer.AddBuffer<Earth.Components.GPSCoordinates>(building);

            for (int i = 0; i < way.geometry.Length; i++)
            {
                if (!way.geometry[(i + 1) % way.geometry.Length].Equals(way.geometry[i]))
                {
                    FunkySheep.Earth.Components.GPSCoordinate gpsCoordinate = new FunkySheep.Earth.Components.GPSCoordinate
                    {
                        Value = new double2
                        {
                            x = way.geometry[i].lat,
                            y = way.geometry[i].lon
                        }
                    };

                    gPSCoordinates.Add(new Earth.Components.GPSCoordinates
                    {
                        Value = gpsCoordinate
                    });
                }
            }
        }

        public void SpawnRelationsEntities()
        {
            EntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer buffer = ecbSystem.CreateCommandBuffer();

            for (int i = 0; i < relationsRoot.elements.Length; i++)
            {
                for (int j = 0; j < relationsRoot.elements[i].members.Length; j++)
                {
                    if (relationsRoot.elements[i].members[j].role == "outer")
                        SpawnBuilding(buffer, relationsRoot.elements[i].members[j]);
                }
            }
        }
    }
}
