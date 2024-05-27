using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TerrainManipulatorSelectorUI : MonoBehaviour
{
    public RectTransform optionsHolder;

    public TerrainManipulatorOptionUI.TerrainOption selectedTerrainOption;

    public bool activeAndOverridingCamera = false;
    public bool entityPlacementMode = false;

    [SerializeField] private CameraManager cameraManager;

    private Vector2Int lastGridPosPlaced;

    public GameObject outlineOfSelectedObject;

    private bool wasHoldingTwoFingers = false;

    public TerrainManipulator terrainManipulator;
    public EntityPlacer entityPlacer;

    public float width = 1400;
    public float startPositionBias = 10;

    private List<TerrainManipulatorOptionUI> terrainManipulatorOptions = new List<TerrainManipulatorOptionUI>();

    public SizeOptionsManager sizeOptionsManager;

    private void Start()
    {
        terrainManipulatorOptions = optionsHolder.GetComponentsInChildren<TerrainManipulatorOptionUI>().ToList();

        SetupOptions();

        SetActive(false);
    }

    private void SetupOptions()
    {
        Vector3 xDistChange = new Vector3(width / (terrainManipulatorOptions.Count - 1), 0, 0);

        Vector3 spawnPos = new Vector3(startPositionBias - width / 2, 0, 0);

        for (int i = 0; i < terrainManipulatorOptions.Count; i++)
        {
            terrainManipulatorOptions[i].GetComponent<RectTransform>().anchoredPosition = spawnPos;

            spawnPos += xDistChange;
        }
    }

    public static bool Entity(TerrainManipulatorOptionUI.TerrainOption option)
    {
        switch (option)
        {
            case TerrainManipulatorOptionUI.TerrainOption.Colony:
            case TerrainManipulatorOptionUI.TerrainOption.Ant:
            case TerrainManipulatorOptionUI.TerrainOption.Soldier:
            case TerrainManipulatorOptionUI.TerrainOption.Queen:
            case TerrainManipulatorOptionUI.TerrainOption.Spider:
            case TerrainManipulatorOptionUI.TerrainOption.Beetle:
                return true;
            default:
                return false;
        }
    }

    public void Select(TerrainManipulatorOptionUI option)
    {
        if (selectedTerrainOption != option.terrainOption)
        {
            selectedTerrainOption = option.terrainOption;

            SetOptionOutline(option.gameObject);

            SetActive(true);

            MainGameHelpUI.Instance.ShowNameOfSelectedOption(option.nameOfOption);
        }
        else
        {
            Deselect();
        }
    }

    public void Deselect()
    {
        selectedTerrainOption = TerrainManipulatorOptionUI.TerrainOption.None;

        DisableOptionOutline();

        SetActive(false);
    }

    public void SetOptionOutline(GameObject selectedTerrainOption)
    {
        outlineOfSelectedObject.SetActive(true);
        outlineOfSelectedObject.transform.position = selectedTerrainOption.transform.position;
    }

    public void DisableOptionOutline()
    {
        outlineOfSelectedObject.SetActive(false);
    }

    [SerializeField] private bool justClickedOccured = false;
    [SerializeField] private bool firstClickOnUI = false;

    private void Update()
    {
        if (!activeAndOverridingCamera)
        {
            return;
        }

        if (!GameInput.Pressing)
        {
            wasHoldingTwoFingers = false;
            justClickedOccured = false;
            firstClickOnUI = false;
            SetCameraDraggingType(true);

            return;
        }

        bool justClicked = GameInput.JustClicked;

        if (justClicked)
        {
            firstClickOnUI = GameInput.IsPointerOverUI;
        }

        if (GameInput.IsPointerOverUI || firstClickOnUI || GameInput.pressingTime <= 0.04f)
        {
            return;
        }

        Vector2Int gridPosition = MapGenerator.GridPos(GameInput.WorldPointerPosition);

        if (GameInput.TouchesAndPointersCount > 1)
        {
            wasHoldingTwoFingers = true;
            SetCameraDraggingType(false);
        }

        if (wasHoldingTwoFingers)
        {
            return;
        }

        if (!justClickedOccured)
        {
            justClicked = true;
            justClickedOccured = true;
        }

        if (justClicked)
        {
            lastGridPosPlaced = MapGenerator.GridPos(GameInput.WorldPointerPosition);
            justClickedOccured = true;

            if (!MapGenerator.Instance.AlmostInsideMap(gridPosition))
            {
                wasHoldingTwoFingers = true;
                SetCameraDraggingType(false);
                return;
            }
        }

        if (Entity(selectedTerrainOption))
        {
            if (justClicked)
            {
                entityPlacer.PlaceEntity(selectedTerrainOption, GameInput.WorldPointerPosition, GameManager.Instance.civIndexOfCurrentlyShownPheromonesInTiles);
            }
        }
        else
        {
            terrainManipulator.ManipulateTerrain(selectedTerrainOption, gridPosition, lastGridPosPlaced);
        }

        lastGridPosPlaced = gridPosition;
    }

    private void SetActive(bool active)
    {
        activeAndOverridingCamera = active;

        SetCameraDraggingType(active);

        if (active && !Entity(selectedTerrainOption))
        {
            sizeOptionsManager.SetVisible(true);
        }
        else
        {
            sizeOptionsManager.SetVisible(false);
        }
    }

    private void SetCameraDraggingType(bool boundaries)
    {
        cameraManager.curDraggingType = boundaries ? CameraManager.DraggingType.Boundaries : CameraManager.DraggingType.Generic;
    }
}
