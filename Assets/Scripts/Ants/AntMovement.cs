using UnityEngine;

public class AntMovement : Movement
{
    private Ant ant;

    private void Awake()
    {
        ant = GetComponent<Ant>();
    }

    public override void SetFrozen(bool frozen)
    {
        Frozen = frozen;

        IAntUpdateJobsManager.Instance._fullForceArrayA[ant.UpdateJobIndex] = frozen ? 0 : ant.antBody.FullForce ? 2 : 1;
    }    

    public override void SetTarget(Vector2 targetPosition)
    {
        desiredDirection = (targetPosition - Position).normalized;
        IAntUpdateJobsManager.Instance.UpdateAntDesiredDirection(ant, desiredDirection);
    }

    public void SetTargetWithoutUpdatingIAntUpdater(Vector2 targetPosition)
    {
        desiredDirection = (targetPosition - Position).normalized;
    }

    public override void UpdatePosition(bool fullForce)
    {
        if (!Frozen)
        {
            Vector2 desiredVelocity;
            Vector2 desiredSteeringForce;

            if (fullForce)
            {
                desiredVelocity = desiredDirection * maxSpeed;
                desiredSteeringForce = 1000 * steerStrength * (desiredVelocity - Velocity);
            }
            else
            {
                desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

                desiredVelocity = desiredDirection * maxSpeed;
                desiredSteeringForce = steerStrength * (desiredVelocity - Velocity);
            }

            Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength);

            Velocity = Vector2.ClampMagnitude(Velocity + acceleration * GameInput.simDeltaTime, maxSpeed);
            Position += Velocity * GameInput.simDeltaTime;

            float zAngle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
            UpdatePositionAndRotation(Position, Quaternion.Euler(0, 0, zAngle));

            UpdateIAntsJobMovement();
        }
    }

    public void UpdateIAntsJobMovement()
    {
        IAntUpdateJobsManager.Instance.UpdateAntMovement(ant);        
    }

    public void UpdateIAntsJobWanderingStrength()
    {
        IAntUpdateJobsManager.Instance.UpdateAntWanderingStrength(ant);
    }

    public void UpdateIAntsJobSpeed(float speed)
    {
        IAntUpdateJobsManager.Instance.UpdateAntSpeed(ant, speed);
    }
}
