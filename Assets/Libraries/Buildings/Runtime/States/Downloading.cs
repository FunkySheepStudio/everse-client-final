using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Buildings.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Buildings/States/Downloading")]
    public class Downloading : FunkySheep.States.State
    {
        public FunkySheep.Types.String waysUrlTemplate;
        public FunkySheep.Types.String relationsUrlTemplate;
        public FunkySheep.Maps.Types.MapPositionRounded mapPosition;
        public FunkySheep.Maps.Types.MapPositionRounded initialMapPosition;
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

            string waysUrl = InterpolatedUrl(tileMapPosition, waysUrlTemplate);
            manager.StartCoroutine(FunkySheep.Network.Downloader.Download(waysUrl, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                tile.waysRoot = JsonUtility.FromJson<Types.JsonOsmWays>(fileStr);
                //tile.ConvertWays(tileSize.Value, zoomLevel.Value, initialMapPosition.Value, prefab);
            }));

            string relationsUrl = InterpolatedUrl(tileMapPosition, relationsUrlTemplate);
            manager.StartCoroutine(FunkySheep.Network.Downloader.Download(relationsUrl, (fileID, file) =>
            {
                string fileStr = System.Text.Encoding.Default.GetString(file);
                tile.relationsRoot = JsonUtility.FromJson<Types.JsonOsmRelations>(fileStr);
            }));
        }

        /// <summary>
        /// Interpolate the url inserting the boundaries and the types of OSM data to download
        /// </summary>
        /// <param boundaries="boundaries">The gps boundaries to download in</param>
        /// <returns>The interpolated Url</returns>
        public string InterpolatedUrl(int2 tileMapPosition, FunkySheep.Types.String url)
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

        public override void Stop()
        {
        }
    }
}
