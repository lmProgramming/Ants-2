using System.Collections.Generic;
using UnityEngine;

public class Civilization : MonoBehaviour
{
    public float foodCollected = 0f;

    public List<Colony> colonies;

    public List<Ant> ants;

    public bool centreCivilization = false;

    public int civIndex;
    public List<int> enemyIndexes;

    public int antsSpawnedNumber = 0;
    public int antsAlive = 0;

    public enum Color
    {
        Red,
        Blue, 
        Yellow, 
        Purple
    }

    public Color color;

    private void Start()
    {
        SetEnemyIndexes();
    }

    public void SetEnemyIndexes()
    {
        enemyIndexes = new List<int>(CivilizationsManager.Instance.activeIndexes);
        enemyIndexes.Remove(civIndex);

        for (int i = 0; i < ants.Count; i++)
        {
            ants[i].enemyCivIndexes = enemyIndexes;
        }
    }

    float CalculateDistanceToNearestColony(Vector2 position)
    {
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < CivilizationsManager.Instance.activeCivilizations.Count; i++)
        {
            for (int j = 0; j < CivilizationsManager.Instance.activeCivilizations[i].colonies.Count; j++)
            {
                float distance = Vector2.Distance(CivilizationsManager.Instance.activeCivilizations[i].colonies[j].position, position);
                minDistance = Mathf.Min(distance, minDistance);
            }
        }

        return minDistance;
    }

    public void SpawnStartingColonies(int coloniesAmount)
    {
        if (centreCivilization)
        {
            AddColony(CivilizationsManager.Instance.SpawnColonyAt(Vector2.zero, this, true));
            coloniesAmount -= 1;
        }

        int tries = 0;
        while (coloniesAmount > 0)
        {
            Vector2 newColonyPos;

            float minimalDistanceBetweenColonies = GameManager.Instance.mapSize / 2;

            bool suitable = false;

            float bestDistance = 0;
            Vector2 bestPos = Vector2.zero;

            while (!suitable)
            {
                newColonyPos = new Vector2(Random.Range(-MapGenerator.mapWidthHalfWithoutWalls + 5, MapGenerator.mapWidthHalfWithoutWalls - 5) * 0.9f, Random.Range(-MapGenerator.mapHeightHalfWithoutWalls + 5, MapGenerator.mapHeightHalfWithoutWalls - 5) * 0.9f);

                suitable = Colony.CheckIfCouldSpawnColony(MapGenerator.GridPos(newColonyPos), CivilizationsManager.GetColonyRadius());

                minimalDistanceBetweenColonies *= 0.95f;
                tries++;

                if (!suitable)
                {
                    if (tries > 100)
                    {
                        coloniesAmount--;
                        break;
                    }

                    continue;
                }

                float minDistance = CalculateDistanceToNearestColony(newColonyPos);

                if (minDistance < minimalDistanceBetweenColonies)
                {
                    suitable = false;
                }

                if (minDistance > bestDistance)
                {
                    bestDistance = minDistance;
                    bestPos = newColonyPos;
                }

                // the position fits all the requirements
                if (suitable)
                {
                    AddColony(CivilizationsManager.Instance.SpawnColonyAt(newColonyPos, this, true));
                    coloniesAmount -= 1;

                    break;
                }
                // spawn the colony in the best ever found spot
                if (tries > 50)
                {
                    AddColony(CivilizationsManager.Instance.SpawnColonyAt(bestPos, this, true));
                    coloniesAmount -= 1;
                }
            }
        }
    }

    public Colony GetClosestColony(Vector2 position)
    {
        float bestDistance = Mathf.Infinity;
        Colony closestColony = null;

        for (int i = 0; i < colonies.Count; i++)
        {
            float distance = Vector2.Distance(colonies[i].position, position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closestColony = colonies[i];
            }
        }

        return closestColony;
    }

    public void AddColony(Colony colony)
    {
        colonies.Add(colony);
    }

    public Colony SpawnColonyAt(Vector2 pos, bool popAnimation = false)
    {
        return CivilizationsManager.Instance.SpawnColonyAt(pos, this, popAnimation:popAnimation);
    }

    public void RemoveAnt(Ant removedAnt)
    {
        ants.Remove(removedAnt);
        AntsManager.Instance.RemoveAnt(removedAnt);
        antsAlive--;
    }

    public void RemoveColony(Colony removedColony)
    {
        colonies.Remove(removedColony);
    }

    public void SpawnStartingAnts(int startingAntsAmount)
    {
        if (colonies.Count == 0)
        {
            return;
        }

        for (int j = 0; j < startingAntsAmount; j++)
        {
            SpawnAntAtColony(colonies[Random.Range(0, colonies.Count)], false, Ant.AntType.Worker);
        }
    }

    public void SpawnAntAtColony(Colony colony, bool addToAntUpdateJob, Ant.AntType antType)
    {
        float antZRotationFromColonyCentre = Random.Range(0, 360);

        AntSpawner.Instance.SpawnAnt(colony.PositionOfAntAtAngleFromColonyCentre(antZRotationFromColonyCentre), antZRotationFromColonyCentre, this, antType, addToAntUpdaterJob: addToAntUpdateJob, popAnimation: true);
    }

    public void SpawnAnt(Ant newAnt)
    {
        antsAlive++;
        antsSpawnedNumber++;

        newAnt.Civilization = this;

        newAnt.enemyCivIndexes = enemyIndexes;
                
        // ted
        if (color == Color.Red && Random.value < (1f / 50000f))
        {
            newAnt.antBody.UpdateSprite(CivilizationsManager.Instance.ted);
            newAnt.antBody.maxHealth *= 30;
            newAnt.antBody.Heal();
        }

        ants.Add(newAnt);
        newAnt.antMovement.SetTargetWithoutUpdatingIAntUpdater(newAnt.transform.position + newAnt.transform.forward);
    }

    public float GetCollectedFood()
    {
        float foodCollected = 0f;
        for (int i = 0; i < colonies.Count; i++)
        {
            foodCollected += colonies[i].foodCollected;
        }
        return foodCollected;
    }
}
