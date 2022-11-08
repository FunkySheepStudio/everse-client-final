using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Components
{
    public struct Building : IComponentData, IEnableableComponent
    {
        public float3 center;
        public float minHeight;
        public float maxHeight;
    }
}
