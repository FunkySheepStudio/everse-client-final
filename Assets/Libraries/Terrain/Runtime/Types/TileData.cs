using System;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Terrain.Types
{
    [Serializable]
    public class TileData
    {
        public int2 position;
        public Texture2D heightTexture;
        public Texture2D diffuseTexture;

        public TileData(int2 position)
        {
            this.position = position;
        }
    }
}
