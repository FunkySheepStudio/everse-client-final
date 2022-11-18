using UnityEngine;
using FunkySheep.States;
using Unity.Entities;
using Unity.Collections;
using FunkySheep.Images.Components;

namespace FunkySheep.Trees.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Trees/States/Downloading")]
    public class Downloading : State
    {
        public void OnDiffuseMapDownloaded(Maps.Types.Tile mapTile)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            NativeArray<Pixels> pixels = mapTile.texture.GetRawTextureData<Pixels>();
            Entity trees = entityManager.CreateEntity();
            entityManager.AddBuffer<Pixels>(trees);
            entityManager.GetBuffer<Pixels>(trees).CopyFrom(pixels.ToArray());
            entityManager.AddComponent<Components.MapPosition>(trees);
            entityManager.SetComponentData(trees, new Components.MapPosition { Value = mapTile.mapPosition });
        }

        public override void OnDrawGizmos()
        {
        }

        public override void Start()
        {

        }

        public override void Stop()
        {
        }

        public override void Update()
        {
        }
    }
}
