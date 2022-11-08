using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth.Components
{
    [Serializable]
    public struct GPSCoordinate : IComponentData
    {
        public double2 Value;
    }
}
