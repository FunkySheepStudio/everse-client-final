using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FunkySheep.Terrain
{
    public class Connector : MonoBehaviour
    {
        public UnityEngine.Terrain terrain;
        public UnityEngine.TerrainData terrainData;

        public bool topConnected = false;
        public bool leftConnected = false;
        public bool cornerConnected = false;

        private void Start()
        {
            terrain = GetComponent<UnityEngine.Terrain>();
            terrainData = terrain.terrainData;
        }

        private void Update()
        {
            if (
              !topConnected &&
              terrain.topNeighbor != null &&
              terrain.topNeighbor.GetComponent<Tile>().heightUpdated
            )
            {
                StartCoroutine("ConnectTop",terrain.topNeighbor);
            }

            if (!leftConnected && terrain.leftNeighbor != null && terrain.leftNeighbor.GetComponent<Tile>().heightUpdated)
            {
                StartCoroutine("ConnectLeft", terrain.leftNeighbor);
            }

            if (!cornerConnected &&
                terrain.leftNeighbor != null && terrain.leftNeighbor.GetComponent<Tile>().heightUpdated &&
                terrain.topNeighbor != null && terrain.topNeighbor.GetComponent<Tile>().heightUpdated &&
                terrain.leftNeighbor.topNeighbor != null && terrain.leftNeighbor.topNeighbor.GetComponent<Tile>().heightUpdated
              )
            {
                StartCoroutine("ConnectCorners");
            }

            if (cornerConnected && leftConnected && topConnected)
            {
                StartCoroutine("CreateEcsCollider");
                enabled = false;
            }
        }

        [BurstCompile]
        void ConnectTop(UnityEngine.Terrain top)
        {
            float[,] heights = terrainData.GetHeights(0, terrainData.heightmapResolution - 1, terrainData.heightmapResolution, 1);
            float[,] heightsTop = top.terrainData.GetHeights(0, 0, terrainData.heightmapResolution, 1);
            float[,] heightsNew = heights;

            for (int y = 0; y < heightsNew.Length; y++)
            {
                heightsNew[0, y] = (heights[0, y] + heightsTop[0, y]) / 2;
            }

            terrainData.SetHeightsDelayLOD(0, top.terrainData.heightmapResolution - 1, heightsNew);
            terrainData.SyncHeightmap();
            top.terrainData.SetHeightsDelayLOD(0, 0, heightsNew);
            top.terrainData.SyncHeightmap();

            topConnected = true;
        }

        [BurstCompile]
        void ConnectLeft(UnityEngine.Terrain left)
        {
            float[,] heights = terrainData.GetHeights(0, 0, 1, terrainData.heightmapResolution);
            float[,] heightsLeft = left.terrainData.GetHeights(terrainData.heightmapResolution - 1, 0, 1, terrainData.heightmapResolution);
            float[,] heightsNew = heights;

            for (int x = 0; x < heightsNew.Length; x++)
            {
                heightsNew[x, 0] = (heights[x, 0] + heightsLeft[x, 0]) / 2;
            }

            terrainData.SetHeightsDelayLOD(0, 0, heightsNew);
            terrainData.SyncHeightmap();
            left.terrainData.SetHeightsDelayLOD(left.terrainData.heightmapResolution - 1, 0, heightsNew);
            left.terrainData.SyncHeightmap();

            leftConnected = true;
        }

        [BurstCompile]
        void ConnectCorners()
        {
            float[,] heights = terrainData.GetHeights(0, terrainData.heightmapResolution - 1, 1, 1);
            float[,] heightsLeft = terrain.leftNeighbor.terrainData.GetHeights(terrainData.heightmapResolution - 1, terrainData.heightmapResolution - 1, 1, 1);
            float[,] heightsTop = terrain.topNeighbor.terrainData.GetHeights(0, 0, 1, 1);
            float[,] heightsLeftTop = terrain.topNeighbor.leftNeighbor.terrainData.GetHeights(terrainData.heightmapResolution - 1, 0, 1, 1);

            float[,] heightsNew = heights;

            heightsNew[0, 0] = (heights[0, 0] + heightsLeft[0, 0] + heightsTop[0, 0] + heightsLeftTop[0, 0]) / 4;

            terrainData.SetHeightsDelayLOD(0, terrainData.heightmapResolution - 1, heights);
            terrainData.SyncHeightmap();

            terrain.leftNeighbor.terrainData.SetHeightsDelayLOD(terrainData.heightmapResolution - 1, terrainData.heightmapResolution - 1, heights);
            terrain.leftNeighbor.terrainData.SyncHeightmap();


            terrain.topNeighbor.terrainData.SetHeightsDelayLOD(0, 0, heights);
            terrain.topNeighbor.terrainData.SyncHeightmap();


            terrain.topNeighbor.leftNeighbor.terrainData.SetHeightsDelayLOD(terrainData.heightmapResolution - 1, 0, heights);
            terrain.topNeighbor.leftNeighbor.terrainData.SyncHeightmap();

            cornerConnected = true;
            Tile tile = GetComponent<Tile>();
            tile.onCreatedTileEvent.Raise(tile.mapPosition);
        }

        [BurstCompile]
        public void CreateEcsCollider()
        {
            var physicsCollider = new PhysicsCollider();
            var size = new int2(terrainData.heightmapResolution, terrainData.heightmapResolution);
            var scale = terrainData.heightmapScale;

            var colliderHeights = new NativeArray<float>(terrainData.heightmapResolution * terrainData.heightmapResolution,
                Allocator.TempJob);

            var terrainHeights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                terrainData.heightmapResolution);


            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                {
                    var height = terrainHeights[x, y];
                    colliderHeights[y + x * size.x] = height;
                }

            physicsCollider.Value = Unity.Physics.TerrainCollider.Create(colliderHeights, size, scale, Unity.Physics.TerrainCollider.CollisionMethod.Triangles);

            colliderHeights.Dispose();

            EntityCommandBufferSystem ecbSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer buffer = ecbSystem.CreateCommandBuffer();

            Entity entity = buffer.CreateEntity();

            LocalToWorld localToWorld = new LocalToWorld
            {
                Value = transform.localToWorldMatrix
            };

            buffer.AddComponent<LocalToWorld>(entity, localToWorld);
            buffer.SetName(entity, new FixedString64Bytes(terrain.name + " Collider"));
            buffer.AddComponent<PhysicsCollider>(entity, physicsCollider);
            buffer.AddSharedComponent<PhysicsWorldIndex>(entity, new PhysicsWorldIndex { Value = 0});
        }
    }
}