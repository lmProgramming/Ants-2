using UnityEngine;

public class GoingTowardsEnemy : Behaviour
{
    private Creature enemy;

    private bool calledToBattle;

    private Vector2 positionBeforeFighting;

    private float timeLeftPursuing;
    private float defaultPursuingTime = 20;

    public GoingTowardsEnemy(Creature enemyAnt, Creature thisAnt, bool calledToBattle)
    {
        this.enemy = enemyAnt;

        creature = thisAnt;

        this.calledToBattle = calledToBattle;

        timeLeftPursuing = defaultPursuingTime;
    }

    public override State GetState()
    {
        return State.GoingTowardsEnemy;
    }

    public override void Start()
    {
        if (!calledToBattle)
        {
            if (creature.type == Creature.CreatureType.Ant)
            {
                Fighting.CallReinforcements((Ant) creature, enemy);
            }
            Fighting.NotifyOpponent(creature, enemy);
        }

        positionBeforeFighting = creature.Position;
    }

    public override void Update()
    {
        creature.enemy = enemy;
        if (enemy != null)
        {
            creature.movement.SetTarget(enemy.Position);

            if (Vector2.Distance(enemy.Position, creature.Position) < 1f)
            {
                creature.brain.SetBehaviour(new Fighting(enemy, creature, positionBeforeFighting));
            }
        }
        else
        {
            enemy = Fighting.FindNewEnemy(creature);
            if (enemy is not null)
            {
                creature.brain.SetBehaviour(new GoingTowardsEnemy(enemy, creature, true));
            }
            else
            {
                creature.brain.SetBehaviour(new GoToPoint(positionBeforeFighting, creature, 2f));
            }
        }

        timeLeftPursuing -= GameInput.simDeltaTime;
        if (timeLeftPursuing < 0)
        {
            creature.brain.SetBehaviour(new GoToPoint(positionBeforeFighting, creature, 2f));
        }
    }

    public override void Stop()
    {

    }
}
