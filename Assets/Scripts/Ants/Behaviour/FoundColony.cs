using UnityEngine;
using UnityEngine.UIElements;

public class FoundColony : Behaviour
{
    private Colony targetColony;
    private Ant ant;
    private Vector2 targetColonyPosition;
    private float targetColonyRadius;

    public FoundColony(GridTile targetTile, Ant ant)
    {
        targetColony = targetTile.GetColony();
        this.ant = ant;
        targetColonyPosition = targetColony.position;
        targetColonyRadius = targetColony.radius;

        // invoked if going to colony and it was destroyed
        targetColony.Destroyed.AddListener(ColonyDestroyed);
    }

    override public State GetState()
    {
        return State.FoundColony;
    }

    public override void Start()
    {
        ant.antBody.SetFullForce(true);

        ant.antMovement.SetTarget(targetColonyPosition);
    }

    override public void Update()
    {
        ant.antMovement.SetTarget(targetColonyPosition);

        if (Vector2.Distance(ant.Position, targetColonyPosition) < targetColonyRadius)
        {
            VisitColony();
        }
    }

    public void VisitColony()
    {
        if (targetColony != null)
        {
            ant.antBody.DepositFood(targetColony);

            ant.antBody.visitedColonyAction.Invoke();

            targetColony.AddTripSafety(ant.antBody.safeTrip);

            ant.antBody.ResetTripSafety();
        }

        ant.antBrain.SetDefaultBehaviour();
    }

    public void ColonyDestroyed()
    {
        ant.antBrain.SetDefaultBehaviour();
    }

    public override void Stop()
    {

    }
}
