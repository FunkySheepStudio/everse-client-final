using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Images.Components
{
    public struct Pixels : IBufferElementData
    {
        public Color32 Value;
    }
}
