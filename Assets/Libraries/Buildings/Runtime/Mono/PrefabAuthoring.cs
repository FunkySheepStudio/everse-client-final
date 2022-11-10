using UnityEngine;
using Unity.Entities;

namespace FunkySheep.Buildings
{
    public class PrefabAuthoring : MonoBehaviour
    {
        public class BuildingAuthoring : Baker<PrefabAuthoring>
        {
            public override void Bake(PrefabAuthoring authoring)
            {
                AddComponent<FunkySheep.Buildings.Components.Tags.Prefab>();
            }
        }
    }

}
