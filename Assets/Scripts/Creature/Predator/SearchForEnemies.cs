using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SearchForEnemies : Behaviour
{
    public float sensorCheckInterval = 0.3f;

    private float timeLastSensorCheck = 0;

    private float fieldOfView = 60;
    private int SCAN_RAY_LENGTH = 15;

    public SearchForEnemies(Creature creature)
    {
        this.creature = creature;
    }

    public override State GetState()
    {
        return State.SearchingForEnemies;
    }

    public override void Start()
    {

    }

    override public void Update()
    {
        if (timeLastSensorCheck > Time.time - sensorCheckInterval)
            return;

        timeLastSensorCheck = Time.time;

        Creature creatureFound = PerformScan();

        if (creatureFound != null)
        {
            creature.brain.SetBehaviour(new GoingTowardsEnemy(creatureFound, creature, false));
        }
        else if (allNegative && !allZero)
        {
            creature.movement.RotateToBack();
        }
    }

    private bool allNegative = true;
    private bool allZero = true;

    private Creature PerformScan()
    {
        Vector2Int startGridPos = MapGenerator.GridPos(creature.Position);
        float zAngle = creature.movement.GetAngle();

        allNegative = true;
        allZero = true;
        for (int i = 0; i < 3; i++)
        {
            Creature creatureFound = ScanRay(startGridPos, zAngle + Random.Range(-fieldOfView, fieldOfView), SCAN_RAY_LENGTH);

            if (creatureFound != null)
            {
                return creatureFound;
            }
        }

        return null;
    }

    private int scanned = 0;

    private Creature ScanRay(Vector2Int startGridPos, float zAngle, int tilesToVisit)
    {
        float radians = Mathf.Deg2Rad * zAngle;
        int dx = (int)(Mathf.Cos(radians) * tilesToVisit);
        int dy = (int)(Mathf.Sin(radians) * tilesToVisit);

        int x = startGridPos.x;
        int y = startGridPos.y;

        int endX = x + dx;
        int endY = y + dy;

        int stepX = (int)Mathf.Sign(dx);
        int stepY = (int)Mathf.Sign(dy);

        dx = Mathf.Abs(dx);
        dy = Mathf.Abs(dy);

        int dx2 = dx << 1;
        int dy2 = dy << 1;

        scanned = 0;
        if (dx > dy)
        {
            int fraction = dy2 - dx;

            while (x != endX || y != endY)
            {
                scanned++;

                if (fraction >= 0)
                {
                    y += stepY;
                    fraction -= dx2;
                }
                x += stepX;
                fraction += dy2;

                int sensorPositionIndex = x + MapGenerator.mapWidth * y;

                if (IGridTileUpdater.Instance.antOnTileCivIndexesA[sensorPositionIndex] != -1 && IGridTileUpdater.Instance.antOnTileCivIndexesA[sensorPositionIndex] != Predator.PREDATOR_CIV_INDEX)
                {
                    return MapGenerator.Instance.TileAtArrayIndex(sensorPositionIndex).GetCreatureOnTile();
                }

                if (IGridTileUpdater.Instance.tileStatesA[sensorPositionIndex] == TileState.Wall)
                {
                    return null;
                }
            }
        }
        else
        {
            int fraction = dx2 - dy;

            while (x != endX || y != endY)
            {
                scanned++;

                if (fraction >= 0)
                {
                    x += stepX;
                    fraction -= dy2;
                }
                y += stepY;
                fraction += dx2;

                int sensorPositionIndex = x + MapGenerator.mapWidth * y;

                if (IGridTileUpdater.Instance.antOnTileCivIndexesA[sensorPositionIndex] != -1 && IGridTileUpdater.Instance.antOnTileCivIndexesA[sensorPositionIndex] != Predator.PREDATOR_CIV_INDEX)
                {
                    return MapGenerator.Instance.TileAtArrayIndex(sensorPositionIndex).GetCreatureOnTile();
                }

                if (IGridTileUpdater.Instance.tileStatesA[sensorPositionIndex] == TileState.Wall)
                {
                    return null;
                }
            }
        }

        return null;
    }

    public override void Stop()
    {

    }
}
