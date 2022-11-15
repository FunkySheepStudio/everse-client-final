using Unity.Entities;

namespace FunkySheep.Buildings.Systems
{
    [UpdateAfter(typeof(WallsSystemGroup))]
    public class RoofSystemGroup : ComponentSystemGroup
    {
    }
}

