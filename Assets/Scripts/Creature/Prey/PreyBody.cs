using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBody : Body
{
    [SerializeField]
    public float foodAmountForReproduction = 2000;

    public override void GetFood(float foodGatheredAmount)
    {
        foodAmount += foodGatheredAmount;

        CheckToSpawnChild();
    }

    private void CheckToSpawnChild()
    {
        if (foodAmount > foodAmountForReproduction)
        {
            foodAmount -= foodAmountForReproduction;
            CreatureManager.Instance.SpawnCreature(creature.creatureType, Position, true);
        }
    }
}
