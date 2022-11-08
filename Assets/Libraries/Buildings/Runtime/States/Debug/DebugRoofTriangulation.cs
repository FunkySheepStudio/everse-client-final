using FunkySheep.Buildings.Systems;
using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Buildings.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Buildings/States/Debug/Debug Roof Triangulation")]
    public class DebugRoofTriangulation : FunkySheep.States.State
    {
        TriangulateRoof system;

        public override void Start()
        {
            system = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<TriangulateRoof>();
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
