using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Predator : Creature
{
    public static int PREDATOR_CIV_INDEX = -2;
    public Vector2 healthRandomMultiplierRange;

    public override int CivIndex => PREDATOR_CIV_INDEX;

    private void Start()
    {
        body.maxHealth *= Random.Range(healthRandomMultiplierRange.x, healthRandomMultiplierRange.y);
        body.Heal();
    }
}
