using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryToFlee : Behaviour
{
    private Creature enemy;

    public override State GetState()
    {
        return State.TryToFlee;
    }

    public TryToFlee(Creature enemy, Creature thisCreature)
    {
        this.enemy = enemy;
        creature = thisCreature;
    }

    public override void Start()
    {

    }

    public override void Update()
    {
        creature.enemy = enemy;
        if (enemy != null)
        {
            creature.movement.SetTargetOppositeTo(enemy.Position);

            if (Vector2.Distance(enemy.Position, creature.Position) < 1f)
            {
                creature.brain.SetBehaviour(new Fighting(enemy, creature, creature.Position));
            }
        }
        else
        {
            enemy = Fighting.FindNewEnemy(creature, 0.5f);
            if (enemy is not null)
            {
                creature.brain.SetBehaviour(new TryToFlee(enemy, creature));
            }
            else
            {
                creature.brain.SetDefaultBehaviour();
            }
        }
    }

    public override void Stop()
    {

    }
}
