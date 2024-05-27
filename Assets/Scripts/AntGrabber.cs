using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntGrabber : MonoBehaviour
{
    // ABANDONED IDEA









    public float minDistance = 2f;

    public Ant antGrabbed = null;

    private void Update()
    {
        if (GameInput.JustClickedOutsideUI)
        {
            antGrabbed = GrabAnt(GameInput.WorldPointerPosition);
        }

        if (GameInput.Pressing && antGrabbed)
        {
            if (antGrabbed)
            {
                //antGrabbed.movement.Po
            }
        }
        else
        {
            antGrabbed = null;
        }
    }

    public Ant GrabAnt(Vector2 pos)
    {
        Ant ant = null;

        float lowest = minDistance;

        for (int i = 0; i < AntsManager.Instance.existingAnts.Count; i++)
        {
            float dist = Vector2.Distance(AntsManager.Instance.existingAnts[i].Position, pos);
            if (dist < lowest)
            {
                lowest = dist;
                ant = AntsManager.Instance.existingAnts[i];
            }
        }

        return ant;
    }
}
