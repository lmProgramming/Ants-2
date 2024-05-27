using UnityEngine;
using UnityEngine.EventSystems;

public sealed class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    public static float pressingTime;

    public static float timeSinceLastLeftClick = 100f;

    public float maxTimeBetweenDoubleClicks = 0.5f;

    public static bool leftDoubleClick = false;

    public static bool pressingAfterLeftDoubleClick = false;

    public static float deltaTime;
    public static float unscaledDeltaTime;
    public static float simDeltaTime;
    public static float simSpeed = 1;

    public static int amountOfHeldUIElements = 0;

    private void Awake()
    {
        Instance = this;
        simSpeed = 1f;
    }

    public static void StartHoldingUIElement()
    {
        amountOfHeldUIElements++;
    }

    public static void StopHoldingUIElement()
    {
        amountOfHeldUIElements--;
    }

    private void Update()
    {
        deltaTime = Time.deltaTime;
        simDeltaTime = deltaTime * simSpeed;
        unscaledDeltaTime = Time.unscaledDeltaTime;

        leftDoubleClick = false;

        if (JustClicked)
        {
            if (timeSinceLastLeftClick < maxTimeBetweenDoubleClicks)
            {
                leftDoubleClick = true;
                pressingAfterLeftDoubleClick = true;

                // after double click, we don't want our next click to also be double click
                timeSinceLastLeftClick = maxTimeBetweenDoubleClicks;
            }
            else
            {
                timeSinceLastLeftClick = 0f;
            }
        }
        else
        {
            if (JustStoppedClicking)
            {
                pressingAfterLeftDoubleClick = false;
            }

            if (Pressing)
            {
                pressingTime += unscaledDeltaTime;
            }
            else
            {
                pressingTime = 0f;
            }

            timeSinceLastLeftClick += unscaledDeltaTime;
        }
    }

    public static bool JustClicked
    {
        get
        {
            return Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began);
        }
    }

    public static bool JustClickedOutsideUI
    {
        get
        {
            return (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)) && !IsPointerOverUI;
        }
    }

    // shouldn't be used
    public static bool JustRightClicked
    {
        get
        {
            return Input.GetMouseButtonDown(1) || (Input.touchCount == 2 && Input.GetTouch(1).phase == TouchPhase.Began) && false;
        }
    }

    public static bool JustStoppedClicking
    {
        get
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    return true;
                }
            }
            return Input.GetMouseButtonUp(0);
        }
    }

    public static bool JustStoppedClickingOutsideUI
    {
        get
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Ended && !IsPointerOverUI)
                {
                    return true;
                }
            }
            return Input.GetMouseButtonUp(0) && !IsPointerOverUI;
        }
    }

    public static bool Pressing
    {
        get
        {
            return Input.GetMouseButton(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase != TouchPhase.Ended) || (Input.touchCount > 1);
        }
    }

    public static GameObject ObjectUnderPointer
    {
        get
        {
            Vector2 pointerPos = ViewportPointerPosition;

            Ray ray = Camera.main.ViewportPointToRay(pointerPos);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit)
            {
                return hit.transform.gameObject;
            }

            return null;
        }
    }

    public static Vector2 GetWorldPointerPosition(int pointerNumber = 0)
    {
        Vector2 pointerPos = Vector2.zero;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount >= 1)
            {
                pointerPos = Input.GetTouch(pointerNumber).position;
            }
        }
        else
        {
            pointerPos = Input.mousePosition;
        }

        pointerPos = Camera.main.ScreenToWorldPoint(pointerPos);

        return pointerPos;
    }

    // warning: returns positive infinity for no touches 
    public static Vector2 WorldPointerPosition
    {
        get
        {
            return GetWorldPointerPosition();
        }
    }

    // warning: returns positive infinity for no touches
    public static Vector2 ScreenPointerPosition
    {
        get
        {
            Vector2 pointerPos = Vector2.positiveInfinity;
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount >= 1)
                {
                    pointerPos = Input.GetTouch(0).position;
                }
            }
            else
            {
                pointerPos = Input.mousePosition;
            }

            return pointerPos;
        }
    }

    // warning: returns positive infinity for no touches
    public static Vector2 CenteredScreenPointerPosition
    {
        get
        {
            Vector2 pointerPos = ScreenPointerPosition;

            return pointerPos - new Vector2(Screen.width, Screen.height);
        }
    }

    //public static bool PointerInsideMap
    //{
    //    get
    //    {
    //        Vector2 pointerPos = WorldPointerPosition;

    //        Vector2Int tilePos = Cord.ToVecInt(pointerPos);

    //        return Tiles.PosInsideMap(tilePos);
    //    }
    //}

    public static Vector2Int TilePosFromPointer
    {
        get
        {
            Vector2 pointerPos = WorldPointerPosition;

            Vector2Int tilePos = Cord.ToVecInt(pointerPos);

            return tilePos;
        }
    }

    public static int TileXFromPointer
    {
        get
        {
            Vector2 pointerPos = WorldPointerPosition;

            Vector2Int tilePos = Cord.ToVecInt(pointerPos);

            return tilePos.x;
        }
    }

    public static int TileYFromPointer
    {
        get
        {
            Vector2 pointerPos = WorldPointerPosition;

            Vector2Int tilePos = Cord.ToVecInt(pointerPos);

            return tilePos.y;
        }
    }

    // if tiles are in 1D array
    //public static int TileIndexFromPointer
    //{
    //    get
    //    {
    //        Vector3 pointerPos = WorldPointerPosition;

    //        Debug.Log(pointerPos);

    //        int indexOfTileUnderPointer = Cord.TileIndexFromPos(pointerPos);

    //        return indexOfTileUnderPointer;
    //    }
    //}

#if UNITY_EDITOR
    public static float TouchesAndPointersCount
    {
        get
        {
            return Input.GetMouseButton(0) ? 1 : 0;
        }
    }
#else
    public static float TouchesAndPointersCount
    {
        get
        {
            return Input.touchCount;
        }
    }
#endif

    // warning: returns positive infinity for more touches than one or none
    public static Vector2 ViewportPointerPosition
    {
        get
        {
            Vector2 pointerPos = Vector2.positiveInfinity;
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 1)
                {
                    pointerPos = Input.GetTouch(0).position;
                }
            }
            else
            {
                pointerPos = Input.mousePosition;
            }

            pointerPos = Camera.main.ScreenToViewportPoint(pointerPos);

            return pointerPos;
        }
    }

    public static bool IsPointerOverUI
    {
        get
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }

            if (Input.touchCount > 0)
            {
                int id = Input.touches[0].fingerId;
                if (EventSystem.current.IsPointerOverGameObject(id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
