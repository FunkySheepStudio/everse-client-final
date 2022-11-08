using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FunkySheep.Earth.Components
{
    [Serializable]
    public struct GPSCoordinates : IBufferElementData, IEnableableComponent
    {
        public GPSCoordinate Value;
    }
}
