using FunkySheep.Earth.Types;
using FunkySheep.Maps.Types;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Buildings.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Buildings/States/Downloading")]
    public class Downloading : FunkySheep.States.State
    {
        public FunkySheep.Types.String url;
        public FunkySheep.Maps.Types.MapPositionRounded mapPosition;
        public FunkySheep.Maps.Types.ZoomLevel zoomLevel;
        public FunkySheep.Maps.Types.TileSize tileSize;
        public Types.TileList currentTiles;
        public GameObject prefab;

        public override void Start()
        {
            currentTiles.tiles.Clear();
        }

        public override void Update()
        {
            Download(mapPosition.Value);
            Download(mapPosition.Value + new int2 { x = 0, y = 1 });
            Download(mapPosition.Value + new int2 { x = 1, y = 0 });
            Download(mapPosition.Value + new int2 { x = 1, y = 1 });

            Download(mapPosition.Value + new int2 { x = 0, y = -1 });
            Download(mapPosition.Value + new int2 { x = -1, y = 0 });
            Download(mapPosition.Value + new int2 { x = -1, y = -1 });

            Download(mapPosition.Value + new int2 { x = -1, y = 1 });
            Download(mapPosition.Value + new int2 { x = 1, y = -1 });
        }

        public void Download(int2 tileMapPosition)
        {
            Types.TileData tile = currentTiles.tiles.Find(tile => tile.position.Equals(tileMapPosition));
            if (tile != null)
                return;

            tile = new Types.TileData(tileMapPosition);
            currentTiles.tiles.Add(tile);

            string tileUrl = InterpolatedUrl(tileMapPosition);

            manager.StartCoroutine(FunkySheep.Network.Downloader.Download(tileUrl, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                tile.data = JsonUtility.FromJson<Types.JsonOsmRoot>(fileStr);

                for (int i = 0; i < tile.data.elements.Length; i++)
                {
                    for (int j = 0; j < tile.data.elements[i].geometry.Length; j++)
                    {
                        double2 gpsCoordinates = new double2
                        {
                            x = tile.data.elements[i].geometry[j].lat,
                            y = tile.data.elements[i].geometry[j].lon
                        };

                        float3 position = GetWorldPosition(gpsCoordinates);
                        GameObject pointGo = GameObject.Instantiate(prefab, manager.transform);
                        pointGo.transform.position = position;
                    }
                }
            }));
        }

        /// <summary>
        /// Interpolate the url inserting the boundaries and the types of OSM data to download
        /// </summary>
        /// <param boundaries="boundaries">The gps boundaries to download in</param>
        /// <returns>The interpolated Url</returns>
        public string InterpolatedUrl(int2 tileMapPosition)
        {
            double4 gpsBoundaries = FunkySheep.Earth.Utils.CaclulateGpsBoundaries(
                zoomLevel.Value,
                tileMapPosition
            );

            string[] parameters = new string[5];
            string[] parametersNames = new string[5];

            parameters[0] = gpsBoundaries.w.ToString().Replace(',', '.');
            parametersNames[0] = "startLatitude";

            parameters[1] = gpsBoundaries.x.ToString().Replace(',', '.');
            parametersNames[1] = "startLongitude";

            parameters[2] = gpsBoundaries.y.ToString().Replace(',', '.');
            parametersNames[2] = "endLatitude";

            parameters[3] = gpsBoundaries.z.ToString().Replace(',', '.');
            parametersNames[3] = "endLongitude";

            return url.Interpolate(parameters, parametersNames);
        }

        /// <summary>
        /// Return the world position given GPS Coordinates
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public float3 GetWorldPosition(double2 gpsCoordinates)
        {
            float2 calculatedMapPosition = Earth.Utils.GpsToMapRealFloat2(zoomLevel.Value, gpsCoordinates);
            float2 mapOffset = calculatedMapPosition - mapPosition.Value;

            return new float3(
                mapOffset.x * tileSize.Value,
            0,
                 tileSize.Value - mapOffset.y * tileSize.Value // Since map coordinates are reversed on Y axis
            );
        }

        public override void Stop()
        {
        }
    }
}
