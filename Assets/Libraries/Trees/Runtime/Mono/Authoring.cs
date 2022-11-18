using UnityEngine;
using Unity.Entities;

namespace FunkySheep.Trees
{
    [AddComponentMenu("FunkySheep/Trees/Authoring")]
    public class Authoring : MonoBehaviour
    {
        public class TreesBaker : Baker<Authoring>
        {
            public override void Bake(Authoring authoring)
            {
                AddComponent(new Components.Tags.Prefab { });
            }
        }
    }
}
