using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TileState
{
    Pheromone,
    Water,
    Wall,
    Food
}

public class GridTile
{
    public Vector2 position;

    public float foodAmount = 0;

    private IGridTileUpdater gridTileUpdater;

    public const int PATH_PHEROMONE = 0;
    public const int FOOD_PHEROMONE = 1;
    public const int DEPLETEDFOOD_PHEROMONE = 2;

    public Vector2Int gridPos;
    public int arrayIndex;

    private SMSprite smSprite;
    private SpriteManager spriteManager;

    private Creature creatureOnTile;

    public Color baseGrassTexture;
    public Color baseWaterTexture;

    public bool NearWall { get => gridTileUpdater.nearWallA[arrayIndex]; set => gridTileUpdater.nearWallA[arrayIndex] = value; }

    public void SetCreatureOnTile(Creature creature)
    {
        creatureOnTile = creature;
        gridTileUpdater.antOnTileCivIndexesA[arrayIndex] = creature.CivIndex;
    }

    public Creature GetCreatureOnTile()
    {
        return creatureOnTile;
    }

    public Colony GetColony()
    {
        return IGridTileUpdater.Instance.colonyOnTiles[arrayIndex];
    }

    public void UpdateColonyOnTile(Colony colony)
    {
        gridTileUpdater.colonyOnTiles[arrayIndex] = colony;
        gridTileUpdater.colonyOnTileCivIndexA[arrayIndex] = colony.civ.civIndex;
    }

    public void SetTileState(TileState state)
    {
        gridTileUpdater.tileStatesA[arrayIndex] = state;

        UpdateIsWallInNeighbours();
        UpdateSpriteInNeighbours();
    }

    public TileState GetTileState()
    {
        return gridTileUpdater.tileStatesA[arrayIndex];
    }

    public bool IsFoodable => GetTileState() == TileState.Pheromone || GetTileState() == TileState.Food;

    public bool IsSpriteBasedOnNeighbours() => GetTileState() == TileState.Water;

    public GridTile()
    {

    }

    private GridTile[] nineTilesAround = new GridTile[9];

    public void SetUpNineTilesAround()
    {
        if (gridPos.x > 1 && gridPos.y > 1 && gridPos.x < MapGenerator.mapWidth - 1 && gridPos.y < MapGenerator.mapHeight - 1)
        {
            nineTilesAround = new GridTile[9]
            {
                MapGenerator.Instance.tileMap[gridPos.x - 1, gridPos.y + 1],
                MapGenerator.Instance.tileMap[gridPos.x - 1, gridPos.y],
                MapGenerator.Instance.tileMap[gridPos.x - 1, gridPos.y - 1],
                MapGenerator.Instance.tileMap[gridPos.x, gridPos.y + 1],
                this,
                MapGenerator.Instance.tileMap[gridPos.x, gridPos.y - 1],
                MapGenerator.Instance.tileMap[gridPos.x + 1, gridPos.y + 1],
                MapGenerator.Instance.tileMap[gridPos.x + 1, gridPos.y],
                MapGenerator.Instance.tileMap[gridPos.x + 1, gridPos.y - 1]
            };
        }
        else
        {
            nineTilesAround = new GridTile[1] { this };
        }
    }

    public bool CheckIfNeedToChangeGridTileIsWall(TileState formerTileState, TileState currentTileState)
    {
        return (formerTileState == TileState.Wall && currentTileState != TileState.Wall) || (formerTileState != TileState.Wall && currentTileState == TileState.Wall);
    }

    public bool NeighbourIsWall()
    {
        for (int i = 0; i < nineTilesAround.Length; i++)
        {
            if (nineTilesAround[i].GetTileState() == TileState.Wall)
            {
                NearWall = true;
                return NearWall;
            }
        }

        NearWall = false;
        return NearWall;
    }

    public int CountNeighboursOfSameType()
    {
        int waterCount = 0;

        for (int i = 0; i < nineTilesAround.Length; i++)
        {
            if (nineTilesAround[i].GetTileState() == GetTileState())
            {
                waterCount++;
            }
        }

        return waterCount;
    }

    public int CountNeighboursOfFoodableType()
    {
        int tileCount = 0;

        for (int i = 0; i < nineTilesAround.Length; i++)
        {
            if (nineTilesAround[i].IsFoodable)
            {
                tileCount++;
            }
        }

        return tileCount;
    }

    public GridTile[] NineTilesAround()
    {
        return nineTilesAround;
    }

