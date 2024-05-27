using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnt : Behaviour
{
    private Ant ant;

    public override State GetState()
    {
        return State.Wandering;
    }

    public PlayerAnt(Ant ant)
    {
        this.ant = ant;
    }

    private float previousWanderingStrength;

    public override void Start()
    {
        previousWanderingStrength = ant.movement.wanderStrength;

        ant.movement.wanderStrength = 0.01f;

        ant.antMovement.UpdateIAntsJobWanderingStrength();

        PlayerInput.SetCurrentDirection(ant.movement.desiredDirection.normalized);

        ant.antBrain.ScanState = AntScanState.NoScan;
    }

    public override void Update()
    {
        UpdateAntMovement();

        if (ant.antBody.HasAnyFood)
        {
            SearchingForColony();
        }
        else
        {
            SearchingForFood();
        }
    }

    private void SearchingForColony()
    {
        Colony closestColony = ant.Civilization.GetClosestColony(ant.Position);

        if (closestColony != null)
        {
            if (Vector2.Distance(ant.Position, closestColony.position) < closestColony.radius)
            {
                VisitColony(closestColony);
            }
        }
    }

    private void SearchingForFood()
    {
        if (ant.body.TileAt.foodAmount > 0)
        {
            ant.antBody.GetFood(ant.body.TileAt);
        }
    }

    public void VisitColony(Colony targetColony)
    {
        ant.antBody.DepositFood(targetColony);

        ant.antBody.visitedColonyAction.Invoke();

        targetColony.AddTripSafety(ant.antBody.safeTrip);

        ant.antBody.ResetTripSafety();
    }

    private void UpdateAntMovement()
    {
        ant.movement.SetTarget(ant.Position + PlayerInput.DesiredPosition);

        ant.antMovement.UpdateIAntsJobSpeed(PlayerInput.Speed * ant.antMovement.maxSpeed);
    }

    public override void Stop()
    {
        ant.movement.wanderStrength = previousWanderingStrength;

        ant.antMovement.UpdateIAntsJobWanderingStrength();
        ant.antMovement.UpdateIAntsJobSpeed(ant.antMovement.maxSpeed);
    }
}
