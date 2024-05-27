using UnityEngine;
using UnityEngine.Events;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool copyNewGameSettings = true;
    public bool spawnCivilizations = true;
    public bool spawnFood = true;
    public bool spawnCaves = true;
    public bool useCamera = true;
    public int startingNestAmount = 1;
    public bool useUI = true;

    public float simulationSpeed = 1f;
    public bool paused = false;
    public float simulationTime = 0f;

    // public PheromoneSpawner pheromoneSpawner;
    // public AntsManager antsManager;

    public int mapSize;

    public string saveName;

    // public NewGameData newGameData;

    // Start is called before the first frame update

    public TopBarUI topBarUI;

    public UnityEvent changedCivIndexOfCurrentlyShownPheromonesInTiles;

    private void Awake()
    {
        Instance = this;
    }

    public int civIndexOfCurrentlyShownPheromonesInTiles { get; private set; } = 0;

    public void Initialize(bool loadGame)
    {
        SetMapSize(mapSize);

        if (!loadGame)
        {
            StartNewGame();
        }

        ChangeSimulationSpeed(simulationSpeed);

        SetFrameRateMobile();
    }

    private void StartNewGame()
    {
        MapGenerator.Instance.GenerateMap(mapSize + 2 * MapGenerator.Instance.borderWallLength);

        if (spawnCaves)
        {
            CaveGenerator.Instance.SpawnCaves();
        }

        if (spawnFood)
        {
            FoodSpawner.Instance.SpawnStartingFood();
        }

        if (spawnCivilizations)
        {
            CivilizationsManager.Instance.SpawnStartingColonies(startingNestAmount);
        }

        AntsManager.Instance.Initialize();

        CivilizationsManager.Instance.SpawnStartingAnts();

        AntsManager.Instance.AssignAntsToJobs();

        if (useUI)
        {
            topBarUI.GenerateTopBar(CivilizationsManager.Instance.activeIndexes, true);
        }

        if (useCamera)
        {
            CameraManager.Instance.updateCamera = true;
        }
    }

    private void SetFrameRateMobile()
    {
        Screen.sleepTimeout = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
    }

    public void ChangeSimulationSpeed(float value)
    {
        simulationSpeed = value;
        GameInput.simSpeed = 1;
        Time.timeScale = value;

        if (value == 0)
        {
            Pause();
        }
        else if (paused)
        {
            UnPause();
        }
    }

    private void Pause()
    {
        paused = true;
    }

    private void UnPause()
    {
        paused = false;
    }

    public void OnValidate()
    {
        ChangeSimulationSpeed(simulationSpeed);
    }

    public void UpdateAntsVariables()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        simulationTime += GameInput.simDeltaTime;
    }

    public void SetMapSize(int x)
    {
        if (x <= 1)
        {
            x = 10;
        }

        mapSize = x;

        if (useCamera)
        {
            float mapSizeBoundary = mapSize * 0.5f;
            CameraManager.Instance.SetBoundaries(-mapSizeBoundary, mapSizeBoundary, -mapSizeBoundary, mapSizeBoundary);
        }
    }

    public void SaveGame(string savename = "")
    {
        if (savename == "")
        {
            savename = saveName;
        }

        // LoadSaveManager.Instance.Save(this, savename);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void ChangeCivIndexOfCurrentlyShownPheromonesInTiles(int newCivIndex)
    {
        civIndexOfCurrentlyShownPheromonesInTiles = newCivIndex;

        changedCivIndexOfCurrentlyShownPheromonesInTiles.Invoke();
    }
}
