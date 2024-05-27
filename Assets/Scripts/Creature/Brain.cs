using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public Creature creature;

    private Behaviour behaviour;

    public enum ImportanceOfTask
    {
        Critical,
        Important,
        Low
    }

    protected virtual void Awake()
    {
        // not really used, get's overwritten almost immedietally by followingPheromones (cant use immedietally cuz jobs)
        behaviour = new Wandering(0.1f, creature);
    }

    public ImportanceOfTask GetImportanceOfTask()
    {
        Behaviour.State antBehaviourState = GetBehaviourState();
        switch (antBehaviourState)
        {
            case Behaviour.State.Fighting:
                return ImportanceOfTask.Critical;
            case Behaviour.State.FoundFood:
            case Behaviour.State.FoundColony:
            case Behaviour.State.GoingTowardsEnemy:
            case Behaviour.State.AttackColony:
                return ImportanceOfTask.Important;
            default:
                return ImportanceOfTask.Low;
        }
    }

    protected virtual void Start()
    {
        creature.movement.SetTargetInFront();

        creature.body.hitAWallAction += OnHitWall;
    }

    private void OnHitWall() => SetDefaultBehaviour();

    public void UpdateCreature()
    {
        behaviour.Update();
    }

    public virtual void SetDefaultBehaviour()
    {
        SetBehaviour(new Wandering(-1, creature));
    }

    public virtual void SetBehaviour(Behaviour newBehaviour)
    {
        behaviour.Stop();

        behaviour = newBehaviour;

        creature.movement.SetFrozen(false);
        creature.body.SetFullForce(false);

        newBehaviour.Start();
    }

    public virtual void NoticedByEnemy(Creature enemy)
    {
        if (GetBehaviourState() != Behaviour.State.Fighting && GetBehaviourState() != Behaviour.State.GoingTowardsEnemy)
        {
            SetBehaviour(new GoingTowardsEnemy(enemy, creature, false));
        }
    }

    public Behaviour.State GetBehaviourState() => behaviour.GetState();

    internal void DebugBehaviour()
    {
        behaviour.DebugBehaviour();
    }
}
