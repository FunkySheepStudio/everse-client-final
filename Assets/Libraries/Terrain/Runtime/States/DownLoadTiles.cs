using FunkySheep.Terrain.Types;
using System.Collections.Generic;
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
        public Maps.Events.TileEvent onDiffuseMapDownloaded;
        Queue<int2> pendingTiles = new Queue<int2>();

        public override void Start()
        {
            currentTiles.tiles.Clear();
        }

        public override void Update()
        {
            if (pendingTiles.Count == 0)
            {
                pendingTiles.Enqueue(mapPosition.Value);
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 0, y = 1 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 1, y = 0 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 1, y = 1 });

                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 0, y = -1 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -1, y = 0 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -1, y = -1 });

                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -1, y = 1 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 1, y = -1 });


                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 1, y = 2 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 2, y = 1 });

                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -1, y = 2 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 2, y = -1 });

                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 1, y = -2 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -2, y = 1 });

                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -1, y = -2 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -2, y = -1 });


                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 0, y = 2 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 2, y = 0 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 2, y = 2 });

                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 0, y = -2 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -2, y = 0 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -2, y = -2 });

                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = -2, y = 2 });
                pendingTiles.Enqueue(mapPosition.Value + new int2 { x = 2, y = -2 });
            } else
            {
                Download(pendingTiles.Dequeue());
            }
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

            GameObject tileGo = GameObject.Instantiate(tilePrefab, manager.transform);
            Tile tileComponent = tileGo.GetComponent<Tile>();
            tileComponent.Init(tile);

            string interpolatedHeightUrl = heightsUrl.Interpolate(values, variables);
            manager.StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(interpolatedHeightUrl, (fileID, texture) =>
            {
                Maps.Types.Tile mapTile = new Maps.Types.Tile(tileMapPosition, texture);
                tileComponent.ProcessHeights(mapTile);
            }));

            string interpolatedDiffuseUrl = diffuseUrl.Interpolate(values, variables);
            manager.StartCoroutine(FunkySheep.Network.Downloader.DownloadTexture(interpolatedDiffuseUrl, (fileID, texture) =>
            {
                Maps.Types.Tile mapTile = new Maps.Types.Tile(tileMapPosition, texture);
                tileComponent.ProcessDiffuse(mapTile);
                onDiffuseMapDownloaded.Raise(mapTile);
            }));
        }

        public override void Stop()
        {
        }

        public override void OnDrawGizmos()
        {
        }
    }
}
