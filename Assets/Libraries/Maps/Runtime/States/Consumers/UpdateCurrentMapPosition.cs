using FunkySheep.States;
using UnityEngine;
using Unity.Mathematics;
using FunkySheep.Maps.Types;

namespace FunkySheep.Maps
{
    [CreateAssetMenu(menuName = "FunkySheep/Maps/States/Consumers/Update Map Position From Transform")]
    public class UpdateMapPositionFromTransform : FunkySheep.States.State
    {
        public FunkySheep.Maps.Types.MapPositionRounded mapPosition;
        public FunkySheep.Maps.Types.MapPositionRounded inititalMapPosition;
        public FunkySheep.Maps.Types.TileSize tileSize;
        public FunkySheep.States.State nextState;

        public override void Start()
        {
            UpdatePosition();
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
            UpdatePosition();
        }

        void UpdatePosition()
        {
            Transform transform = manager.GetComponent<Transform>();
            mapPosition.Value = new int2
            {
                x = inititalMapPosition.Value.x + (int)math.floor(transform.position.x / tileSize.Value),
                y = inititalMapPosition.Value.y - (int)math.floor(transform.position.z / tileSize.Value),
            };
        }
    }
}
