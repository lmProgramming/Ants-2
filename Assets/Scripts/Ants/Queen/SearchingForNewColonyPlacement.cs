using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingForNewColonyPlacement : Behaviour
{
    private const int ANTS_SPAWNED = 5;
    private Ant ant;

    // TODO just make class DeltaTimer
    public float sensorCheckInterval = 0.3f;

    private float timeLastSensorCheck = 0;

    private float startingTimeBias;

    private float wanderingTime;
    private float sensorToCreateNewColonyInterval = 10;
    private float timeLastNewColonySensorCheck = 0;

    private float minimumDistanceToAnotherColony = 70;

    public SearchingForNewColonyPlacement(Ant ant, float startingTimeBias = 0)
    {
        this.ant = ant;
        this.startingTimeBias = startingTimeBias;
    }

    public override void Start()
    {
        timeLastSensorCheck = Time.time + startingTimeBias + Random.Range(0, sensorCheckInterval);
        timeLastNewColonySensorCheck = Time.time + startingTimeBias + Random.Range(0, sensorToCreateNewColonyInterval);

        ant.antBody.SetFullForce(false);
    }

    override public State GetState()
    {
        return State.FollowingPheromones;
    }

    override public void Update()
    {
        if (timeLastSensorCheck < Time.time - sensorCheckInterval)
        {
            timeLastSensorCheck = Time.time;

            PerformScan();
        }

        if (timeLastNewColonySensorCheck < Time.time - sensorToCreateNewColonyInterval)
        {
            timeLastNewColonySensorCheck = Time.time;

            CheckIfPlaceColony();
        }
    }

    public void PerformScan()
    {
        bool scanForFoodPheromone = true;

        ant.antBrain.ScanState = (AntScanState)(scanForFoodPheromone ? 1 : 2);
    }

    public void CheckIfPlaceColony()
    {
        if (!Colony.CheckIfCouldSpawnColony(MapGenerator.GridPos(ant.Position), CivilizationsManager.GetColonyRadius()))
        {
            return;
        }

        Colony closestColony = CivilizationsManager.Instance.ClosestColony(ant.Position);

        float onFoodMultiplier = (ant.body.TileAt.GetTileState() == TileState.Food) ? 0.66f : 1f;

        bool farEnough = closestColony == null || Vector2.Distance(closestColony.position, ant.Position) > minimumDistanceToAnotherColony * onFoodMultiplier;

        if (!farEnough)
        {
            minimumDistanceToAnotherColony *= 0.9f;

            return;
        }

        Colony spawnedColony = ant.Civilization.SpawnColonyAt(ant.Position, true);

        for (int i = 0; i < ANTS_SPAWNED; i++)
        {
            spawnedColony.SpawnAnt();
        }

        ant.Die();
    }

    public override void Stop()
    {

    }
}
