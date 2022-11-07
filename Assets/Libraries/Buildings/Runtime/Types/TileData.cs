using System;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Buildings.Types
{
    [Serializable]
    public class TileData
    {
        public int2 position;
        public JsonOsmWays waysRoot;
        public JsonOsmRelations relationsRoot;

        public TileData(int2 position)
        {
            this.position = position;
        }

        public void ConvertWays(float tileSize, int zoom, int2 initialMapPosition, GameObject prefab)
        {
            for (int i = 0; i < waysRoot.elements.Length; i++)
            {
                if (waysRoot.elements[i].geometry != null)
                {
                    for (int j = 0; j < waysRoot.elements[i].geometry.Length; j++)
                    {
                        double2 gpsCoordinates = new double2
                        {
                            x = waysRoot.elements[i].geometry[j].lat,
                            y = waysRoot.elements[i].geometry[j].lon
                        };

                        double2 nextgpsCoordinates = new double2
                        {
                            x = waysRoot.elements[i].geometry[(j + 1) % waysRoot.elements[i].geometry.Length].lat,
                            y = waysRoot.elements[i].geometry[(j + 1) % waysRoot.elements[i].geometry.Length].lon
                        };

                        float3 position = Earth.Utils.GpsToMapRealOffseted(gpsCoordinates.x, gpsCoordinates.y, zoom, initialMapPosition) * tileSize;
                        float3 nextposition = Earth.Utils.GpsToMapRealOffseted(nextgpsCoordinates.x, nextgpsCoordinates.y, zoom, initialMapPosition) * tileSize;

                        Debug.DrawLine(position, nextposition, Color.red, 10000);
                        GameObject.Instantiate(prefab, position, quaternion.identity);

                    }
                }
            }
        }
    }
}
