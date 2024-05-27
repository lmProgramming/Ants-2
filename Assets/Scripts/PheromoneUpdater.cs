using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PheromoneUpdater : MonoBehaviour
{
    public float pheromoneDecayInterval;
    private float timeLastPheromoneCheck;
    public float pheromoneDecayRate;
    private float realPheromoneDecayRate;

    public bool diffuse = false;

    public float diffuseThreshold;
    public float diffuseRate;

    public static PheromoneUpdater Instance;

    public IGridTileUpdater gridTileUpdater;

    private void Awake()
    {
        Instance = this;

        gridTileUpdater = new IGridTileUpdater();
    }

    private void Start()
    {
        realPheromoneDecayRate = pheromoneDecayRate * pheromoneDecayInterval;
        timeLastPheromoneCheck = pheromoneDecayInterval + Random.Range(0f, pheromoneDecayInterval);
    }

    public void Update()
    {
        if (timeLastPheromoneCheck < Time.time - pheromoneDecayInterval)
        {
            if (pheromoneDecayRate > 0 || diffuse)
            {
                DecayPheromones();
            }

            timeLastPheromoneCheck = Time.time;
        }
    }

    private void DecayPheromones()
    {
        realPheromoneDecayRate = pheromoneDecayRate * pheromoneDecayInterval * GameInput.simSpeed;

        GridTile[,] tileMap = MapGenerator.Instance.tileMap;
        int borderWallLength = MapGenerator.Instance.borderWallLength;

        float mult = 1 - realPheromoneDecayRate;

        float diffusionAmount = Mathf.Min(1, pheromoneDecayInterval * diffuseRate * GameInput.simSpeed);

        int diffuseDir = Random.value > 0.5f ? 1 : -1;

        StartCoroutine(UpdatePheromoneTilesSprites(tileMap, mult, diffusionAmount, diffuseDir, borderWallLength));
    }

    private IEnumerator UpdatePheromoneTilesSprites(GridTile[,] tileMap, float mult, float diffusionAmount, int diffuseDir, int borderWallLength)
    {
        //int barsDone = barsToDoPerFrame;

        int mapWidth = MapGenerator.mapWidth;
        int mapHeight = MapGenerator.mapHeight;

        //int tilesAmountDifferenceBetweenFrames = barsToDoPerFrame * mapHeight;

        //int tilesAmountOnBorder = borderWallLength * mapWidth;
        //int tilesLengthWithoutBorder = MapGenerator.Instance.tiles.Length - tilesAmountOnBorder;

        //Color[] newColors = new Color[0];

        List<SpriteManager> spriteManagers = SpriteManagerCreator.Instance.spriteManagers;

        foreach (var spriteManager in spriteManagers)
        {
            gridTileUpdater.UpdatePheromones(mult, spriteManager.indexMin, spriteManager.indexMax, diffuse, diffusionAmount, diffuseDir, mapWidth, mapHeight, borderWallLength, diffuseThreshold, spriteManager);
            yield return null;
        }
    }

    public void UpdateAllSprites()
    {
        GridTile[,] tileMap = MapGenerator.Instance.tileMap;

        int borderWallLength = MapGenerator.Instance.borderWallLength;

        for (int x = borderWallLength; x < MapGenerator.mapWidth - borderWallLength; x++)
        {
            for (int y = borderWallLength; y < MapGenerator.mapHeight - borderWallLength; y++)
            {
                tileMap[x, y].UpdateSprite();
            }
        }
    }

    public void OnDestroy()
    {
        gridTileUpdater.DisposeOfArrays();
    }
}
