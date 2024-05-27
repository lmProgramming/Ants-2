using System.Collections.Generic;
using UnityEngine;

public sealed class CivilizationsManager : MonoBehaviour
{
    public static CivilizationsManager Instance;

    public List<Civilization> activeCivilizations;
    public List<Civilization> allCivilizations;

    [SerializeField] private Color[] civilizationsColors;

    public Civilization redCivilization;
    public Civilization blueCivilization;
    public Civilization yellowCivilization;
    public Civilization purpleCivilization;

    public Civilization baseCivilization;

    public GameObject colonyPrefab;
    public Transform coloniesHolder;

    public int startingAntsAmount;

    public bool spawnRedCivilization = true;
    public bool spawnBlueCivilization = true;
    public bool spawnYellowCivilization = false;
    public bool spawnPurpleCivilization = false;

    public List<int> activeIndexes = new List<int>();

    public Sprite[] colonySprites;
    public Sprite[] flagSprites;

    public Sprite ted;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }

    public Sprite RandomColonySprite()
    {
        return colonySprites[Random.Range(0, colonySprites.Length)];
    }

    public Sprite GetFlagByCivIndex(int index)
    {
        return flagSprites[index];
    }

    public void SpawnStartingColonies(int startingColoniesAmount)
    {
        if (spawnRedCivilization)
        {
            redCivilization.SpawnStartingColonies(startingColoniesAmount);
            activeCivilizations.Add(redCivilization);
            activeIndexes.Add(0);
        }
        if (spawnBlueCivilization)
        {
            blueCivilization.SpawnStartingColonies(startingColoniesAmount);
            activeCivilizations.Add(blueCivilization);
            activeIndexes.Add(1);
        }
        if (spawnYellowCivilization)
        {
            yellowCivilization.SpawnStartingColonies(startingColoniesAmount);
            activeCivilizations.Add(yellowCivilization);
            activeIndexes.Add(2);
        }
        if (spawnPurpleCivilization)
        {
            purpleCivilization.SpawnStartingColonies(startingColoniesAmount);
            activeCivilizations.Add(purpleCivilization);
            activeIndexes.Add(3);
        }
    }

    public void SpawnStartingAnts()
    {
        for (int i = 0; i < activeCivilizations.Count; i++)
        {
            activeCivilizations[i].SpawnStartingAnts(startingAntsAmount);
        }
    }

    public Civilization GetCivFromIndex(int index)
    {
        switch (index)
        {
            case 0:
                return redCivilization;
            case 1:
                return blueCivilization;
            case 2:
                return yellowCivilization;
            case 3:
                return purpleCivilization;
            default:
                return null;
        }
    }

    public Color GetColorByIndex(int index)
    {
        return civilizationsColors[index];
    }

    public bool CheckIfCanSpawnColony(Vector2Int pos)
    {
        return Colony.CheckIfCouldSpawnColony(pos, colonyPrefab.GetComponent<Colony>().radius);
    }

    public Colony SpawnColonyAt(Vector2 pos, Civilization civilization, bool forceSpawn = false, bool popAnimation = false)
    {
        Vector2Int gridPos = MapGenerator.GridPos(pos);
        Colony colony = null;

        bool canNormallySpawnColony = CheckIfCanSpawnColony(gridPos);

        if (canNormallySpawnColony || forceSpawn)
        {
            GameObject newColony = Instantiate(colonyPrefab, coloniesHolder);

            newColony.transform.SetPositionAndRotation(pos, Quaternion.identity);

            colony = newColony.GetComponent<Colony>();

            if (!canNormallySpawnColony)
            {
                colony.RemoveWallsAround();
            }

            if (popAnimation)
            {
                AnimationSpawner.AddPopAnimation(newColony);
            }

            colony.Initialize(pos, civilization);
        }
        else
        {
            Debug.Log("COULDNT SPAWN COLONY" + " " + gridPos);
        }

        return colony;
    }

    public static float GetColonyRadius()
    {
        return Instance.colonyPrefab.GetComponent<Colony>().radius;
    }

    public Colony ClosestColony(Vector2 pos)
    {
        float closestDistance = Mathf.Infinity;
        Colony closestColony = null;
        foreach (Civilization civilization in activeCivilizations)
        {
            foreach (Colony colony in civilization.colonies)
            {
                float distance = Vector2.Distance(colony.position, pos);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestColony = colony;
                }
            }
        }

        return closestColony;
    }
    //public Colony SpawnColonyFromData(ColonyData colonyData)
    //{
    //    GameObject newColony = Instantiate(colonyPrefab, coloniesHolder);

    //    newColony.transform.SetPositionAndRotation(colonyData.position, Quaternion.identity);

    //    Colony colony = newColony.GetComponent<Colony>();
    //    colony.civ = baseCivilization;
    //    colony.foodCollected = colonyData.foodCollected;

    //    return colony;
    //}
}
