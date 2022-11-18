using UnityEngine;
using Unity.Mathematics;

namespace FunkySheep.Maps.Types
{
    public class Tile
    {
        public int2 mapPosition;
        public Texture2D texture;

        public Tile(int2 mapPosition, Texture2D texture)
        {
            this.mapPosition = mapPosition;
            this.texture = texture;
        }
    }
}
