using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColony : Behaviour
{
    private Ant ant;

    private Colony targetColony;
    private Vector2 targetColonyPosition;
    private float targetColonyRadius;

    public override State GetState()
    {
        return State.AttackColony;
    }

    public AttackColony(GridTile targetTile, Ant ant)
    {
        this.ant = ant;

        Ant enemyAntNearby = CheckIfEnemiesNearby(targetTile);

        if (enemyAntNearby != null)
        {
            ant.antBrain.SetBehaviour(new GoingTowardsEnemy(enemyAntNearby, ant, false));
            return;
        }

        targetColony = targetTile.GetColony();

        if (targetColony == null ) 
        {
            ant.brain.SetDefaultBehaviour();    
        }

        targetColonyPosition = targetColony.position;
        targetColonyRadius = targetColony.radius;

        // invoked if going to colony and it was destroyed
        targetColony.Destroyed.AddListener(ColonyDestroyed);

        // here instead of start, because this behaviour can be overriden
        ant.antBody.SetFullForce(true);

        ant.antMovement.SetTarget(targetColonyPosition);
    }

    private Ant CheckIfEnemiesNearby(GridTile targetTile)
    {
        Ant[] antsAroundColony = AntsManager.Instance.FastAntsInCircleExcludingCivIndex(targetTile.position, 5, ant.CivIndex);
        if (antsAroundColony.Length > 0)
        {
            return antsAroundColony[Random.Range(0, antsAroundColony.Length)];
        }
        return null;
    }

    public override void Start()
    {

    }

    override public void Update()
    {
        ant.antMovement.SetTarget(targetColonyPosition);

        if (Vector2.Distance(ant.Position, targetColonyPosition) < targetColonyRadius || targetColony == null || targetColony.CivIndex == ant.CivIndex)
        {
            VisitColony();
        }
    }

    public void VisitColony()
    {
        if (targetColony != null)
        {
            ant.antBody.StealFood(targetColony);
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
