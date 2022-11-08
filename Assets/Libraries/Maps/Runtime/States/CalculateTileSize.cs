using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using FunkySheep.Maps.Components;

namespace FunkySheep.Maps
{
    [CreateAssetMenu(menuName = "FunkySheep/Maps/States/Consumers/Calculate Tile Size")]
    public class CalculateTileSize : FunkySheep.States.State
    {
        public FunkySheep.Earth.Types.GpsCoordinates gpsCoordinates;
        public FunkySheep.Maps.Types.ZoomLevel zoomLevel;
        public FunkySheep.Maps.Types.TileSize tileSize;
        public FunkySheep.States.State nextState;

        public override void Start()
        {
            tileSize.Value = (float)(156543.03 / math.pow(2, zoomLevel.Value) * math.cos(math.PI * 2 / 360 * gpsCoordinates.Value.y) * 256);
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity tileSizeEntity = entityManager.CreateEntity();
            entityManager.AddComponent<TileSize>(tileSizeEntity);
            entityManager.SetComponentData<TileSize>(tileSizeEntity, new TileSize
            {
                Value = tileSize.Value
            });

        }

        public override void Update()
        {
        }

        public override void Stop()
        {
        }

        public override void OnDrawGizmos()
        {
        }
    }
}
