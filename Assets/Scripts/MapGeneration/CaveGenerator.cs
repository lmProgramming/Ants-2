using System.Collections.Generic;
using UnityEngine;

public sealed class CaveGenerator : MonoBehaviour
{
    public static CaveGenerator Instance;

    public bool spawnCave = true;

    public float wallNoiseThreshold = 0.8f;

    public float noiseScale;
    public int octaves;
    public float persistence;
    public float lacunarity;

    public int smallFragmentsWallMaxTolerance = 2;

    public int seed;

    private void Awake()
    {
        Instance = this;

        SetRandomSeed();
    }

    public void SetRandomSeed()
    {
        seed = Random.Range(0, 1000000000);
    }

    public void SpawnCaves()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        int mapWidth = MapGenerator.mapWidth;
        int mapHeight = MapGenerator.mapHeight;

        int borderWallLength = MapGenerator.Instance.borderWallLength;

        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, Vector2.zero);
        GridTile[,] tileMap = MapGenerator.Instance.tileMap;

        for (int x = borderWallLength; x < mapWidth - borderWallLength; x++)
        {
            for (int y = borderWallLength; y < mapHeight - borderWallLength; y++)
            {
                if (x < borderWallLength * 2)
                {
                    noiseMap[x, y] *= Mathf.Pow(x / (float)(borderWallLength * 2), 2);
                }
                if (x > mapWidth - (borderWallLength * 2))
                {
                    noiseMap[x, y] *= Mathf.Pow((mapWidth - x) / (float)(borderWallLength * 2), 2);
                }
                if (y < borderWallLength * 2)
                {
                    noiseMap[x, y] *= Mathf.Pow(y / (float)(borderWallLength * 2), 2);
                }
                if (y > mapHeight - borderWallLength * 2)
                {
                    noiseMap[x, y] *= Mathf.Pow((mapHeight - y) / (float)(borderWallLength * 2), 2);
                }

                if (noiseMap[x, y] <= wallNoiseThreshold)
                {
                    tileMap[x, y].ChangeToWall();
                }
                else
                {
                    tileMap[x, y].ChangeToPheromone();
                }
            }
        }

        List<GridTile> tilesToDelete = new List<GridTile>();

        for (int x = borderWallLength; x < mapWidth - borderWallLength; x++)
        {
            for (int y = borderWallLength; y < mapHeight - borderWallLength; y++)
            {
                if (tileMap[x, y].GetTileState() == TileState.Wall)
                {
                    if (WallsAround(tileMap[x, y]) <= smallFragmentsWallMaxTolerance)
                    {
                        tilesToDelete.Add(tileMap[x, y]);
                    }
                }
            }
        }

        for (int i = 0; i < tilesToDelete.Count; i++)
        {
            tilesToDelete[i].ChangeToPheromone();
        }
    }

    private int WallsAround(GridTile tile)
    {
        GridTile[] neighbours = tile.NineTilesAround();

        int wallsAround = 0;
        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i].GetTileState() == TileState.Wall)
            {
                wallsAround++;
            }
        }

        return wallsAround;
    }

    private void OnValidate()
    {
        SpawnCaves();
    }
}