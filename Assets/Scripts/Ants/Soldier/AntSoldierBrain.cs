using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSoldierBrain : AntBrain
{
    public override void HandleScanResults(SensorResultType resultType, Vector2Int position)
    {
        //ant.ss = resultType.ToString() + " " + position;
        switch (resultType)
        {
            case SensorResultType.Pheromone:
                ant.antMovement.SetTarget(MapGenerator.Instance.RealPosition(position));
                break;
            case SensorResultType.Colony:
                HandleColonyBehaviour(position);
                break;
            case SensorResultType.AvoidObstacle:
                ant.antMovement.SetTargetImmediate(MapGenerator.Instance.RealPosition(position));
                break;
            case SensorResultType.EnemyAnt:
                SetBehaviour(new GoingTowardsEnemy(MapGenerator.Instance.TileAtAssumeInsideMap(position).GetCreatureOnTile(), ant, false));
                break;
            case SensorResultType.TurnBack:
                ant.antMovement.RotateToRandomBackDir();
                break;
            default:
                break;
        }

        ScanState = AntScanState.NoScan;
    }
}
