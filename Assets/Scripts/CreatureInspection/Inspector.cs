using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : MonoBehaviour
{
    [SerializeField] private bool isOn = false;

    private void Update()
    {
        if (isOn)
        {
            if (GameInput.amountOfHeldUIElements == 0 && GameInput.JustClickedOutsideUI)
            {
                Creature creatureClosestToPointer = SelectClickedCreature();
                if (creatureClosestToPointer != null)
                {
                    CameraManager.Instance.StartFollowingObjectIfNotFollowingItAlready(creatureClosestToPointer.gameObject);
                    creatureClosestToPointer.brain.SetBehaviour(new PlayerAnt((Ant)creatureClosestToPointer));
                }
                else
                {
                    CameraManager.Instance.StopFollowingObject();
                }
            }
        }
    }

    private Creature SelectClickedCreature()
    {
        //if (CameraManager.Instance.Zoom > 32)
        //{
        //    return null;
        //}

        Creature creatureClosestToPointer = CreatureManager.Instance.CreatureClosestToPosition(GameInput.WorldPointerPosition, 3);

        return creatureClosestToPointer;
    }
}
