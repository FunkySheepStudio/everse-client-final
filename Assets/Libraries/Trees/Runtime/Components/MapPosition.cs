using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Trees.Components
{
    public struct MapPosition : IComponentData
    {
        public int2 Value;
    }
}
