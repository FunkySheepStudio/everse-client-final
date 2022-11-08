using System;
using Unity.Entities;

namespace FunkySheep.Maps.Components
{
    [Serializable]
    public struct ZoomLevel : IComponentData
    {
        public int Value;
    }
}
