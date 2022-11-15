using UnityEngine;
using Unity.Entities;
using FunkySheep.LevelOfDetail.Components.Tags;

namespace Game.Player
{
    [AddComponentMenu("Game/Player/Authoring")]
    public class Authoring : MonoBehaviour
    {
        public class PlayerBaker : Baker<Authoring>
        {
            public override void Bake(Authoring authoring)
            {
                AddComponent<CurrentPosition>();
            }
        }
    }
}
