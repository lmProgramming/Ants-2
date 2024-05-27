using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AntBody : Body
{
    [HideInInspector]
    public AntMovement antMovement;
    public PheromonePlacer placer;
    [HideInInspector]
    public Ant ant;

    [SerializeField] private AnimationCurve cargoDropOff;
    [SerializeField] private SpriteRenderer carriedFoodSpriteRenderer;

    public UnityAction visitedColonyAction;

    private float timeFoodPickedUp;
    private float amountOfFoodPickedUp;

    public float taintByDepletedFoodPheromoneChance = 0.05f;

    [HideInInspector]
    public bool safeTrip = true;

    private void Awake()
    {
        antMovement = GetComponent<AntMovement>();
        ant = GetComponent<Ant>();
    }

    private new void Start()
    {
        base.Start();

        visitedColonyAction += JustVisitedColony;
    }

    public override void SetFullForce(bool fullForce)
    {
        IAntUpdateJobsManager.Instance._fullForceArrayA[ant.UpdateJobIndex] = ant.antMovement.Frozen ? 0 : fullForce ? 2 : 1;
    }

    public void JustVisitedColony()
    {
        placer.taintedByDepletedFoodPheromone = false;

        Heal();
    }

    //protected override bool CheckIfOnNewTile()
    //{
    //    GridTile newTile = MapGenerator.Instance.TileAtAssumeInsideMap(Position);
    //    if (newTile != TileAt)
    //    {
    //        TileAt = newTile;
    //        TileAt.SetCreatureOnTile(ant);
    //        return true;
    //    }

    //    return false;
    //}

    public bool WantsToRefillAtBase()
    {
        return placer.PheromoneLeft < placer.pheromoneCapacity / 2 || health < maxHealth / 5;
    }

    public override void UpdateCreature()
    {
        if (CheckIfOnNewTile())
        {
            if (TileAt.GetTileState() == TileState.Wall)
            {
                if (hitAWallLastFrame)
                {
                    ant.Die();
                }

                SnapToLastPosition();

                antMovement.RotateToRandomBackDir();

                hitAWallAction.Invoke();

                hitAWallLastFrame = true;

                return;
            }
            else
            {
                hitAWallLastFrame = false;

                if (TileAt.PheromoneAmount(ant.CivIndex, GridTile.DEPLETEDFOOD_PHEROMONE) > 0)
                {
                    if (HasAnyFood)
                    {
                        TileAt.RegressDepletedFoodPheromone(ant.CivIndex);
                    }
                    else if (Random.value < taintByDepletedFoodPheromoneChance)
                    {
                        placer.taintedByDepletedFoodPheromone = true;
                    }
                }

                lastPosition = Position;
            }

            if (foodAmount > 0)
                foodAmount = cargoDropOff.Evaluate((Time.time - timeFoodPickedUp) / 150) * amountOfFoodPickedUp;
            if (foodAmount <= 0)
                carriedFoodSpriteRenderer.enabled = false;

            placer.UpdatePheromone();
        }
    }

    public override void GetFood(float foodGatheredAmount)
    {
        foodAmount = foodGatheredAmount;
        amountOfFoodPickedUp = foodGatheredAmount;

        timeFoodPickedUp = Time.time;
        carriedFoodSpriteRenderer.enabled = true;
    }

    public void GetMoreFood(float additionalFood)
    {
        GetFood(foodAmount + additionalFood);
    }

    public bool CargoFull()
    {
        return RemainingCargoCapacity() < 1;
    }

    public float RemainingCargoCapacity()
    {
        return cargoCapacity - foodAmount;
    }

    public override void DropFoodOnDeath()
    {
        base.DropFoodOnDeath();

        if (foodAmount > 0)
        {
            TileAt.AddOrChangeToFoodIfFoodable(foodAmount * 2);
        }
    }

    public void DepositFood(Colony colony)
    {
        colony.DepositFood(foodAmount);

        carriedFoodSpriteRenderer.enabled = false;
        foodAmount = 0;

        antMovement.RotateToBack();
    }

    public void StealFood(Colony colony)
    {
        foodAmount = colony.StealFood(ant.Civilization, cargoCapacity);

        GetFood(foodAmount);

        carriedFoodSpriteRenderer.enabled = foodAmount > 0;

        antMovement.RotateToBack();
    }

    public GridTile ScanForFoodAroundTile(GridTile tileScanned)
    {
        GridTile[] tilesArroundFormerFoodTarget = MapGenerator.Instance.XTilesInRectFromLeftUpperCornerGridPosAssumeInside(MapGenerator.GridPos(tileScanned.position) - new Vector2Int(2, 2), 5);
        for (int i = 0; i < tilesArroundFormerFoodTarget.Length; i++)
        {
            if (tilesArroundFormerFoodTarget[i].GetTileState() == TileState.Food)
            {
                return tilesArroundFormerFoodTarget[i];
            }
        }
        return null;
    }

    public override void StartedFighting()
    {
        base.StartedFighting();

        safeTrip = false;
    }

    public void ResetTripSafety()
    {
        safeTrip = true;
    }

    public void UpdateIAntsJob()
    {
        IAntUpdateJobsManager.Instance.UpdateValuesFromAnt(ant);
    }
}
