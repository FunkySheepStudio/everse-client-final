using System;
using Unity.Mathematics;

namespace FunkySheep.Buildings.Types
{
    [Serializable]
    public class TileData
    {
        public int2 position;
        public JsonOsmRoot data;

        public TileData(int2 position)
        {
            this.position = position;
        }
    }
}
