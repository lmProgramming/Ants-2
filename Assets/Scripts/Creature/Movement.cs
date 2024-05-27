using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Creature creature;

    // max speed
    public float maxSpeed = 2;
    // how quickly the ant changes direction
    public float steerStrength = 2;
    // applied randomly to the current direction, changing it
    public float wanderStrength = 0.1f;

    // direction the ant will steer towards
    [HideInInspector]
    public Vector2 desiredDirection;

    public Vector2 Position { get; set; }

    // don't change, derived from desired diretion
    public Vector2 Velocity { get; protected set; }

    public bool Frozen { get; protected set; } = false;

    public virtual void SetFrozen(bool frozen)
    {
        Frozen = frozen;
    }

    public void MutateTraits(float mutationStrength)
    {
        maxSpeed *= 1 + (Random.value - 0.5f) * mutationStrength;
        steerStrength *= 1 + (Random.value - 0.5f) * mutationStrength;
        wanderStrength *= 1 + (Random.value - 0.5f) * mutationStrength;
    }

    public void SetTargetInFront() => SetTarget(creature.Position + new Vector2(transform.right.x, transform.right.y));

    public virtual void SetTarget(Vector2 targetPosition)
    {
        desiredDirection = (targetPosition - Position).normalized;
    }

    public virtual void SetTargetImmediate(Vector2 targetPosition)
    {
        desiredDirection = (targetPosition - Position).normalized;

        Velocity = Vector2.zero;
        UpdatePosition(true);
    }

    public virtual void SetTargetOppositeTo(Vector2 targetPosition)
    {
        desiredDirection = (Position - targetPosition).normalized;
    }
    
    public float GetAngle()
    {
        return transform.rotation.eulerAngles.z;
    }

    public void RotateToBackSlow()
    {
        desiredDirection = -transform.right;
        UpdatePosition(true);
    }

    public void RotateToBack()
    {
        Velocity = Vector2.zero;
        desiredDirection = -transform.right;

        UpdatePosition(true);
    }

    public void RotateToRandomBackDir()
    {
        Velocity = Vector2.zero;
        float randomAngle = Random.Range(150f, 210f);
        Quaternion rotation = Quaternion.Euler(0f, 0f, randomAngle);
        desiredDirection = rotation * transform.right;

        UpdatePosition(true);
    }

    public virtual void UpdatePosition(bool fullForce)
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
        }
        else
        {

        }
    }

    public void UpdatePositionAndRotation(Vector2 position, Quaternion rotation)
    {
        Position = position;
        transform.localPosition = position;
        transform.localRotation = rotation;
    }

    public void UpdateTransformPosition() => transform.localPosition = Position;
    public void UpdatePositionFromTransform() => Position = transform.localPosition;
}
