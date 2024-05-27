using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataHolder
{
    public bool genuineSave = true;

    public string saveName;
    public string saveTimeString;
    public long saveTimeBinary;

    public float simulationTime = 0f;
    public int antsSpawned = 0;

    public List<AntData> antsData = new List<AntData>();

    public float lastTimeToGraphSaved = -100f;
    public float graphSaveInterval = 30;

    public int mapSize = 200;

    public int startingAntsAmount = 50;

    public float foodAbundance = 4;

    public bool spawnRedCivilization = true;
    public bool spawnPurpleCivilization = false;
    public bool spawnBlueCivilization = false;
    public bool spawnYellowCivilization = false;

    public bool diffusePheromones = false;
    public float pheromoneDecayRate = 0;

    public bool generateCaves = false;
    public float caveNoiseThreshold = 0.35f;

    public int maxNumberOfAnts = 4000;

    public bool spawnRandomBeetles = false;
    public bool spawnRandomSpiders = false;

    public float individualAntsMutationRange = 0.5f;

    //public bool playerPlaying = false;

    //public float simulationAccuracy = 1;

    //public float hungerMult = 0.75f;
    //public float speedMult = 2;
    //public float rotationSpeedMult = 2;
    //public float sensorMult = 1;
    //public float foodProcessingMult = 6;
    //public float mitosisSpeedMult = 1;
    //public float healSpeedMult = 1.5f;
    //public float hungerMultFromAge = 0.0033333333f;
    //public float bodyPartCostMult = 1;
    //public float genesCostMult = 1;
    //public float newCellCostMult = 1;
    //public float initialHealthPercentage = 1f;
    //
    //public bool aggresivnessPermitted = true;
    //public float aggresivnessMultiplier = 1;
    //
    //public float wasteDecompositionMultiplier = 3;
    //public float wastePerWasteObject = 50f;
    //
    //public float mutationRange = 0.05f;
    //
    //public float newBodyPartMutationChance = 0.05f;
    //public float removeBodyPartMutationChance = 0.05f;
    //public float moveBodyPartMutationChance = 0.05f;
    //
    //public float minPlantNutrition = 40;
    //public float maxPlantNutrition = 60;
    //
    //public float minMeatNutrition = 50;
    //public float maxMeatNutrition = 70;
    //
    //public float intervalBetweenBirths = 80f;
    //
    //public bool completelyRandomiseStartingGenes = false;
    //
    //public List<WrappedList> valuesByTime = new List<WrappedList>();
    //
    //public bool colorCodeCellsByState = false;
    //
    //// food spawner
    //
    //public float timeToSpawnAFood = 0.25f;
    //
    //public List<Vector3> plantFoodsPositions = new List<Vector3>();
    //
    //public List<Vector3> meatsPositions = new List<Vector3>();
    //public List<float> meatsValues = new List<float>();
    //
    //public int totalPlantFoodsSpawned = 0;
    //
    //// unnecesary stats
    //
    //public int succesfulHunts = 0;
    //
    ////// initial settings
    //
    ////public int startingNumberOfCreatures;
    //
    //// body part spawned count
    //
    //public int protectiveSpikesSpawned = 0;
    //public int compostersSpawned = 0;
    //public int turbinesSpawned = 0;
    //public int offensiveSpikesSpawned = 0;
    //
    // camera

    public Vector3 cameraPosition = Vector3.zero;
    public float cameraZoom = 10f;

    //// wastes
    //
    //public List<Vector3> wastePositions = new List<Vector3>();
    //public List<float> wasteValues = new List<float>();
    //
    // speed slider

    public float simulationSpeed = 20f;
    public float maxSliderValue = 20f;
    public int coloniesPerCivilizationAmount = 1;

    //// creature selector and manipulator
    //
    //public bool canSelectOtherCellsAsPlayer = false;
    //
    //public float energyGivenAsFeeding = 50f;
    //public float mutationMult = 10f;

    public void CopyData(GameManager gameManager, string savename = "")
    {
        if (savename == "")
        {
            saveName = gameManager.saveName;
        }
        else
        {
            saveName = savename;
        }

        saveTimeString = System.DateTime.Now.ToString();
        saveTimeBinary = System.DateTime.Now.ToBinary();

        //simulationTime = gameManager.simulationTime;
        //creaturesSpawned = gameManager.creaturesSpawned;

        //foreach (Creature creature in gameManager.creatures)
        //{
        //    creaturesData.Add(new CreatureData(creature));
        //}

        //foreach (Plant plantFood in gameManager.plantFoods)
        //{
        //    plantFoodsPositions.Add(plantFood.transform.position);
        //}

        //foreach (Meat meat in gameManager.meats)
        //{
        //    meatsPositions.Add(meat.transform.position);
        //    meatsValues.Add(meat.value);
        //}

        //lastTimeToGraphSaved = gameManager.lastTimeToGraphSaved;
        //graphSaveInterval = gameManager.graphSaveInterval;

        //mapWidth = gameManager.mapWidth;
        //mapHeight = gameManager.mapHeight;

        //playerPlaying = gameManager.playerPlaying;

        //simulationAccuracy = gameManager.simulationAccuracy;

        //hungerMult = gameManager.hungerMult;
        //speedMult = gameManager.speedMult;
        //rotationSpeedMult = gameManager.rotationSpeedMult;
        //sensorMult = gameManager.sensorMult;
        //foodProcessingMult = gameManager.foodProcessingMult;
        //mitosisSpeedMult = gameManager.mitosisSpeedMult;
        //healSpeedMult = gameManager.healSpeedMult;
        //hungerMultFromAge = gameManager.hungerMultFromAge;
        //bodyPartCostMult = gameManager.bodyPartCostMult;
        //newCellCostMult = gameManager.newCellCostMult;
        //initialHealthPercentage = gameManager.initialSize;
        //genesCostMult = gameManager.genesCostMult;

        //aggresivnessPermitted = gameManager.aggresivnessPermitted;
        //aggresivnessMultiplier = gameManager.aggresivnessMultiplier;

        //wasteDecompositionMultiplier = gameManager.wasteDecompositionMultiplier;
        //wastePerWasteObject = gameManager.wastePerWasteObject;

        //mutationRange = gameManager.mutationRange;

        //newBodyPartMutationChance = gameManager.newBodyPartMutationChance;
        //removeBodyPartMutationChance = gameManager.removeBodyPartMutationChance;
        //moveBodyPartMutationChance = gameManager.moveBodyPartMutationChance;

        //minPlantNutrition = gameManager.minPlantNutrition;
        //maxPlantNutrition = gameManager.maxPlantNutrition;

        //minMeatNutrition = gameManager.minMeatNutrition;
        //maxMeatNutrition = gameManager.maxMeatNutrition;

        //intervalBetweenBirths = gameManager.intervalBetweenBirths;

        //completelyRandomiseStartingGenes = gameManager.completelyRandomiseStartingGenes;

        //foreach (List<float> val in gameManager.valuesByTime)
        //{
        //    valuesByTime.Add(new WrappedList(val));
        //}

        //colorCodeCellsByState = gameManager.colorCodeCellsByState;

        //genesOn = gameManager.genesOn;

        //creatureNames = gameManager.creatureNames;

        //// unnecesary stats

        //succesfulHunts = gameManager.cellsKilledByOtherCells;

        //// food spawner

        //timeToSpawnAFood = FoodSpawner.Instance.timeToSpawnAFood;

        //totalPlantFoodsSpawned = FoodSpawner.Instance.foodPlantsSpawned;

        //// body part spawned count

        //protectiveSpikesSpawned = BodyPartsSpawner.Instance.protectiveSpikesSpawned;
        //compostersSpawned = BodyPartsSpawner.Instance.compostersSpawned;
        //turbinesSpawned = BodyPartsSpawner.Instance.turbinesSpawned;
        //offensiveSpikesSpawned = BodyPartsSpawner.Instance.offensiveSpikesSpawned;

        // camera

        cameraPosition = Camera.main.transform.position;
        cameraZoom = Camera.main.orthographicSize;

        // wastes

        //foreach (GameObject wasteObject in WasteManager.Instance.wastes)
        //{
        //    wastePositions.Add(wasteObject.transform.position);
        //    wasteValues.Add(wasteObject.GetComponent<Waste>().value);
        //}

        // speed slider

        //simulationSpeed = gameManager.simulationSpeed;

        //maxSliderValue = SimulationUI.Instance.speedSlider.maxValue;

        //// creature selector and manipulator

        //canSelectOtherCellsAsPlayer = CreatureSelector.Instance.canSelectOtherCellsAsPlayer;

        //energyGivenAsFeeding = CreatureSelector.Instance.creatureManipulator.energyGivenAsFeeding;
        //mutationMult = CreatureSelector.Instance.creatureManipulator.mutationMult;
    }

    public void LoadSettingsFromPlayerPrefs()
    {
        startingAntsAmount = PlayerPrefs.GetInt("startingAntsAmount", 50);

        mapSize = PlayerPrefs.GetInt("mapSize", 200);

        foodAbundance = PlayerPrefs.GetFloat("foodAbundance", 4);

        spawnRedCivilization = PlayerPrefs.GetInt("startingCivilizationsAmount", 1) >= 1;
        spawnPurpleCivilization = PlayerPrefs.GetInt("startingCivilizationsAmount", 1) >= 2;
        spawnBlueCivilization = PlayerPrefs.GetInt("startingCivilizationsAmount", 1) >= 3;
        spawnYellowCivilization = PlayerPrefs.GetInt("startingCivilizationsAmount", 1) >= 4;

        diffusePheromones = PlayerPrefs.GetInt("diffusePheromones", 0) == 1;
        pheromoneDecayRate = PlayerPrefs.GetFloat("pheromoneDecayRate", 3) / 100;

        generateCaves = PlayerPrefs.GetInt("generateCaves", 0) == 1;
        caveNoiseThreshold = PlayerPrefs.GetFloat("caveDensity", 0.35f);

        maxNumberOfAnts = PlayerPrefs.GetInt("maximumAntsNumber", 4000);

        coloniesPerCivilizationAmount = PlayerPrefs.GetInt("startingNestsAmount", 1);

        spawnRandomSpiders = PlayerPrefs.GetInt("spawnRandomSpiders", 0) == 1;
        spawnRandomBeetles = PlayerPrefs.GetInt("spawnRandomBeetles", 0) == 1;

        individualAntsMutationRange = PlayerPrefs.GetFloat("individualAntsMutationRange", 0.5f);
    }
}