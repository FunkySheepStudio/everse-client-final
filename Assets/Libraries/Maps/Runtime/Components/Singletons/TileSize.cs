using System;
using Unity.Entities;

namespace FunkySheep.Maps.Components
{
    [Serializable]
    public struct TileSize : IComponentData
    {
        public float Value;
    }
}
