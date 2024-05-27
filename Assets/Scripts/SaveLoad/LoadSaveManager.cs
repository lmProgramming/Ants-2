using System.IO;
using UnityEngine;

public sealed class LoadSaveManager : MonoBehaviour
{
    private GameManager gameManager;

    public static LoadSaveManager Instance;

    public bool loadWorld = true;

    public bool loadSettings = true;

    public bool saveWorld = true;

    public string addres = "C:/Users/PC/AppData/LocalLow/LM Programming/Ants 2";

    public string saveLocation;

    // public string saveName = "data";

    public string initialSettingsSaveName = "initialSettings";

    private void Awake()
    {
        Instance = this;

        gameManager = GameManager.Instance;
        // Debug.Log(Application.persistentDataPath);

        saveLocation = Application.persistentDataPath + "/save";
    }

    private void Start()
    {
        Load();
    }

    private void LoadSettings(GameDataHolder data)
    {
        GameManager.Instance.mapSize = data.mapSize;
        CivilizationsManager.Instance.startingAntsAmount = data.startingAntsAmount;

        CivilizationsManager.Instance.spawnRedCivilization = data.spawnRedCivilization;
        CivilizationsManager.Instance.spawnPurpleCivilization = data.spawnPurpleCivilization;
        CivilizationsManager.Instance.spawnBlueCivilization = data.spawnBlueCivilization;
        CivilizationsManager.Instance.spawnYellowCivilization = data.spawnYellowCivilization;

        // range from 0 - 10
        float foodAbundance = data.foodAbundance;
        FoodSpawner.Instance.foodNoiseThreshold = 1 - foodAbundance / 10;
        // 0 -> 4 -> 0.8, 4 -> 0.8, 10 -> 2
        FoodSpawner.Instance.foodAmountBounds *= 1 + ((Mathf.Max(4, foodAbundance) - 5) / 5);

        GameManager.Instance.spawnCaves = data.caveNoiseThreshold > 0;
        CaveGenerator.Instance.wallNoiseThreshold = data.caveNoiseThreshold;

        PheromoneUpdater.Instance.diffuse = data.diffusePheromones;
        PheromoneUpdater.Instance.pheromoneDecayRate = data.pheromoneDecayRate;

        AntsManager.Instance.maxAntsAmount = data.maxNumberOfAnts;

        GameManager.Instance.startingNestAmount = data.coloniesPerCivilizationAmount;

        RandomCreatureSpawner.Instance.spawnBeetles = data.spawnRandomBeetles;
        RandomCreatureSpawner.Instance.spawnSpiders = data.spawnRandomSpiders;

        AntsManager.Instance.antMutationStrength = data.individualAntsMutationRange;
    }

    private void Load()
    {
        if (PlayerPrefs.GetInt("resumable") == 1 && loadWorld)
        {
            GameDataHolder data = SaveGame.Load(PlayerPrefs.GetString("saveNameToLoad"));

            //GameManager.Instance.Initialize(true, data);
        }
        else
        {
            if (loadSettings)
            {
                GameDataHolder initialSettingsData = SaveGame.Load(initialSettingsSaveName);

                LoadSettings(initialSettingsData);

                GameManager.Instance.copyNewGameSettings = true;

                GameManager.Instance.Initialize(false);

            }
            else
            {
                GameManager.Instance.Initialize(false);
            }
        }
    }

    public void Save(GameManager gameManager, string name)
    {
        if (saveWorld)
        {
            GameDataHolder data = new GameDataHolder();

            data.CopyData(gameManager, name);

            SaveGame.Save(name, data);

            PlayerPrefs.SetInt("resumable", 1);

            PlayerPrefs.Save();
        }
    }

    public GameDataHolder LastSavedData()
    {
        FileInfo[] files = GetFilesFromSaveLocation();

        int highestNum = -1;
        int lastSaveIndex = -1;
        int i = 0;

        foreach (FileInfo file in files)
        {
            if (int.TryParse(Path.GetFileNameWithoutExtension(file.FullName), out int number))
            {
                if (number > highestNum)
                {
                    highestNum = number;
                    lastSaveIndex = i;
                }
            }

            i++;
        }

        return SaveGame.Load(Path.GetFileNameWithoutExtension(files[lastSaveIndex].Name));
    }

    public FileInfo[] GetFilesFromSaveLocation()
    {
        DirectoryInfo info = new DirectoryInfo(saveLocation);

        FileInfo[] fileInfo = info.GetFiles();

        return fileInfo;
    }

    public void SaveInitialSettings()
    {
        GameDataHolder initialSettingsData = new GameDataHolder();

        //initialSettingsData.creatureDesigned = creatureDesigned;
        //initialSettingsData.SaveBodyPartValues(bodyPartValues);

        initialSettingsData.genuineSave = false;

        SaveGame.Save(initialSettingsSaveName, initialSettingsData);
    }

    public void SaveInitialSimulationSettings(GameDataHolder initialSettingsData)
    {
        initialSettingsData.genuineSave = false;

        SaveGame.Save(initialSettingsSaveName, initialSettingsData);
    }

    public DirectoryInfo CreateASaveFolder()
    {
        string filePath = Application.persistentDataPath + "/save";

        DirectoryInfo folder = Directory.CreateDirectory(filePath);

        return folder;
    }

    public GameDataHolder LoadOrCreateSave(string filename)
    {
        string filePath = Path.Combine(Application.persistentDataPath, filename + ".json");

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameDataHolder>(dataAsJson);
        }
        else
        {
            return new GameDataHolder();
        }
    }

    public void ClearAllSaves()
    {
        FileInfo[] fileInfo = GetFilesFromSaveLocation();

        if (fileInfo.Length > 0)
        {
            foreach (FileInfo file in fileInfo)
            {
                file.Delete();
            }
        }
    }

    public void Clear(string filename)
    {
        SaveGame.Clear(filename);
    }
}