    public GridTile(SpriteManager _spriteManager, GameObject parent, Vector2 pos, Vector2Int _gridPos, float grassTextureStrength, float waterTextureStrength, IGridTileUpdater gridTileUpdater)
    {
        this.gridTileUpdater = gridTileUpdater;

        position = pos;

        spriteManager = _spriteManager;
        smSprite = spriteManager.AddSprite(parent, 0.1f, 0.1f, Vector2.zero, new Vector2(1f, 1f), position, false);

        smSprite.SetSizeXY(1, 1);

        gridPos = _gridPos;

        SetupBaseTextures(grassTextureStrength, waterTextureStrength);

        arrayIndex = gridPos.x + gridPos.y * MapGenerator.mapHeight;
    }

    private void SetupBaseTextures(float grassTextureStrength, float waterTextureStrength)
    {
        Color baseNoiseColor = new Color(Random.value, Random.value, Random.value);

        baseGrassTexture = baseNoiseColor * grassTextureStrength + MapGenerator.grassColor;
        baseWaterTexture = baseNoiseColor * waterTextureStrength + MapGenerator.waterColor;
    }

    public GridTile(Vector2 pos, Vector2Int _gridPos)
    {
        position = pos;
        gridPos = _gridPos;
    }

    public void AddPheromone(bool foodPheromone, int civIndex, float amount)
    {
        if (GetTileState() == TileState.Pheromone && !NearWall)
        {
            if (foodPheromone)
            {
                AddFoodPheromone(civIndex, amount);
            }
            else
            {
                AddPathPheromone(civIndex, amount);
            }

            UpdateSprite();
        }
    }

    public void AddPathPheromone(int civIndex, float amount)
    {
        gridTileUpdater.AddPheromone(arrayIndex, civIndex, PATH_PHEROMONE, amount);
    }

    public void AddFoodPheromone(int civIndex, float amount)
    {
        gridTileUpdater.AddPheromone(arrayIndex, civIndex, FOOD_PHEROMONE, amount);
    }

    public void RawAddPheromone(int civIndex, int pheromoneIndex, float amount)
    {
        gridTileUpdater.AddPheromone(arrayIndex, civIndex, pheromoneIndex, amount);
    }

    public void MultiplyFoodPheromoneInNeighbours(float mult, int civIndex)
    {
        for (int i = 0; i < nineTilesAround.Length; i++)
        {
            gridTileUpdater.MultiplyFoodPheromonePheromones(arrayIndex, mult, civIndex);
        }
    }

    public void MultiplyPheromonesAndNeighbours(float mult, int civIndex)
    {
        for (int i = 0; i < nineTilesAround.Length; i++)
        {
            gridTileUpdater.MultiplyPheromones(nineTilesAround[i].arrayIndex, mult, civIndex);
        }
    }

    public void MultiplyPheromones(float mult, int civIndex)
    {
        gridTileUpdater.MultiplyPheromones(arrayIndex, mult, civIndex);
    }

    public void SetPheromone(int civIndex, int pheromoneIndex, float amount)
    {
        gridTileUpdater.SetPheromone(arrayIndex, civIndex, pheromoneIndex, amount);
    }

    public void RegressDepletedFoodPheromone(int civIndex)
    {
        float depletedPheromoneAmount = PheromoneAmount(civIndex, DEPLETEDFOOD_PHEROMONE);
        depletedPheromoneAmount *= 0.9f;
        depletedPheromoneAmount -= 0.5f;

        gridTileUpdater.SetPheromone(arrayIndex, civIndex, DEPLETEDFOOD_PHEROMONE, depletedPheromoneAmount);
    }

    public void ChangeToWater()
    {
        SetTileState(TileState.Water);

        ResetValues();

        UpdateSprite();
    }

    public void UpdateSprite()
    {
        UpdateSprite(GameManager.Instance.civIndexOfCurrentlyShownPheromonesInTiles);
    }

    public SMSprite GetSprite()
    {
        return smSprite;
    }

    public float PheromoneAmount(int civIndex, int pheromoneIndex)
    {
        return gridTileUpdater.GetPheromoneAt(arrayIndex, civIndex)[pheromoneIndex];
    }

