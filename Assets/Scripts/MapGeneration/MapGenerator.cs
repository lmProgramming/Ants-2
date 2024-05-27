using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GridTile[,] tileMap = new GridTile[0, 0];
    public GridTile[] tiles = new GridTile[0];

    public GameObject tilesHolder;

    private GridTile invisibleTile;

    public static MapGenerator Instance;

    public static int mapWidth;
    public static int mapHeight;

    public static int mapWidthWithoutWalls;
    public static int mapHeightWithoutWalls;

    public static int mapWidthHalf;
    public static int mapWidthHalfWithoutWalls;

    public static int mapHeightHalf;
    public static int mapHeightHalfWithoutWalls;

    public int borderWallLength = 10;

    public bool addRandomTerrainTexture = false;
    public float grassTextureStrength = 0.1f;
    public float waterTextureStrength = 0.1f;

    public Color grassColor_r = new Color32(33, 113, 45, 255);

    public Color waterColor_r = new Color32(0, 133, 202, 255);
    public Color beachColor_r = new Color32(255, 243, 109, 255);

    public Color pathPherColor_r = new Color32(0, 19, 127, 192);
    public Color foodPherColor_r = new Color32(127, 0, 0, 192);
    public Color depletedFoodPherColor_r = new Color32(127, 0, 0, 192);

    public Color foodColor_r = new Color32(255, 250, 196, 255);

    public Color wallColor_r = new Color32(31, 31, 31, 255);

    public static Color grassColor = new Color32(33, 113, 45, 255);

    public static Color waterColor = new Color32(0, 133, 202, 255);
    public static Color beachColor = new Color32(255, 243, 109, 255);

    public static Color pathPherColor = new Color32(0, 19, 127, 192);
    public static Color foodPherColor = new Color32(127, 0, 0, 192);
    public static Color depletedFoodPherColor = new Color32(127, 0, 0, 192);

    public static Color foodColor = new Color32(255, 250, 196, 255);

    public static Color wallColor = new Color32(31, 31, 31, 255);

    public SpriteManagerCreator spriteManagerCreator;

    private void Awake()
    {
        Instance = this;

        OnValidate();
    }

    private void OnValidate()
    {
        grassColor = grassColor_r;

        waterColor = waterColor_r;
        beachColor = beachColor_r;

        pathPherColor = pathPherColor_r;
        foodPherColor = foodPherColor_r;
        depletedFoodPherColor = depletedFoodPherColor_r;

        foodColor = foodColor_r;

        wallColor = wallColor_r;
    }

    public GridTile[,] GenerateMap(int mapSize)
    {
        mapWidth = mapSize;
        mapHeight = mapSize;

        mapWidthWithoutWalls = mapWidth - 2 * borderWallLength;
        mapHeightWithoutWalls = mapHeight - 2 * borderWallLength;

        mapWidthHalf = mapWidth / 2;
        mapWidthHalfWithoutWalls = mapWidthWithoutWalls / 2;

        mapHeightHalf = mapWidth / 2;
        mapHeightHalfWithoutWalls = mapHeightWithoutWalls / 2;

        tileMap = new GridTile[mapWidth, mapHeight];

        tiles = new GridTile[mapWidth * mapHeight];

        float grassTextureStrength = this.grassTextureStrength * (addRandomTerrainTexture ? 1 : 0);
        float waterTextureStrength = this.waterTextureStrength * (addRandomTerrainTexture ? 1 : 0);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int tileIndex = x + y * mapHeight;
                SpriteManager spriteManager = spriteManagerCreator.AssignSpriteManager(tileIndex);
                GridTile pheromone = new GridTile(spriteManager, tilesHolder, new Vector2(x - mapWidthHalf + 0.5f, y - mapHeightHalf + 0.5f), new Vector2Int(x, y), grassTextureStrength, waterTextureStrength, PheromoneUpdater.Instance.gridTileUpdater);

                tileMap[x, y] = pheromone;

                tiles[x + y * mapWidth] = pheromone;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tileMap[x, y].SetUpNineTilesAround();
            }
        }

        PheromoneUpdater.Instance.gridTileUpdater.InitializeLists(tiles);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GridTile pheromone = tileMap[x, y];

                if (x < borderWallLength || x >= mapWidth - borderWallLength || y < borderWallLength || y >= mapHeight - borderWallLength)
                {
                    pheromone.ChangeToWall();
                }
                else
                {
                    pheromone.ChangeToPheromone();
                }
            }
        }

        invisibleTile = tiles[0];

        return tileMap;
    }

    public void AddPheromoneAt(Vector2 position, int civIndex, bool foodPheromone, float amount)
    {
        GridTile pheromone = TileAtAssumeInsideMap(position);

        AddPheromoneAt(pheromone, civIndex, foodPheromone, amount);
    }

    public void AddPheromoneAt(GridTile pheromone, int civIndex, bool foodPheromone, float amount)
    {
        pheromone.AddPheromone(foodPheromone, civIndex, amount);
    }

    public static Vector2Int GridPos(Vector2 position)
    {
        return new Vector2Int((int)(position.x + mapWidthHalf), (int)(position.y + mapHeightHalf));
    }

    public GridTile TileAtArrayIndex(int arrayIndex)
    {
        return tiles[arrayIndex];
    }

    public GridTile TileAt(Vector2Int position)
    {
        if (position.x < mapWidth && position.y < mapHeight && position.x >= 0 && position.y >= 0)
        {
            return tileMap[position.x, position.y];
        }

        return invisibleTile;
    }

    public GridTile TileAt(Vector2 position)
    {
        Vector2Int gridPos = GridPos(position);

        if (gridPos.x < mapWidth && gridPos.y < mapHeight && gridPos.x >= 0 && gridPos.y >= 0)
        {
            return tileMap[gridPos.x, gridPos.y];
        }

        return invisibleTile;
    }

    public GridTile TileAtAssumeInsideMap(Vector2Int gridPos)
    {
        return tileMap[gridPos.x, gridPos.y];
    }

    public GridTile TileAtAssumeInsideMap(Vector2 position)
    {
        Vector2Int gridPos = GridPos(position);
        return tileMap[gridPos.x, gridPos.y];
    }

    public GridTile[] CircleAround(Vector2Int centrePosition, float radius)
    {
        int radiusInt = (int)radius;

        List<GridTile> gridTiles = new List<GridTile>();

        for (int x = Mathf.Max(centrePosition.x - radiusInt, 0); x <= Mathf.Min(centrePosition.x + radiusInt, mapWidth - 1); x++)
        {
            for (int y = Mathf.Max(centrePosition.y - radiusInt, 0); y <= Mathf.Min(centrePosition.y + radiusInt, mapHeight - 1); y++)
            {
                if (Vector2.Distance(new Vector2(x, y), centrePosition) <= radius)
                {
                    gridTiles.Add(tileMap[x, y]);
                }
            }
        }

        return gridTiles.ToArray();
    }

    public GridTile[] CircleAroundExcludingMapBorderWalls(Vector2Int centrePosition, float radius)
    {
        int radiusInt = (int)radius;

        List<GridTile> gridTiles = new();

        for (int x = Mathf.Max(centrePosition.x - radiusInt, borderWallLength); x <= Mathf.Min(centrePosition.x + radiusInt, mapWidth - borderWallLength - 1); x++)
        {
            for (int y = Mathf.Max(centrePosition.y - radiusInt, borderWallLength); y <= Mathf.Min(centrePosition.y + radiusInt, mapHeight - borderWallLength - 1); y++)
            {
                if (Vector2.Distance(new Vector2(x, y), centrePosition) <= radius)
                {
                    gridTiles.Add(tileMap[x, y]);
                }
            }
        }

        return gridTiles.ToArray();
    }

    public GridTile[] ScatterAroundExcludingMapBorderWalls(Vector2Int centrePosition, float radius, float sprayDensity)
    {
        int radiusInt = (int)radius;

        List<GridTile> gridTiles = new();

        int width = Mathf.CeilToInt(radius * 2);
        bool[,] boolMap = SizeOption.GenerateRandomSprayPattern(width, width, sprayDensity, 0);

        int i = 0;
        for (int x = Mathf.Max(centrePosition.x - radiusInt, borderWallLength - 1); x <= Mathf.Min(centrePosition.x + radiusInt, mapWidth - borderWallLength - 1); x++)
        {
            int j = 0;
            for (int y = Mathf.Max(centrePosition.y - radiusInt, borderWallLength - 1); y <= Mathf.Min(centrePosition.y + radiusInt, mapHeight - borderWallLength - 1); y++)
            {
                if (Vector2.Distance(new Vector2(x, y), centrePosition) < radius && boolMap[i, j])
                {
                    gridTiles.Add(tileMap[x, y]);
                }

                j++;
            }

            i++;
        }

        return gridTiles.ToArray();
    }

    public GridTile[] CircleAroundAssumeInside(Vector2Int centrePosition, float radius)
    {
        int radiusInt = (int)radius;

        List<GridTile> gridTiles = new List<GridTile>();

        for (int x = Mathf.Max(centrePosition.x - radiusInt, 0); x <= Mathf.Min(centrePosition.x + radiusInt, mapWidth - 1); x++)
        {
            for (int y = Mathf.Max(centrePosition.y - radiusInt, 0); y <= Mathf.Min(centrePosition.y + radiusInt, mapHeight - 1); y++)
            {
                if (Vector2.Distance(new Vector2(x, y), centrePosition) <= radius)
                {
                    gridTiles.Add(tileMap[x, y]);
                }
            }
        }

        return gridTiles.ToArray();
    }

    public GridTile[] CircleAroundAssumeInside(Vector2 centrePos, float radius)
    {
        Vector2Int centrePosition = GridPos(centrePos);

        return CircleAroundAssumeInside(centrePosition, radius);
    }

    public GridTile[] XTilesInRectFromLeftUpperCornerPos(Vector2 positionOfLeftUpperCorner, int xNum)
    {
        Vector2Int gridPos = GridPos(positionOfLeftUpperCorner);

        if (gridPos.x > 0 && gridPos.y > 0 && gridPos.x + xNum < mapWidth && gridPos.y + xNum < mapHeight)
        {
            GridTile[] tiles = new GridTile[xNum * xNum];
            int i = 0;
            for (int x = 0; x < xNum; x++)
            {
                for (int y = 0; y < xNum; y++)
                {
                    tiles[i] = tileMap[gridPos.x + x, gridPos.y + y];
                    i++;
                }
            }

            return tiles;
        }

        return new GridTile[0];
    }

    public GridTile[] GetXTilesInRectFromCentreGridPos(Vector2Int gridPos, int xNum)
    {
        int halfSize = xNum / 2;
        int startX = gridPos.x - halfSize;
        int startY = gridPos.y - halfSize;

        if (startX >= 0 && startY >= 0 && startX + halfSize <= mapWidth && startY + halfSize <= mapHeight)
        {
            List<GridTile> tiles = new List<GridTile>();
            int index = 0;

            for (int x = 0; x < xNum; x++)
            {
                for (int y = 0; y < xNum; y++)
                {
                    tiles.Add(tileMap[startX + x, startY + y]);
                    index++;
                }
            }

            return tiles.ToArray();
        }

        return new GridTile[0];
    }

    public GridTile[] GetXTilesInRectFromCentreGridPosExcludingMapBorderWalls(Vector2Int gridPos, int xNum)
    {
        int halfSize = xNum / 2;
        int startX = gridPos.x - halfSize;
        int startY = gridPos.y - halfSize;
        int endX = gridPos.x + halfSize;
        int endY = gridPos.y + halfSize;

        startX = Mathf.Clamp(startX, borderWallLength, mapWidth - borderWallLength);
        startY = Mathf.Clamp(startY, borderWallLength, mapHeight - borderWallLength);

        endX = Mathf.Clamp(endX, borderWallLength, mapWidth - borderWallLength);
        endY = Mathf.Clamp(endY, borderWallLength, mapHeight - borderWallLength);

        if (startX >= 0 && startY >= 0 && startX + halfSize <= mapWidth && startY + halfSize <= mapHeight)
        {
            List<GridTile> tiles = new List<GridTile>();
            int index = 0;

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    tiles.Add(tileMap[x, y]);
                    index++;
                }
            }

            return tiles.ToArray();
        }

        return new GridTile[0];
    }

    public GridTile[] XTilesInRectFromLeftUpperCornerGridPos(Vector2Int gridPos, int xNum)
    {
        if (gridPos.x > 0 && gridPos.y > 0 && gridPos.x + xNum < mapWidth && gridPos.y + xNum < mapHeight)
        {
            GridTile[] tiles = new GridTile[xNum * xNum];
            int i = 0;
            for (int x = 0; x < xNum; x++)
            {
                for (int y = 0; y < xNum; y++)
                {
                    tiles[i] = tileMap[gridPos.x + x, gridPos.y + y];
                    i++;
                }
            }

            return tiles;
        }

        return new GridTile[0];
    }

    public GridTile[] XTilesInRectFromLeftUpperCornerGridPosAssumeInside(Vector2Int gridPos, int xNum)
    {
        GridTile[] tiles = new GridTile[xNum * xNum];
        int i = 0;
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < xNum; y++)
            {
                tiles[i] = tileMap[gridPos.x + x, gridPos.y + y];
                i++;
            }
        }

        return tiles;
    }

    public GridTile[] XTilesInRectFromLeftUpperCornerPosAssumeInside(Vector2 positionOfLeftUpperCorner, int xNum)
    {
        Vector2Int gridPos = GridPos(positionOfLeftUpperCorner);

        GridTile[] tiles = new GridTile[xNum * xNum];
        int i = 0;
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < xNum; y++)
            {
                tiles[i] = tileMap[gridPos.x + x, gridPos.y + y];
                i++;
            }
        }

        return tiles;
    }

    public Vector2 RealPosition(Vector2Int tilePos)
    {
        return new Vector2((tilePos.x - (mapWidthHalf + 0.5f)), (tilePos.y - (mapHeightHalf + 0.5f)));
    }

    public bool InsideMap(Vector2 pos)
    {
        Vector2Int gridPos = GridPos(pos);

        return gridPos.x < mapWidth && gridPos.y < mapHeight && gridPos.x >= 0 && gridPos.y >= 0;
    }

    public bool InsideMap(Vector2Int gridPos)
    {
        return gridPos.x < mapWidth && gridPos.y < mapHeight && gridPos.x >= 0 && gridPos.y >= 0;
    }

    public bool AlmostInsideMap(Vector2Int gridPos)
    {
        return gridPos.x < mapWidth - 15 && gridPos.y < mapHeight - 15 && gridPos.x >= 15 && gridPos.y >= 15;
    }

    public static Vector2Int RandomPositionInsideWalls()
    {
        return new Vector2Int(Random.Range(Instance.borderWallLength, mapWidthWithoutWalls), Random.Range(Instance.borderWallLength, mapHeightWithoutWalls));
    }

    // returns null if hasn't found any food
    public float GatherFoodAroundTile(GridTile tileScanned, float upToAmount, int civIndex)
    {
        GridTile[] tilesArroundFormerFoodTarget = tileScanned.NineTilesAround();

        float foodGathered = 0;

        for (int i = 0; i < tilesArroundFormerFoodTarget.Length; i++)
        {
            if (tilesArroundFormerFoodTarget[i].GetTileState() == TileState.Food)
            {
                float foodFromThisTile;

                // if is an ant
                if (AntsManager.IsCivIndexAntIndex(civIndex))
                {
                    foodFromThisTile = tilesArroundFormerFoodTarget[i].GetFood(upToAmount, civIndex);
                }
                else
                {
                    foodFromThisTile = tilesArroundFormerFoodTarget[i].GetFoodWithoutDepletedPheromone(upToAmount);
                }

                upToAmount -= foodFromThisTile;
                foodGathered += foodFromThisTile;

                if (upToAmount < 5)
                {
                    break;
                }
            }
        }

        return foodGathered;
    }

    public static Vector2 CentralizeVectorPosition(Vector2 position)
    {
        return new Vector2(Mathf.Round(position.x * 2) / 2f, Mathf.Round(position.y * 2) / 2f);
    }
}