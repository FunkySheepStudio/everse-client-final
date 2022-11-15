using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Components
{
    public struct Building : IComponentData
    {
        public double2 center;
        public float minHeight;
        public float maxHeight;
        public float perimeter;
    }
}
