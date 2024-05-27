using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreyBrain : Brain
{
    public override void SetDefaultBehaviour()
    {
        SetBehaviour(new Grazing(creature));
    }

    public override void NoticedByEnemy(Creature enemy)
    {
        SetBehaviour(new TryToFlee(enemy, creature));
    }
}
