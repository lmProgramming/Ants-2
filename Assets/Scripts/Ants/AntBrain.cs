using System;
using UnityEngine;

public enum SensorResultType
{
    Food,
    Colony,
    Pheromone,
    EnemyAnt,
    Nothing,
    TurnBack,
    AvoidObstacle
}

public class AntBrain : Brain
{
    [HideInInspector]
    public Ant ant;

    public AntScanState ScanState { get => scanState; set => SetScanState(value); }
    private AntScanState scanState;
    public float antAutonomy = 0;
    public float antBaseAutonomy;

    protected override void Awake()
    {
        base.Awake();
        ant = (Ant) creature;
    }

    protected override void Start()
    {
        base.Start();
        IAntUpdateJobsManager.Instance.UpdateAntMovement(ant);
    }

    private void SetScanState(AntScanState scanState)
    {
        this.scanState = scanState;
        IAntUpdateJobsManager.Instance.antScanStatesNA[ant.UpdateJobIndex] = scanState;
    }

    public void SetBaseAutonomy(float baseAutonomy)
    {
        antBaseAutonomy = baseAutonomy;
    }

    public override void SetDefaultBehaviour()
    {
        SetBehaviour(new FollowingPheromones(ant));
    }

    public override void SetBehaviour(Behaviour behaviour)
    {
        scanState = AntScanState.NoScan;

        base.SetBehaviour(behaviour);
    }

    protected void HandleColonyBehaviour(Vector2Int position)
    {
        GridTile tile = MapGenerator.Instance.TileAtAssumeInsideMap(position);
        Colony colony = tile.GetColony();
        
        if (colony.CivIndex == ant.CivIndex)
        {
            if (ScanState == AntScanState.ScanForPathPheromone || ant.antBody.WantsToRefillAtBase())
            {
                SetBehaviour(new FoundColony(tile, ant));
            }
            else
            {
                ant.antMovement.RotateToRandomBackDir();
            }
        }
        else
        {
            SetBehaviour(new AttackColony(tile, ant));
        }
    }

    public virtual void HandleScanResults(SensorResultType resultType, Vector2Int position)
    {
        //ant.ss = resultType.ToString() + " " + position;
        switch (resultType)
        {
            case SensorResultType.Pheromone:
                ant.antMovement.SetTarget(MapGenerator.Instance.RealPosition(position));
                break;
            case SensorResultType.Food:
                SetBehaviour(new FoundFood(MapGenerator.Instance.TileAtAssumeInsideMap(position), ant));
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
