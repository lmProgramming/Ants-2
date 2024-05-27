using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Colony : MonoBehaviour
{
    public float foodCollected = 0f;
    private const float AdditionalFood = 100f;
    private int antsSpawned = 0;

    public Vector2 position;

    public Civilization civ;

    public float radius;
    public float removalRadius;

    public UnityEvent Destroyed;
    public UnityEvent SwitchedSides;

    public SpriteRenderer colonySpriteRenderer;
    public SpriteRenderer flagSpriteRenderer;

    public int CivIndex => civ.civIndex;

    private GridTile[] colonyTiles;

    private Ant.AntType nextAntToSpawn;
    private int nextAntCost;

    private PseudoCircularArray<bool> safeTrips;
    private const int MaxTripsData = 40;

    private void Start()
    {
        safeTrips = new PseudoCircularArray<bool>(MaxTripsData);
        safeTrips.FillWith(true);

        SetNextAntToSpawn(Ant.AntType.Worker);
    }

    private Ant.AntType SelectNewAntToSpawn()
    {
        if (CheckIfSpawnQueen())
        {
            return Ant.AntType.Queen;
        }

        if (CheckIfSpawnSoldier())
        {
            return Ant.AntType.Soldier;
        }

        return Ant.AntType.Worker;
    }

    private bool CheckIfSpawnSoldier()
    {
        for (int i = 0; i < 4; i++)
        {
            bool wasFighting = !MathExt.RandomFrom(safeTrips, true);
            if (wasFighting)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfSpawnQueen()
    {
        bool enoughAntsSpawned = false;
        if (antsSpawned > 50)
        {
            enoughAntsSpawned = Random.value < Mathf.Clamp(antsSpawned / 150000f, 0, 0.1f);
        }
        if (!enoughAntsSpawned)
        {
            return false;
        }

        antsSpawned /= 10;

        return true;
    }

    private void SetNextAntToSpawn(Ant.AntType nextAntToSpawnType)
    {
        nextAntToSpawn = nextAntToSpawnType;

        switch (nextAntToSpawnType)
        {
            case Ant.AntType.Worker:
                nextAntCost = AntsManager.WORKER_COST;
                break;
            case Ant.AntType.Soldier:
                nextAntCost = AntsManager.SOLDIER_COST;
                break;
            case Ant.AntType.Queen:
                nextAntCost = AntsManager.QUEEN_COST;
                break;
        }
    }

    public void Initialize(Vector2 pos, Civilization civilization)
    {
        position = pos;
        civ = civilization;

        RemoveFoodAround();

        colonyTiles = MapGenerator.Instance.CircleAroundAssumeInside(position, Mathf.CeilToInt(radius));
        foreach (var colonyTile in colonyTiles)
        {
            colonyTile.UpdateColonyOnTile(this);
        }

        colonySpriteRenderer.sprite = CivilizationsManager.Instance.RandomColonySprite();
        flagSpriteRenderer.sprite = CivilizationsManager.Instance.GetFlagByCivIndex(CivIndex);
    }

    private void RemoveFoodAround()
    {
        GridTile[] tilesToEmptyOfFood = MapGenerator.Instance.CircleAround(MapGenerator.GridPos(position), removalRadius);
        for (int i = 0; i < tilesToEmptyOfFood.Length; i++)
        {
            if (tilesToEmptyOfFood[i].GetTileState() == TileState.Food)
            {
                tilesToEmptyOfFood[i].ChangeToPheromone();
            }
        }
    }

    public void RemoveWallsAround()
    {
        GridTile[] tilesToEmptyOfFood = MapGenerator.Instance.CircleAround(MapGenerator.GridPos(position), removalRadius);
        foreach (var tileToEmptyOfFood in tilesToEmptyOfFood)
        {
            if (tileToEmptyOfFood.GetTileState() == TileState.Wall)
            {
                tileToEmptyOfFood.ChangeToPheromone();
            }
        }
    }

    public static bool CheckIfCouldSpawnColony(Vector2Int posOfNewColony, float radius)
    {
        GridTile[] colonyTiles = MapGenerator.Instance.CircleAroundAssumeInside(posOfNewColony, Mathf.CeilToInt(radius));

        if (colonyTiles.Any(colonyTile => colonyTile.GetColony() is not null || !colonyTile.IsFoodable))
        {
            return false;
        }

        return colonyTiles.Length != 0;
    }

    public void DepositFood(float foodAmount)
    {
        foodCollected += foodAmount;
        civ.foodCollected += foodAmount;

        CheckIfSpawnAnt();
    }

    public float StealFood(Civilization civilizationStealingFood, float antCapacity)
    {
        foodCollected -= antCapacity;

        CheckIfChangeCivilization(civilizationStealingFood);

        return antCapacity;
    }

    private void CheckIfChangeCivilization(Civilization civilizationStealingFood)
    {
        if (!(foodCollected <= 0)) return;
        if (Random.value * 1000 < Mathf.Abs(foodCollected))
        {
            SwitchSides(civilizationStealingFood);
        }
    }

    private void SwitchSides(Civilization newCivilization)
    {
        //Debug.Log("SWITHC");
        civ.RemoveColony(this);
        Destroyed.Invoke();

        newCivilization.AddColony(this);

        civ = newCivilization;

        foreach (var colonyTile in colonyTiles)
        {
            colonyTile.UpdateColonyOnTile(this);
        }

        colonySpriteRenderer.sprite = CivilizationsManager.Instance.RandomColonySprite();
        flagSpriteRenderer.sprite = CivilizationsManager.Instance.GetFlagByCivIndex(CivIndex);
    }

    public Vector2 PositionOfAntAtAngleFromColonyCentre(float angle)
    {
        return (Vector2)transform.position + MathExt.GetXYDirection(angle, Random.Range(radius * 0.8f, radius * 1.2f));
    }

    private void CheckIfSpawnAnt()
    {
        if (foodCollected >= nextAntCost)
        {
            SpawnAnt();
            foodCollected -= nextAntCost;
        }
    }

    public void SpawnAnt()
    {
        civ.SpawnAntAtColony(this, true, nextAntToSpawn);

        SetNextAntToSpawn(SelectNewAntToSpawn());

        antsSpawned++;
    }

    public void GetDestroyed()
    {
        float foodAmountPerTile = (foodCollected + AdditionalFood) / colonyTiles.Length;

        Destroy(gameObject);

        civ.RemoveColony(this);

        Destroyed.Invoke();

        if (foodAmountPerTile <= 0)
        {
            return;
        }

        for (int i = 0; i < colonyTiles.Length; i++)
        {
            colonyTiles[i].ChangeToFood(foodAmountPerTile);
            colonyTiles[i].RemoveColonyMarker();
        }
    }

    public void AddTripSafety(bool safeTrip)
    {
        safeTrips.ReplaceLast(safeTrip);
    }
}
