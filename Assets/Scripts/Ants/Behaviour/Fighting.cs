using System.Collections.Generic;
using UnityEngine;

public class Fighting : Behaviour
{
    private Creature enemy;

    private Vector2 positionBeforeFighting;

    public Fighting(Creature enemy, Creature creature, Vector2 positionBeforeFighting)
    {
        this.enemy = enemy;

        this.creature = creature;
        this.positionBeforeFighting = positionBeforeFighting;

        this.creature.body.StartedFighting();

        this.creature.body.hitAWallAction += GoBack;
    }

    public override void Start()
    {
        creature.movement.SetFrozen(true);

        AttackOpponent(creature, enemy);
    }

    public override State GetState()
    {
        return State.Fighting;
    }

    public override void Update()
    {
        if (enemy != null)
        {
            if (Vector2.Distance(creature.Position, enemy.Position) > 2f)
            {
                for (int i = 0; i < creature.body.enemiesFightingThisCreature.Count; i++)
                {
                    if (creature.body.enemiesFightingThisCreature[i] = enemy)
                    {
                        creature.body.enemiesFightingThisCreature.RemoveAt(i);
                        break;
                    }
                }

                enemy = null;
            }
            else if (creature.body.NextDamageDealTime < Time.time)
            {
                creature.body.ResetDamageCooldown();

                bool killedEnemy = enemy.body.GetDamaged(Random.value * creature.body.Damage);

                if (killedEnemy)
                {
                    float foodGatheredFromCorpse = MapGenerator.Instance.GatherFoodAroundTile(enemy.body.TileAt, creature.body.foodLeftAfterDeathRange.y, creature.CivIndex);
                    creature.body.GetFood(foodGatheredFromCorpse);
                }

                return;
            }
        }

        for (int i = 0; i < creature.body.enemiesFightingThisCreature.Count; i++)
        {
            if (creature.body.enemiesFightingThisCreature[i] != null)
            {
                enemy = creature.body.enemiesFightingThisCreature[i];
            }
            else
            {
                creature.body.enemiesFightingThisCreature.RemoveAt(i);
                i--;
            }
        }

        if (enemy == null)
        {
            enemy = FindNewEnemy(creature);
            if (enemy != null)
            {
                creature.brain.SetBehaviour(new GoingTowardsEnemy(enemy, creature, true));
            }
            else
            {
                GoBack();
            }
        }
    }

    public void GoBack()
    {
        creature.brain.SetBehaviour(new GoToPoint(positionBeforeFighting, creature, 2f));
    }

    public static Ant FindNewEnemy(Creature ant, float radiusMultiplier = 2)
    {
        // ants will look for enemies in smaller radius if low on health
        Ant[] antsAround = AntsManager.Instance.FastAntsInCircleExcludingCivIndex(ant.Position, ant.body.health * radiusMultiplier, ant.CivIndex);

        if (antsAround.Length > 0)
        {
            return MathExt.RandomFrom(antsAround);
        }

        return null;
    }

    public static void CallReinforcements(Ant thisAnt, Creature enemyAnt)
    {
        Ant[] antsAround = AntsManager.Instance.FastAntsInCircle(thisAnt.Position, 8, thisAnt.CivIndex);

        int antsLeftToCall = 5;
        for (int i = 0; i < antsAround.Length; i++)
        {
            if (antsAround[i] != thisAnt && antsAround[i].antBrain.GetBehaviourState() != State.Fighting && antsAround[i].antBrain.GetBehaviourState() != State.GoingTowardsEnemy)
            {
                antsAround[i].antBrain.SetBehaviour(new GoingTowardsEnemy(enemyAnt, antsAround[i], true));
                antsLeftToCall--;
                if (antsLeftToCall == 0)
                {
                    break;
                }
            }
        }
    }

    public static void NotifyOpponent(Creature thisAnt, Creature enemyAnt)
    {
        enemyAnt.brain.NoticedByEnemy(thisAnt);
    }

    public static void AttackOpponent(Creature thisAnt, Creature enemyAnt)
    {
        enemyAnt.body.AddEnemyFightingThisAnt(thisAnt);

        if (enemyAnt.brain.GetBehaviourState() != State.Fighting)
        {
            enemyAnt.brain.SetBehaviour(new Fighting(thisAnt, enemyAnt, enemyAnt.Position));
        }
    }

    public override void Stop()
    {

    }
}
