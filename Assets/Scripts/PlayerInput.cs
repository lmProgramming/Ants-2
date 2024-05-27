using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerInput : MonoBehaviour
{
    private static PlayerInput Instance;

    //[SerializeField]
    //Joystick movementJoystick;

    private Vector2 previousDirection = Vector2.zero;

    private void Awake()
    {
        Instance = this;
    }

    public static Vector2 DesiredPosition
    {
        get
        {
            Vector2 joystickDirection = Vector2.zero; //Instance.movementJoystick.Direction;

            if (joystickDirection.x == 0 && joystickDirection.y == 0)
            {
                joystickDirection = Instance.previousDirection;
            }

            Instance.previousDirection = joystickDirection;

            return joystickDirection;
        }
    }

    public static void SetCurrentDirection(Vector2 direction)
    {
        Instance.previousDirection = direction;
    }

    public static float Speed { get => DesiredPosition.magnitude; }
}
