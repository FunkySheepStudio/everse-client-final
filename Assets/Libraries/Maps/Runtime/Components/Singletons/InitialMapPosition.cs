using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Maps.Components
{
    public struct InitialMapPosition : IComponentData
    {
        public int2 Value;
    }
}
