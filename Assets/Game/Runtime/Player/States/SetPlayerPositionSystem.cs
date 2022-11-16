using UnityEngine;
using Unity.Entities;
using FunkySheep.States;

namespace Game.Player
{
    [CreateAssetMenu(menuName = "Game/Player/States/Set Player Position System")]
    public class SetPlayerPositionSystem : State
    {
        EntityManager entityManager;
        public override void OnDrawGizmos()
        {
        }

        public override void Start()
        {
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<Systems.SetInitialPlayerPosition>().playerTransform = manager.transform;
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<Systems.SetPlayerPosition>().playerTransform = manager.transform;
        }

        public override void Stop()
        {
        }

        public override void Update()
        {
        }
    }
}
