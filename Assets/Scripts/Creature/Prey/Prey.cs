using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Creature
{
    public static int PREY_CIV_INDEX = -3;
    public Vector2 healthRandomMultiplierRange;

    public override int CivIndex => PREY_CIV_INDEX;

    private void Start()
    {
        body.maxHealth *= Random.Range(healthRandomMultiplierRange.x, healthRandomMultiplierRange.y);
        body.Heal();
    }
}
