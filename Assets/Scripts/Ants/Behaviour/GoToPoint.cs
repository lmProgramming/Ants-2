using UnityEngine;

public class GoToPoint : Behaviour
{
    private Vector2 point;

    private float timeLeft;

    private bool useTimer = false;

    public GoToPoint(Vector2 point, Creature creature, float? timer = null)
    {
        this.point = point;

        this.creature = creature;

        if (timer is not null)
        {
            timeLeft = (float)timer;
            useTimer = true;
        }

        creature.body.hitAWallAction += StopFollowingPoint;
    }

    public override State GetState()
    {
        return State.FollowingPheromones;
    }

    public override void Start()
    {
        creature.movement.SetTarget(point);
    }

    public override void Update()
    {
        creature.movement.SetTarget(point);

        if (Vector2.Distance(point, creature.Position) < 0.5f || (useTimer && (timeLeft -= GameInput.deltaTime) < 0))
        {
            StopFollowingPoint();
        }
    }

    public void StopFollowingPoint()
    {
        creature.brain.SetDefaultBehaviour();
    }

    public override void Stop()
    {

    }
}
