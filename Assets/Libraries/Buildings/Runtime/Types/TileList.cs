using System.Collections.Generic;
using UnityEngine;

namespace FunkySheep.Buildings.Types
{
    [CreateAssetMenu(menuName = "FunkySheep/Buildings/Types/Tiles list")]
    public class TileList : ScriptableObject
    {
        public List<TileData> tiles;

        private void OnEnable()
        {
            tiles = new List<TileData>();
        }
    }
}
