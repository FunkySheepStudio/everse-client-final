using FunkySheep.States;
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using FunkySheep.Maps.Components;

namespace FunkySheep.Maps
{
    [CreateAssetMenu(menuName = "FunkySheep/Maps/States/Consumers/Calculate Map Position from Gps")]
    public class CalculatePositionFromGps : FunkySheep.States.State
    {
        public FunkySheep.Earth.Types.GpsCoordinates gpsCoordinates;
        public FunkySheep.Maps.Types.ZoomLevel zoomLevel;
        public FunkySheep.Maps.Types.MapPosition mapPosition;
        public FunkySheep.Maps.Types.MapPositionRounded mapPositionRounded;
        public FunkySheep.States.State nextState;

        public override void Start()
        {
            mapPosition.Value.x = (float)((gpsCoordinates.Value.y + 180.0) / 360.0 * (1 << zoomLevel.Value));
            mapPosition.Value.y = (float)((1.0 - math.log(math.tan(gpsCoordinates.Value.x * math.PI / 180.0) + 1.0 / math.cos(gpsCoordinates.Value.x * math.PI / 180.0)) / math.PI) / 2.0 * (1 << zoomLevel.Value));

            if (mapPositionRounded)
                mapPositionRounded.Value = new int2
                {
                    x = (int)math.floor(mapPosition.Value.x),
                    y = (int)math.floor(mapPosition.Value.y),
                };

            // Setting ECS parameters
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity zoomLevelEntity = entityManager.CreateEntity();
            entityManager.AddComponent<ZoomLevel>(zoomLevelEntity);
            entityManager.SetComponentData<ZoomLevel>(zoomLevelEntity, new ZoomLevel
            {
                Value = zoomLevel.Value
            });

            Entity tileSizeEntity = entityManager.CreateEntity();
            entityManager.AddComponent<InitialMapPosition>(tileSizeEntity);
            entityManager.SetComponentData<InitialMapPosition>(tileSizeEntity, new InitialMapPosition
            {
                Value = new int2
                {
                    x = (int)math.floor(mapPosition.Value.x),
                    y = (int)math.floor(mapPosition.Value.y),
                }
            });

            if (nextState)
                manager.AddState(nextState);

            manager.RemoveState(this);
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
