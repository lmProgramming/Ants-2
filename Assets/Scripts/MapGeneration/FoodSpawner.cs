using System.Linq;
using UnityEngine;

public sealed class FoodSpawner : MonoBehaviour
{
    public static FoodSpawner Instance;

    public bool finiteFood = true;

    public Vector2 foodAmountBounds;
    public float AverageFood { get => foodAmountBounds.x + (foodAmountBounds.y - foodAmountBounds.x) / 2; }

    [SerializeField] private bool spawnRandomFoodAtBeginning = true;
    public float foodNoiseThreshold = 0.8f;

    public float noiseScale;
    public int octaves;
    public float persistence;
    public float lacunarity;

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

    public void SpawnStartingFood()
    {
        if (spawnRandomFoodAtBeginning && MapGenerator.Instance != null)
        {
            int mapWidth = MapGenerator.mapWidth;
            int mapHeight = MapGenerator.mapHeight;

            int borderWallLength = MapGenerator.Instance.borderWallLength;

            float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, Vector2.zero);

            float currentFoodNoiseThreshold = foodNoiseThreshold < 1 ? foodNoiseThreshold : 100;

            for (int x = borderWallLength; x < mapWidth - borderWallLength; x++)
            {
                for (int y = borderWallLength; y < mapHeight - borderWallLength; y++)
                {
                    GridTile tile = MapGenerator.Instance.tileMap[x, y];

                    if (tile.GetTileState() == TileState.Wall)
                    {
                        continue;
                    }

                    float borderWallLengthDoubled = borderWallLength * 2f;

                    if (x < borderWallLengthDoubled)
                    {
                        noiseMap[x, y] *= Mathf.Pow(x / (float)(borderWallLengthDoubled), 2);
                    }
                    if (x > mapWidth - (borderWallLengthDoubled))
                    {
                        noiseMap[x, y] *= Mathf.Pow((mapWidth - x) / (float)(borderWallLengthDoubled), 2);
                    }
                    if (y < borderWallLengthDoubled)
                    {
                        noiseMap[x, y] *= Mathf.Pow(y / (float)(borderWallLengthDoubled), 2);
                    }
                    if (y > mapHeight - borderWallLengthDoubled)
                    {
                        noiseMap[x, y] *= Mathf.Pow((mapHeight - y) / (float)(borderWallLengthDoubled), 2);
                    }

                    if (noiseMap[x, y] < currentFoodNoiseThreshold)
                    {
                        continue;
                    }

                    if (CheckIfNoWallsAround(tile))
                    {
                        float noiseMultiplier = Mathf.Sqrt((noiseMap[x, y] - currentFoodNoiseThreshold) / currentFoodNoiseThreshold);
                        float randomFoodBetweenBounds = Random.Range(foodAmountBounds.x, foodAmountBounds.y);

                        int neighbourWalls = CountWallsAround(tile);
                        float neighbourWallMultiplier = neighbourWalls > 4 ? 0 : 1 - neighbourWalls / 4f;

                        float foodValue = noiseMultiplier * randomFoodBetweenBounds * neighbourWallMultiplier;

                        if (foodValue > 0)
                        {
                            tile.ChangeToFood(foodValue);
                        }
                        else
                        {
                            tile.ChangeToPheromone();
                        }
                    }
                }
            }
        }
    }

    private int CountWallsAround(GridTile target)
    {
        GridTile[] tilesAround = MapGenerator.Instance.CircleAround(target.gridPos, 3.5f);

        return tilesAround.Count(obj => obj.GetTileState() == TileState.Wall);
    }

    private bool CheckIfNoWallsAround(GridTile target)
    {
        GridTile[] tilesAround = MapGenerator.Instance.CircleAround(target.gridPos, Random.Range(2f, 3f));

        for (int i = 0; i < tilesAround.Length; i++)
        {
            if (tilesAround[i].GetTileState() == TileState.Wall)
            {
                return false;
            }
        }

        return true;
    }

    public void SpawnFoodAt(Vector2 position)
    {
        MapGenerator.Instance.TileAt(position).ChangeToFood(Random.Range(foodAmountBounds.x, foodAmountBounds.y));
    }
}
