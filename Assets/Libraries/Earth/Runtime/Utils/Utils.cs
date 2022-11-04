using Unity.Mathematics;
using Unity.Burst;
using System;
using UnityEngine;

namespace FunkySheep.Earth
{
    public static class Utils
    {
        /// <summary>
        /// Calculate the GPS boundaries of a tile depending on zoom size and the position on the map
        /// </summary>
        /// <returns>A Double[4] containing [StartLatitude, StartLongitude, EndLatitude, EndLongitude]</returns>
        [BurstCompile]
        public static double4 CaclulateGpsBoundaries(int zoom, int2 mapPosition)
        {
            double startlatitude = Utils.tileZ2lat(zoom, mapPosition.y + 1);
            double startlongitude = Utils.tileX2long(zoom, mapPosition.x);
            double endLatitude = Utils.tileZ2lat(zoom, mapPosition.y);
            double endLongitude = Utils.tileX2long(zoom, mapPosition.x + 1);

            double4 boundaries = new double4
            {
                w = startlatitude,
                x = startlongitude,
                y = endLatitude,
                z = endLongitude
            };


            return boundaries;
        }

        /// <summary>
        /// Get the Longitude of the tile relative to X position
        /// </summary>
        /// <returns></returns>
        [BurstCompile]
        public static double tileX2long(int zoom, float xPosition)
        {
            return xPosition / (double)(1 << zoom) * 360.0 - 180;
        }

        /// <summary>
        ///  Get the latitude of the tile relative to Y position
        /// </summary>
        /// <returns></returns>
        [BurstCompile]
        public static double tileZ2lat(int zoom, float zposition)
        {
            double n = math.PI - 2.0 * math.PI * zposition / (double)(1 << zoom);
            return 180.0 / math.PI * math.atan(0.5 * (math.exp(n) - math.exp(-n)));
        }

        /// <summary>
        /// Get the map tile position depending on zoom level and GPS postions
        /// https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Lon..2Flat._to_tile_numbers
        /// </summary>
        /// <returns></returns>
        public static Vector2 GpsToMapReal(int zoom, double latitude, double longitude)
        {
            Vector2 p = new Vector2();
            p.x = (float)((longitude + 180.0) / 360.0 * (1 << zoom));
            p.y = (float)((1.0 - Math.Log(Math.Tan(latitude * Math.PI / 180.0) +
              1.0 / Math.Cos(latitude * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            return p;
        }

        /// <summary>
        /// Get the map tile position depending on zoom level and GPS postions
        /// https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Lon..2Flat._to_tile_numbers
        /// </summary>
        /// <returns></returns>
        public static Vector2 GpsToMapReal(int zoom, double2 gps)
        {
            return GpsToMapReal(zoom, gps.x, gps.y);
        }

        /// <summary>
        /// Get the map tile position depending on zoom level and GPS postions
        /// https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Lon..2Flat._to_tile_numbers
        /// </summary>
        /// <returns></returns>
        public static Vector2 GpsToMapRealFloat2(int zoom, double2 gps)
        {
            Vector2 mapCoordinates = GpsToMapReal(zoom, gps.x, gps.y);
            return new float2(mapCoordinates.x, mapCoordinates.y);
        }
    }
}
