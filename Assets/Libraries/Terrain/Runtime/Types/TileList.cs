using System.Collections.Generic;
using UnityEngine;

namespace FunkySheep.Terrain.Types
{
    [CreateAssetMenu(menuName = "FunkySheep/Terrain/Types/Tiles list")]
    public class TileList : ScriptableObject
    {
        public List<TileData> tiles;

        private void OnEnable()
        {
            tiles = new List<TileData>();
        }
    }
}