    public void UpdateSprite(int civIndex)
    {
        switch (GetTileState())
        {
            case TileState.Pheromone:
                if (civIndex >= 0)
                {
                    gridTileUpdater.UpdatePheromoneSpriteManually(arrayIndex, civIndex);
                }
                else
                {
                    smSprite.SetColor(baseGrassTexture);
                }
                break;
            case TileState.Food:
                smSprite.SetColor(Color.Lerp(baseGrassTexture, MapGenerator.foodColor, Mathf.Sqrt(foodAmount / FoodSpawner.Instance.foodAmountBounds.y)));
                break;
            case TileState.Water:
                int foodableNeighbours = CountNeighboursOfFoodableType();
                /* formula:
                 * x = int at which beach starts to dissapear
                 * y = int at which water is full
                 * 
                 * a = 1 / (x-y)
                 * b = 1 - x / (x-y)
                 * t = a * n + b
                 * x = 6, y = 9
                */
                smSprite.SetColor(Color.Lerp(baseWaterTexture, MapGenerator.beachColor, -0.5f * (9 - foodableNeighbours) + 4.5f));
                break;
            case TileState.Wall:
                smSprite.SetColor(MapGenerator.wallColor);
                break;
            default: throw new Exception("no tile state");
        }
    }

    public void ResetAntsNumbers()
    {
        creatureOnTile = null;
        gridTileUpdater.antOnTileCivIndexesA[arrayIndex] = -1;
    }

    public void RemoveColonyMarker()
    {
        gridTileUpdater.colonyOnTileCivIndexA[arrayIndex] = -1;
    }

    public void ChangeToWall()
    {
        SetTileState(TileState.Wall);

        ResetValues();

        UpdateSprite();
    }

    public void UpdateIsWallInNeighbours()
    {
        for (int i = 0; i < nineTilesAround.Length; i++)
        {
            if (nineTilesAround[i] != this)
            {
                nineTilesAround[i].NeighbourIsWall();
            }
        }
    }

    public void UpdateSpriteInNeighbours()
    {
        for (int i = 0; i < nineTilesAround.Length; i++)
        {
            if (nineTilesAround[i] != this && nineTilesAround[i].IsSpriteBasedOnNeighbours())
            {
                nineTilesAround[i].UpdateSprite();
            }
        }
    }

    public void ChangeToFood(float startingFood)
    {
        if (startingFood > 0)
        {
            if (GetColony() != null)
            {
                GetColony().DepositFood(startingFood);
                return;
            }

            SetTileState(TileState.Food);

            ResetValues();

            if (foodAmount > 0)
            {
                startingFood += foodAmount;
            }

            foodAmount = startingFood;
            
            UpdateSprite();
        }
        else
        {
            Debug.LogWarning("startingFood <= 0 :(");
        }
    }

    public void AddOrChangeToFood(float food)
    {
        if (GetTileState() == TileState.Food)
        {
            AddFood(food);
        }
        else
        {
            ChangeToFood(food);
        }
    }

    public bool AddOrChangeToFoodIfFoodable(float food)
    {
        if (IsFoodable)
        {
            AddOrChangeToFood(food);
            return true;
        }
        return false;
    }

    public void AddFood(float food)
    {
        if (GetTileState() == TileState.Food)
        {
            foodAmount += food;

            foodAmount = Mathf.Min(foodAmount, FoodSpawner.Instance.foodAmountBounds.y * 1.5f);

            UpdateSprite();
        }
    }

    public void ChangeToDepletedFoodPheromone(int civIndex, int depletedFoodAmount)
    {
        SetTileState(TileState.Pheromone);

        ResetValues();

        gridTileUpdater.AddPheromone(arrayIndex, civIndex, DEPLETEDFOOD_PHEROMONE, depletedFoodAmount);

        UpdateSprite();
    }

    public void ChangeToPheromone()
    {
        SetTileState(TileState.Pheromone);

        ResetValues();

        UpdateSprite();
    }

    private void ResetValues()
    {
        foodAmount = 0;

        gridTileUpdater.ResetPheromone(arrayIndex);

        ResetAntsNumbers();
    }

    public float GetFoodWithoutDepletedPheromone(float maxAmountGathered)
    {
        if (GetTileState() == TileState.Food)
        {
            if (maxAmountGathered >= foodAmount)
            {
                float formerFoodAmount = foodAmount;

                ChangeToPheromone();

                return formerFoodAmount;
            }
            else
            {
                foodAmount -= maxAmountGathered;

                UpdateSprite();

                return maxAmountGathered;
            }
        }

        return 0;
    }

    public float GetFood(float maxAmountGathered, int civIndex)
    {
        if (GetTileState() == TileState.Food)
        {
            if (maxAmountGathered >= foodAmount)
            {
                float formerFoodAmount = foodAmount;

                ChangeToDepletedFoodPheromone(civIndex, 30);

                return formerFoodAmount;
            }
            else
            {
                foodAmount -= maxAmountGathered;

                UpdateSprite();

                return maxAmountGathered;
            }
        }

        return 0;
    }
}