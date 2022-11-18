using System;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using FunkySheep.Maps.Types;
using FunkySheep.Terrain.Types;
using FunkySheep.LevelOfDetail.Components;

namespace FunkySheep.Terrain
{
    [AddComponentMenu("FunkySheep/Earth/Terrain/Tile")]
    [RequireComponent(typeof(UnityEngine.Terrain))]
    public class Tile : MonoBehaviour
    {
        public MapPositionRounded initialMapPosition;
        public int2 mapPosition;
        public TileSize tileSize;
        public bool heightUpdated = false;
        public Material material;
        public FunkySheep.Events.Types.Int2Event onCreatedTileEvent;
        UnityEngine.Terrain terrain;

        private void Awake()
        {
            terrain = GetComponent<UnityEngine.Terrain>();
            //terrain.enabled = false;
            terrain.allowAutoConnect = true;
            terrain.materialTemplate = Instantiate<Material>(material);

            terrain.terrainData = new TerrainData();
        }

        public void Init(TileData tile)
        {
            mapPosition = tile.position;
            name = tile.position.ToString();
            Vector3 position = new Vector3(
                (tile.position.x - initialMapPosition.Value.x) * tileSize.Value,
            0,
                (initialMapPosition.Value.y - tile.position.y) * tileSize.Value // Map axis are reverted
            );
            transform.localPosition = position;
        }

        public void ProcessHeights(Maps.Types.Tile mapTile)
        {

            NativeArray<Byte> bytes = mapTile.texture.GetRawTextureData<Byte>();
            NativeArray<float> heights = new NativeArray<float>(bytes.Length / 4, Allocator.Temp);

            terrain.terrainData.heightmapResolution = (int)math.sqrt(heights.Length);
            terrain.terrainData.size = new Vector3(
                tileSize.Value,
                8900,
                tileSize.Value
            );

            var setHeightsFromTextureJob = new SetHeightsFromTextureJob
            {
                bytes = bytes,
                heights = heights
            };
            setHeightsFromTextureJob.Schedule(heights.Length, 256).Complete();

            StartCoroutine("ConvertArrayTo2DArray" ,heights);
            
        }

        public void ProcessDiffuse(Maps.Types.Tile mapTile)
        {
            mapTile.texture.wrapMode = TextureWrapMode.Clamp;
            mapTile.texture.filterMode = FilterMode.Point;
            terrain.materialTemplate.SetTexture("_MainTex", mapTile.texture);
        }

        [BurstCompile]
        struct SetHeightsFromTextureJob : IJobParallelFor
        {
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            public NativeArray<Byte> bytes;
            [NativeDisableContainerSafetyRestriction]
            [NativeDisableParallelForRestriction]
            public NativeArray<float> heights;

            public void Execute(int i)
            {
                heights[i] = (math.floor(bytes[(i * 4) + 1] * 256.0f) + math.floor(bytes[(i * 4) + 2]) + bytes[(i * 4) + 3] / 256) - 32768.0f;
                heights[i] /= 8900;
            }
        }

        [BurstCompile]
        void ConvertArrayTo2DArray(NativeArray<float> heights)
        {
            float[] flatArray = heights.ToArray();
            int borderCount = (int)math.sqrt(flatArray.Length);
            float[,] array2D = new float[borderCount + 1, borderCount + 1];

            for (int i = 0; i < flatArray.Length; i++)
            {
                // Reverse X and Y since the downloaded image is readed from the top left
                int x = (int)math.floor(i / borderCount);
                int y = i % borderCount;

                array2D[x, y] = flatArray[i];

                // Add the next row since the terrain tile is 1 row bigger
                array2D[x + 1, y] = flatArray[i];
                array2D[x, y + 1] = flatArray[i];
                array2D[x + 1, y + 1] = flatArray[i];
            }

            heights.Dispose();

            terrain.terrainData.SetHeightsDelayLOD(0, 0, array2D);
            terrain.terrainData.SyncHeightmap();

            terrain.enabled = true;
            heightUpdated = true;
            gameObject.AddComponent<Connector>();
        }
    }
}
