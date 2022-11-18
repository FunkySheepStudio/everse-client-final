using FunkySheep.Events;
using FunkySheep.Maps.Types;
using UnityEngine;

namespace FunkySheep.Maps.Events
{
    [CreateAssetMenu(menuName = "FunkySheep/Maps/Events/On Downloaded")]
    public class TileEvent : Event<Tile>
    {
    }
}
