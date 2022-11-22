using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Terrain
{
    public class Utils : MonoBehaviour
    {
        [BurstCompile]
        public static float? GetHeight(float3 position)
        {
            foreach (UnityEngine.Terrain terrain in UnityEngine.Terrain.activeTerrains)
            {
                UnityEngine.Bounds bounds = terrain.terrainData.bounds;
                Vector2 terrainMin = new Vector2(
                  bounds.min.x + terrain.transform.position.x,
                  bounds.min.z + terrain.transform.position.z
                );

                Vector2 terrainMax = new Vector2(
                  bounds.max.x + terrain.transform.position.x,
                  bounds.max.z + terrain.transform.position.z
                );

                if (position.x >= terrainMin.x && position.z >= terrainMin.y && position.x <= terrainMax.x && position.z <= terrainMax.y)
                {
                    if (terrain.GetComponent<Tile>().heightUpdated == true)
                    {
                        return terrain.terrainData.GetInterpolatedHeight(
                          (position.x - terrainMin.x) / (terrainMax.x - terrainMin.x),
                          (position.z - terrainMin.y) / (terrainMax.y - terrainMin.y)
                        );
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        [BurstCompile]
        public static float? GetHeight(float2 position)
        {
            return GetHeight(new float3
            {
                x = position.x,
                y = 0,
                z = position.y
            });
        }
    }
}
