using Unity.Entities;
using UnityEngine;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(OSMSystemGroup))]
    public class WallsSystemGroup : ComponentSystemGroup
    {
    }
}

