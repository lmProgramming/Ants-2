using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorBrain : Brain
{
    public override void SetDefaultBehaviour()
    {
        SetBehaviour(new SearchForEnemies(creature));
    }
}
