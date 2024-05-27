using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// deals only with creatures OTHER than ants
public sealed class CreatureManager : MonoBehaviour
{
    public static CreatureManager Instance;

    public GameObject spiderPrefab;
    public GameObject beetlePrefab;

    public List<Creature> existingCreatures = new List<Creature>();

    public Transform creaturesHolder;

    public int creaturesSpawned = 0;

    private void Awake()
    {
        Instance = this;
    }

    public enum CreatureType
    {
        Beetle,
        Spider,
        Ant
    }

    public Creature CreatureClosestToPosition(Vector2 position, float maxDistance, bool includeAnts = true)
    {
        float lowestDistance = maxDistance;
        Creature closestCreature = null;

        for (int i = 0; i < existingCreatures.Count; i++)
        {
            float distance = Vector2.Distance(existingCreatures[i].Position, position);
            if (distance < lowestDistance)
            {
                lowestDistance = distance;
                closestCreature = existingCreatures[i];
            }
        }

        if (includeAnts)
        {
            Creature closestAnt = AntsManager.Instance.AntClosestToPosition(position, maxDistance);

            if (closestAnt != null && Vector2.Distance(closestAnt.Position, position) < lowestDistance)
            {
                closestCreature = closestAnt;
            }
        }

        return closestCreature;
    }

    public bool CheckIfCanSpawn(Vector2 position)
    {
        return MapGenerator.Instance.TileAt(position).GetTileState() != TileState.Wall;
    }

    private GameObject CreaturePrefabFromType(CreatureType type)
    {
        switch (type)
        {
            case CreatureType.Beetle:
                return beetlePrefab;
            case CreatureType.Spider:
                return spiderPrefab;
            default:
                return null;
        }
    }

    public Creature SpawnCreature(CreatureType creatureType, Vector2 position, bool popAnimation = false)
    {
        if (CheckIfCanSpawn(position))
        {
            GameObject prefab = CreaturePrefabFromType(creatureType);

            GameObject newCreature = Instantiate(prefab, creaturesHolder);

            newCreature.transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, Random.Range(-360, 360)));

            Creature creature = newCreature.GetComponent<Creature>();

            creature.movement.Position = position;

            existingCreatures.Add(creature);

            creaturesSpawned++;

            if (popAnimation)
            {
                AnimationSpawner.AddPopAnimation(newCreature);
            }

            return creature;
        }

        return null;
    }

    private void Update()
    {
        if (!GameManager.Instance.paused)
        {
            for (int i = 0; i < existingCreatures.Count; i++)
            {
                existingCreatures[i].UpdateCreature();
            }
        }
    }

    public Creature[] CreaturesInCircle(Vector2 centerOfCircle, float radius)
    {
        List<Creature> creaturesInsideCircle = new List<Creature>();

        for (int i = 0; i < existingCreatures.Count; i++)
        {
            if (Vector2.Distance(existingCreatures[i].Position, centerOfCircle) < radius)
            {
                creaturesInsideCircle.Add(existingCreatures[i]);
            }
        }

        return creaturesInsideCircle.ToArray();
    }

    public Creature CreatureInCircle(Vector2 centerOfCircle, float radius)
    {
        for (int i = 0; i < existingCreatures.Count; i++)
        {
            if (Vector2.Distance(existingCreatures[i].Position, centerOfCircle) < radius)
            {
                return existingCreatures[i];
            }
        }

        return null;
    }

    internal void RemoveCreatureFromList(Creature creature)
    {
        existingCreatures.Remove(creature);
    }
}
