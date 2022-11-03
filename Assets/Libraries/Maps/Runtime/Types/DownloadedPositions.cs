using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace FunkySheep.Maps.Types
{
    [CreateAssetMenu(menuName = "FunkySheep/Maps/Types/Downloaded Positions")]
    public class DownloadedPositions : ScriptableObject
    {
        public List<int2> positions;

        private void OnEnable()
        {
            positions = new List<int2>();
        }
    }
}
