using FunkySheep.Buildings.Systems;
using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Buildings.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Buildings/States/Debug/DebugPointsCalculationSystem")]
    public class DebugPointsCalculationSystem : FunkySheep.States.State
    {
        CalculatePointsCoordinates system;

        public override void Start()
        {
            system = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CalculatePointsCoordinates>();
        }

        public override void Update()
        {
        }

        public override void OnDrawGizmos()
        {
            if (system != null)
            {
                system.OnDrawGizmos();
            }
        }

        public override void Stop()
        {
        }
    }
}
