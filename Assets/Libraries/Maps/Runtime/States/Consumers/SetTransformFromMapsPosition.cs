using FunkySheep.States;
using UnityEngine;
using Unity.Mathematics;

namespace FunkySheep.Maps
{
    [CreateAssetMenu(menuName = "FunkySheep/Maps/States/Consumers/Set Transform From Map Position")]
    public class SetTransformFromMapsPosition : FunkySheep.States.State
    {
        public FunkySheep.Maps.Types.MapPositionRounded initalMapPositionRounded;
        public FunkySheep.Maps.Types.MapPosition mapPosition;
        public FunkySheep.Maps.Types.TileSize tileSize;
        public FunkySheep.States.State nextState;

        public override void Start()
        {
            Transform transform = manager.GetComponent<Transform>();
            transform.position = new Vector3
            {
                x = (mapPosition.Value.x - initalMapPositionRounded.Value.x) * tileSize.Value,
                y = transform.position.y,
                z = (initalMapPositionRounded.Value.y - mapPosition.Value.y + 1) * tileSize.Value, // Tile start on top
            };

            if (nextState)
                manager.AddState(nextState);

            manager.RemoveState(this);
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
        }
    }
}
