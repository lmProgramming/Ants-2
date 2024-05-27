using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RandomCreatureSpawner : MonoBehaviour
{
    public static RandomCreatureSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool spawnSpiders = false;
    public bool spawnBeetles = false;

    [SerializeField] private Vector2 spawnTimeBounds;

    [SerializeField] private float lastCheckTime;
    [SerializeField] private float checkInterval;

    private float desiredCreaturesAmount = 10;

    [SerializeField] private float probabilityFunctionMultiplier;

    private List<CreatureManager.CreatureType> spawnableCreatureTypes;
    private bool canSpawnCreatures { get => spawnableCreatureTypes.Count > 0; }

    private void Start()
    {
        SetSpawnableCreatures();
    }

    private List<CreatureManager.CreatureType> SetSpawnableCreatures()
    {
        spawnableCreatureTypes = new List<CreatureManager.CreatureType>();
        if (spawnSpiders)
        {
            spawnableCreatureTypes.Add(CreatureManager.CreatureType.Spider);
        }
        if (spawnBeetles)
        {
            spawnableCreatureTypes.Add(CreatureManager.CreatureType.Beetle);
        }

        return spawnableCreatureTypes;
    }

    private void Update()
    {
        if (!canSpawnCreatures)
        {
            return;
        }

        if (lastCheckTime < GameManager.Instance.simulationTime - checkInterval) 
        { 
            if (CheckIfSpawnCreature())
            {
                SpawnRandomCreature();
                lastCheckTime = GameManager.Instance.simulationTime;
            }
        }
    }

    private void SpawnRandomCreature()
    {
        CreatureManager.CreatureType type = MathExt.RandomFrom(spawnableCreatureTypes);

        for (int i = 0; i < 2; i++)
        {
            Vector2Int gridPosition = MapGenerator.RandomPositionInsideWalls();

            GridTile tile = MapGenerator.Instance.TileAt(gridPosition);
            if (!tile.NearWall)
            {
                if (AntsManager.Instance.FastAntInCircle(tile.position, 25) == null && CreatureManager.Instance.CreatureInCircle(tile.position, 25) == null)
                {
                    SpawnCreature(type, tile.position);
                    return;
                }
            }
        }
    }

    private void OnValidate()
    {
        SetSpawnableCreatures();
    }

    private void SpawnCreature(CreatureManager.CreatureType type, Vector2 position)
    {
        CreatureManager.Instance.SpawnCreature(type, position, true);
    }

    private bool CheckIfSpawnCreature()
    {
        float x = CreatureManager.Instance.existingCreatures.Count / desiredCreaturesAmount;
        float insideSqrt = -probabilityFunctionMultiplier * x + 1;
        if (insideSqrt <= 0)
        {
            return false;
        }

        float probability = Mathf.Sqrt(insideSqrt);
        return Random.value < probability;
    }
}
