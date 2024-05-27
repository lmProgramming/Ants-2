using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public enum CreatureType
    {
        Ant,
        Predator,
        Prey
    }

    public CreatureType type;
    public CreatureManager.CreatureType creatureType;

    public Body body;
    public Movement movement;
    public Brain brain;
    public Health health;

    protected bool dead = false;

    public bool IsDead { get { return dead; } }

    public Creature enemy;

    public List<int> enemyCivIndexes = new();

    public virtual int CivIndex { get; }

    public Vector2 Position
    {
        get => movement.Position;
        set => movement.Position = value;
    }

    public virtual void Die()
    {
        if (!dead)
        {
            dead = true;

            body.DropFoodOnDeath();

            CreatureManager.Instance.RemoveCreatureFromList(this);
            Destroy(gameObject);
        }
    }

    public void UpdateCreature()
    {
        body.UpdateCreature();
        brain.UpdateCreature();
    }
}
