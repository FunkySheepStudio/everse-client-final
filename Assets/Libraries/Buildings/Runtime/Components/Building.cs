using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Components
{
    public struct Building : IComponentData, IEnableableComponent
    {
        public double2 center;
        public float minHeight;
        public float maxHeight;
    }
}
