using FunkySheep.Terrain.Types;
using Unity.Mathematics;
using UnityEngine;

namespace FunkySheep.Terrain
{
    [CreateAssetMenu(menuName = "FunkySheep/Terrain/States/Download Tiles")]
    public class DownloadTiles : FunkySheep.States.State
    {
        public FunkySheep.Types.String heightsUrl;
        public FunkySheep.Types.String diffuseUrl;
        public FunkySheep.Maps.Types.MapPositionRounded mapPosition;
        public FunkySheep.Maps.Types.MapPositionRounded initialMapPosition;
        public FunkySheep.Maps.Types.ZoomLevel zoomLevel;
        public FunkySheep.Maps.Types.TileSize tileSize;
        public Types.TileList currentTiles;
        public GameObject tilePrefab;

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
            TileData tile = currentTiles.tiles.Find(tile => tile.position.Equals(tileMapPosition));
            if (tile != null)
                return;

            tile = new Types.TileData(tileMapPosition);
            currentTiles.tiles.Add(tile);
            string[] variables = new string[3] { "zoom", "position.x", "position.y" };

            string[] values = new string[3] {
                zoomLevel.Value.ToString(),
                tileMapPosition.x.ToString(),
                tileMapPosition.y.ToString()
            };

            string interpolatedHeightUrl = heightsUrl.Interpolate(values, variables);
            manager.StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(interpolatedHeightUrl, (fileID, texture) =>
            {
                tile.heightTexture = texture;
                AddTile(tile);
            }));

            string interpolatedDiffuseUrl = diffuseUrl.Interpolate(values, variables);
            manager.StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(interpolatedDiffuseUrl, (fileID, texture) =>
            {
                tile.diffuseTexture = texture;
                AddTile(tile);
            }));
        }

        void AddTile(TileData tile)
        {
            if (tile.heightTexture == null || tile.diffuseTexture == null)
                return;

            GameObject tileGo = GameObject.Instantiate(tilePrefab, manager.transform);
            tileGo.GetComponent<Tile>().Init(tile);
        }

        public override void Stop()
        {
        }

        public override void OnDrawGizmos()
        {
        }
    }
}
