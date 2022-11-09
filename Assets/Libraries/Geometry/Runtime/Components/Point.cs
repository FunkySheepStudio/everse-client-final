using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Geometry.Components
{
    public struct Points : IBufferElementData
    {
        public float3 Value;

        public float2 ToXY()
        {
            return new float2
            {
                x = Value.x,
                y = Value.z
            };
        }
    }
}
