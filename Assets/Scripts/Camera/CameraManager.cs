using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public sealed class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    private Camera cameraMain;

    public bool followingObject = false;
    public GameObject objectToFollow;

    public bool forcedToFollowCurrentObject = false;

    public float dragSpeed = 1f;
    public float keySpeed = 1f;

    public bool fakingMobilePlatformAndTestingMobileInput;
    public Transform sq1;
    public Transform sq2;

    public bool isZooming = false;

    // generic - drag one finger to move the screen, boundaries - finger near bounds moves the screen
    public enum DraggingType
    {
        Generic,
        Boundaries,
        None
    }

    public DraggingType curDraggingType = DraggingType.Generic;

    public float screenPercentageDistanceToBoundToStartDragging = 0.1f;
    public float boundDraggingSpeed = 1;

    private float minXPos;
    private float maxXPos;
    private float minYPos;
    private float maxYPos;

    private Vector2 previousMousePosition = Vector2.zero;

    public float scrollSpeed = 1f;
    public float mobileScrollSpeed = 1f;

    public float maxZoom;
    public float minZoom;

    public bool updateCamera = true;

    public PostProcessLayer postProcessLayer;
    public PostProcessVolume postProcessVolume;

    public RenderTexture pausedMenuGameTexture;
    public RawImage backgroundBlurImage;

    public int nextFramesToRender = 0;

    [SerializeField] private AnimationCurve zoomCurve;
    [SerializeField] private AnimationCurve moveCurve;

    public float Zoom { get => cameraMain.orthographicSize; set => cameraMain.orthographicSize = value; }

    private void Awake()
    {
        cameraMain = GetComponent<Camera>();
        Instance = this;
    }

    private bool IsMobilePlatform()
    {
        return Application.isMobilePlatform || fakingMobilePlatformAndTestingMobileInput;
    }

    private void Start()
    {
        pausedMenuGameTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

        backgroundBlurImage.texture = pausedMenuGameTexture;
    }

    public bool SupportsRenderOfPausedMenuGameTexture()
    {
        return SupportsRender(pausedMenuGameTexture.depthStencilFormat, pausedMenuGameTexture.format);
    }

    public static bool SupportsRender()
    {
        return SupportsRender(GraphicsFormat.R8G8B8A8_SRGB, RenderTextureFormat.ARGB32);
    }

    public static bool SupportsRender(GraphicsFormat format, RenderTextureFormat renderTextureFormat)
    {
        return false;
        //return SystemInfo.IsFormatSupported(format, FormatUsage.Render) && SystemInfo.SupportsRenderTextureFormat(renderTextureFormat);
    }

    public void StartFollowingObject(GameObject objectToFollow)
    {
        followingObject = true;
        this.objectToFollow = objectToFollow;
    }

    public void StartFollowingObjectIfNotFollowingItAlready(GameObject objectToFollow)
    {
        if (objectToFollow == this.objectToFollow)
        {
            StopFollowingObject();
            return;
        }

        followingObject = true;
        this.objectToFollow = objectToFollow;
    }

    public void ForceStartFollowingObject(GameObject objectToFollow)
    {
        followingObject = true;
        this.objectToFollow = objectToFollow;
        forcedToFollowCurrentObject = true;
    }

    public void StopFollowingObject()
    {
        if (!forcedToFollowCurrentObject)
        {
            followingObject = false;
            objectToFollow = null;
        }
    }

    public void ForceStopFollowingObject()
    {
        followingObject = false;
        objectToFollow = null;
        forcedToFollowCurrentObject = false;
    }

    public void SetBoundaries(float minXPos, float maxXPos, float minYPos, float maxYPos)
    {
        this.minXPos = minXPos;
        this.maxXPos = maxXPos;
        this.minYPos = minYPos;
        this.maxYPos = maxYPos;

        minZoom = (maxXPos + maxYPos) / 1.5f;
    }

    public void DisableBlur()
    {
        postProcessLayer.enabled = false;
        postProcessVolume.enabled = false;
    }

    public void EnableBlur()
    {
        postProcessLayer.enabled = true;
        postProcessVolume.enabled = true;
    }

    public Camera UICamera;

    public void TurnOnCameraRenderingToTexture()
    {
        cameraMain.targetTexture = pausedMenuGameTexture;
        backgroundBlurImage.SetNativeSize();

        nextFramesToRender = 2;
    }

    public void TurnOffCameraRenderingToTexture()
    {
        cameraMain.targetTexture = null;

        EnableCamera();
    }

    public void EnableCamera()
    {
        cameraMain.enabled = true;
        UICamera.clearFlags = CameraClearFlags.Nothing;
    }

    public void EnableCameraForOneFrame()
    {
        cameraMain.enabled = true;

        nextFramesToRender = 2;
    }

    public void DisableCamera()
    {
        cameraMain.enabled = false;
        UICamera.clearFlags = CameraClearFlags.SolidColor;
    }

    private void Update()
    {
        if (ShouldUpdateCamera())
        {
            if (followingObject && objectToFollow != null)
            {
                // camera centered around some object
                transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, transform.position.z);
            }
            else
            {
                // camera free to move, can move by dragging finger or mouse
                ProcessDrag();
            }

            // zoom the camera
            ProcessZoom();

            if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            {
                previousMousePosition = Input.mousePosition;
            }
        }
    }

    private bool ShouldUpdateCamera()
    {
        return updateCamera && (followingObject || GameInput.amountOfHeldUIElements == 0);
    }

    public void LateUpdate()
    {
        if (--nextFramesToRender == 0)
        {
            DisableCamera();
        }
    }

    private void ProcessDrag()
    {
        if (curDraggingType == DraggingType.None)
        {
            return;
        }

        bool nowDown;

        //check if input is activated (mouse down or finger touching screen)
        if (IsMobilePlatform())
        {
            nowDown = Input.touchCount > 0;
        }
        else
        {
            nowDown = (Input.GetMouseButton(0) && !GameInput.IsPointerOverUI) || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
        }

        //if mouse currently holding or finger touching the screen
        if (nowDown)
        {
            Vector2 incrementDrag = Vector2.zero;

            if (curDraggingType == DraggingType.Generic)
            {
                incrementDrag = ProcessGenericDrag(incrementDrag);
            }
            else if (curDraggingType == DraggingType.Boundaries)
            {
                incrementDrag = ProcessBoundariesDrag(incrementDrag);
            }

            transform.Translate(dragSpeed * zoomCurve.Evaluate(cameraMain.orthographicSize / minZoom) * incrementDrag);

            // clamp the position
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minXPos, maxXPos), Mathf.Clamp(transform.position.y, minYPos, maxYPos), transform.position.z);
        }
    }

    private Vector2 ProcessGenericDrag(Vector2 incrementDrag)
    {
        if (IsMobilePlatform())
        {
            if (Input.touchCount > 0)
            {
                if (fakingMobilePlatformAndTestingMobileInput)
                {
                    sq1.transform.position = GameInput.GetWorldPointerPosition(0);
                }

                int viableTouches = 0;

                for (int i = 0; i < Input.touches.Length; i++)
                {
                    if (Input.GetTouch(i).phase != TouchPhase.Ended)
                    {
                        viableTouches += 1;

                        incrementDrag.x += -Input.GetTouch(i).deltaPosition.x;
                        incrementDrag.y += -Input.GetTouch(i).deltaPosition.y;
                    }
                }

                incrementDrag /= Mathf.Max(1, viableTouches);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                incrementDrag.x = previousMousePosition.x - Input.mousePosition.x;
                incrementDrag.y = previousMousePosition.y - Input.mousePosition.y;
            }

            incrementDrag.x += Input.GetAxis("Horizontal") * keySpeed;
            incrementDrag.y += Input.GetAxis("Vertical") * keySpeed;
        }

        return incrementDrag;
    }

    private Vector2 ProcessBoundariesDrag(Vector2 incrementDrag)
    {
        if (IsMobilePlatform())
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touches.Length; i++)
                {
                    if (Input.GetTouch(i).phase != TouchPhase.Ended)
                    {
                        Vector2 touchDistToLeftUpperCorner = Input.GetTouch(i).position / new Vector2(Screen.width, Screen.height);

                        if (touchDistToLeftUpperCorner.x < screenPercentageDistanceToBoundToStartDragging)
                        {
                            incrementDrag.x -= (screenPercentageDistanceToBoundToStartDragging - touchDistToLeftUpperCorner.x) * boundDraggingSpeed;
                        }
                        else if (touchDistToLeftUpperCorner.x > 1 - screenPercentageDistanceToBoundToStartDragging)
                        {
                            incrementDrag.x += (touchDistToLeftUpperCorner.x - (1 - screenPercentageDistanceToBoundToStartDragging)) * boundDraggingSpeed;
                        }

                        if (touchDistToLeftUpperCorner.y < screenPercentageDistanceToBoundToStartDragging)
                        {
                            incrementDrag.y -= (screenPercentageDistanceToBoundToStartDragging - touchDistToLeftUpperCorner.y) * boundDraggingSpeed;
                        }
                        else if (touchDistToLeftUpperCorner.y > 1 - screenPercentageDistanceToBoundToStartDragging)
                        {
                            incrementDrag.y += (touchDistToLeftUpperCorner.y - (1 - screenPercentageDistanceToBoundToStartDragging)) * boundDraggingSpeed;
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 touchDistToLeftUpperCorner = Input.mousePosition / new Vector2(Screen.width, Screen.height);

                if (touchDistToLeftUpperCorner.x < screenPercentageDistanceToBoundToStartDragging)
                {
                    incrementDrag.x -= (screenPercentageDistanceToBoundToStartDragging - touchDistToLeftUpperCorner.x) * boundDraggingSpeed;
                }
                else if (touchDistToLeftUpperCorner.x > 1 - screenPercentageDistanceToBoundToStartDragging)
                {
                    incrementDrag.x += (touchDistToLeftUpperCorner.x - (1 - screenPercentageDistanceToBoundToStartDragging)) * boundDraggingSpeed;
                }

                if (touchDistToLeftUpperCorner.y < screenPercentageDistanceToBoundToStartDragging)
                {
                    incrementDrag.y -= (screenPercentageDistanceToBoundToStartDragging - touchDistToLeftUpperCorner.y) * boundDraggingSpeed;
                }
                else if (touchDistToLeftUpperCorner.y > 1 - screenPercentageDistanceToBoundToStartDragging)
                {
                    incrementDrag.y += (touchDistToLeftUpperCorner.y - (1 - screenPercentageDistanceToBoundToStartDragging)) * boundDraggingSpeed;
                }
            }

            incrementDrag.x += Input.GetAxis("Horizontal") * keySpeed;
            incrementDrag.y += Input.GetAxis("Vertical") * keySpeed;
        }

        return incrementDrag;
    }

    private void ProcessZoom()
    {
        float increment = 0;

        if (!GameInput.IsPointerOverUI)
        {
            if (IsMobilePlatform())
            {
                if (Input.touchCount == 2)
                {
                    if (!isZooming)
                    {
                        isZooming = true;
                    }

                    if (fakingMobilePlatformAndTestingMobileInput)
                    {
                        sq1.transform.position = GameInput.GetWorldPointerPosition(0);
                        sq2.transform.position = GameInput.GetWorldPointerPosition(1);
                    }

                    //two finger
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    //scale the zoom increment value base on touchZoomSpeed
                    increment = deltaMagnitudeDiff * mobileScrollSpeed;
                }
                else
                {
                    isZooming = false;
                }
            }
            else
            {
                //mice and keyboard
                increment = -Input.GetAxis("Mouse ScrollWheel");
            }
        }

        //set new size
        cameraMain.orthographicSize += increment * scrollSpeed * zoomCurve.Evaluate(cameraMain.orthographicSize / minZoom);

        // clamp the camera between max and min zoom
        cameraMain.orthographicSize = Mathf.Clamp(cameraMain.orthographicSize, maxZoom, minZoom);
    }
}
