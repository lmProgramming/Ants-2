using System.Collections.Generic;
using UnityEngine;

public class Ant : Creature
{
    [HideInInspector]
    public AntMovement antMovement;
    [HideInInspector]
    public AntBrain antBrain;
    [HideInInspector]
    public AntBody antBody;

    public int UpdateJobIndex { get; set; }
    public Civilization Civilization { get; set; }
    public override int CivIndex => Civilization.civIndex;

    public enum AntType
    {
        Worker,
        Soldier,
        Queen
    }

    //TODO REMOVE LATER
    //public Behaviour.State s;
    //public string ss;

    private void Awake()
    {
        antMovement = (AntMovement) movement;
        antBrain = (AntBrain) brain;
        antBody = (AntBody) body;

        type = CreatureType.Ant;
    }

    public void UpdateAnt()
    {
        antBody.UpdateCreature();
        antBrain.UpdateCreature();

        //s = antBrain.GetBehaviourState();
        //ss = s.ToString();
    }

    public override void Die()
    {
        if (!dead)
        {
            dead = true;

            antBody.DropFoodOnDeath();

            Civilization.RemoveAnt(this);
            IAntUpdateJobsManager.Instance.RemoveAntFromList(this);
            Destroy(gameObject);
        }
    }
}

