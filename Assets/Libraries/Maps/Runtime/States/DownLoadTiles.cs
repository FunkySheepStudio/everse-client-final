using FunkySheep.Maps.Types;
using FunkySheep.Terrain;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Maps
{
    [CreateAssetMenu(menuName = "FunkySheep/Maps/States/Download Tiles")]
    public class DownloadTiles : FunkySheep.States.State
    {
        public FunkySheep.Types.String url;
        public FunkySheep.Maps.Types.MapPositionRounded mapPosition;
        public FunkySheep.Maps.Types.MapPositionRounded initialMapPosition;
        public FunkySheep.Maps.Types.ZoomLevel zoomLevel;
        public FunkySheep.Maps.Types.TileSize tileSize;
        public Types.DownloadedPositions downloadedPositions;
        public GameObject tilePrefab;

        public override void Update()
        {
            base.Update();
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
            if (downloadedPositions.positions.Contains(tileMapPosition))
                return;

            downloadedPositions.positions.Add(tileMapPosition);
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };

            string[] values = new string[3] {
                zoomLevel.Value.ToString(),
                tileMapPosition.x.ToString(),
                tileMapPosition.y.ToString()
            };

            string interpolatedUrl = url.Interpolate(values, variables);

            manager.StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(interpolatedUrl, (fileID, texture) =>
            {
                AddTile(tileMapPosition, texture);
            }));
        }

        void AddTile(int2 tileMapPosition, Texture2D texture)
        {
            GameObject tileGo = GameObject.Instantiate(tilePrefab, manager.transform);
            tileGo.GetComponent<Tile>().Init(tileMapPosition, texture);
        }
    }
}
