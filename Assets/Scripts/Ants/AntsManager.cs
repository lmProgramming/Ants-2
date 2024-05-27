using System.Collections.Generic;
using UnityEngine;

public sealed class AntsManager : MonoBehaviour
{
    public static AntsManager Instance;

    private IAntUpdateJobsManager iAntUpdatePosJobsManager;

    public int maxAntsAmount = 800;

    public int antsCount = 0;

    public Vector2 antBaseAutonomyRange;

    private Ant[] allAntsArray;
    private List<int> emptyIndexes = new List<int>();

    [HideInInspector]
    public List<Ant> existingAnts = new List<Ant>();

    public List<Ant>[,] listOfAntsInGrid;
    public int antGridSize;

    private float timeToUpdateAntsInGrid;
    public float timeBetweenUpdatingAntsInGrid;

    [SerializeField] private Sprite[] antSprites;
    [SerializeField] private Sprite[] antSoldierSprites;
    [SerializeField] private Sprite[] antQueenSprites;

    public static int WORKER_COST =  1000;
    public static int SOLDIER_COST = 1500;
    public static int QUEEN_COST =  10000;

    public float antMutationStrength = 0.1f;

    public void Awake()
    {
        Instance = this;

        iAntUpdatePosJobsManager = new IAntUpdateJobsManager();
    }

    public static bool IsCivIndexAntIndex(int index)
    {
        return index < 4 && index > 0;
    }

    public void ResetListOfAntsInGrid()
    {
        for (int x = 0; x < listOfAntsInGrid.GetLength(0); x++)
        {
            for (int y = 0; y < listOfAntsInGrid.GetLength(1); y++)
            {
                listOfAntsInGrid[x, y].Clear();
            }
        }
    }

    public Vector2Int GridPositionFromPosition(Vector2 pos)
    {
        return new Vector2Int(MathExt.ClampInt(Mathf.FloorToInt((pos.x + MapGenerator.mapWidthHalfWithoutWalls) / antGridSize), 0, listOfAntsInGrid.GetLength(0) - 1),
                              MathExt.ClampInt(Mathf.FloorToInt((pos.y + MapGenerator.mapHeightHalfWithoutWalls)/ antGridSize), 0, listOfAntsInGrid.GetLength(1) - 1));
    }

    public void InsertAntIntoGrid(Ant ant)
    {
        Vector2Int gridPos = GridPositionFromPosition(ant.Position);
        listOfAntsInGrid[gridPos.x, gridPos.y].Add(ant);
    }

    public int AmountOfEmptyIndexes()
    {
        return emptyIndexes.Count;
    }

    // civilizationIndex = -1 means ignore civ index
    public Ant[] FastAntsInCircle(Vector2 centerOfCircle, float radius, int civilizationIndex = -1)
    {
        List<Ant> antsInsideCircle = new List<Ant>();

        Vector2Int gridPosLeftDown = GridPositionFromPosition(centerOfCircle + new Vector2(-radius, -radius));
        Vector2Int gridPosRightTop = GridPositionFromPosition(centerOfCircle + new Vector2( radius,  radius));

        for (int x = gridPosLeftDown.x; x <= gridPosRightTop.x; x++)
        {
            for (int y = gridPosLeftDown.y; y <= gridPosRightTop.y; y++)
            {
                foreach (Ant ant in listOfAntsInGrid[x, y])
                {
                    if (Vector2.Distance(ant.Position, centerOfCircle) < radius)
                    {
                        if (civilizationIndex == -1 || ant.CivIndex == civilizationIndex)
                        {
                            antsInsideCircle.Add(ant);
                        }
                    }
                }
            }
        }

        return antsInsideCircle.ToArray();
    }

    public Ant AntClosestToPosition(Vector2 centerOfCircle, float maxDistance)
    {
        Vector2Int gridPosLeftDown = GridPositionFromPosition(centerOfCircle + new Vector2(-maxDistance, -maxDistance));
        Vector2Int gridPosRightTop = GridPositionFromPosition(centerOfCircle + new Vector2( maxDistance,  maxDistance));

        float minDistance = maxDistance;
        Ant closestAnt = null;

        for (int x = gridPosLeftDown.x; x <= gridPosRightTop.x; x++)
        {
            for (int y = gridPosLeftDown.y; y <= gridPosRightTop.y; y++)
            {
                foreach (Ant ant in listOfAntsInGrid[x, y])
                {
                    float distance = Vector2.Distance(ant.Position, centerOfCircle);
                    if (Vector2.Distance(ant.Position, centerOfCircle) < minDistance)
                    {
                        minDistance = distance;
                        closestAnt = ant;   
                    }
                }
            }
        }

        return closestAnt;
    }

    public Ant FastAntInCircle(Vector2 centerOfCircle, float radius, int civilizationIndex = -1)
    {
        Vector2Int gridPosLeftDown = GridPositionFromPosition(centerOfCircle + new Vector2(-radius, -radius));
        Vector2Int gridPosRightTop = GridPositionFromPosition(centerOfCircle + new Vector2( radius,  radius));

        for (int x = gridPosLeftDown.x; x <= gridPosRightTop.x; x++)
        {
            for (int y = gridPosLeftDown.y; y <= gridPosRightTop.y; y++)
            {
                foreach (Ant ant in listOfAntsInGrid[x, y])
                {
                    if (Vector2.Distance(ant.Position, centerOfCircle) < radius)
                    {
                        if (civilizationIndex == -1 || ant.CivIndex == civilizationIndex)
                        {
                            return ant;
                        }
                    }
                }
            }
        }

        return null;
    }

    public Ant[] FastAntsInCircleExcludingCivIndex(Vector2 centerOfCircle, float radius, int excludedCivIndex)
    {
        List<Ant> antsInsideCircle = new List<Ant>();

        Vector2Int gridPosLeftDown = GridPositionFromPosition(centerOfCircle + new Vector2(-radius, -radius));
        Vector2Int gridPosRightTop = GridPositionFromPosition(centerOfCircle + new Vector2(radius, radius));

        for (int x = gridPosLeftDown.x; x <= gridPosRightTop.x; x++)
        {
            for (int y = gridPosLeftDown.y; y <= gridPosRightTop.y; y++)
            {
                foreach (Ant ant in listOfAntsInGrid[x, y])
                {
                    if (Vector2.Distance(ant.Position, centerOfCircle) < radius)
                    {
                        if (excludedCivIndex != ant.CivIndex)
                        {
                            antsInsideCircle.Add(ant);
                        }
                    }
                }
            }
        }

        return antsInsideCircle.ToArray();
    }

    // civilizationIndex = -1 means ignore civ index
    public Ant[] AntsInCircle(Vector2 centerOfCircle, float radius, int civilizationIndex = -1)
    {
        List<Ant> antsInsideCircle = new List<Ant>();

        for (int i = 0; i < existingAnts.Count; i++)
        {
            if (Vector2.Distance(existingAnts[i].Position, centerOfCircle) < radius)
            {
                if (civilizationIndex == -1 || existingAnts[i].CivIndex == civilizationIndex)
                {
                    antsInsideCircle.Add(existingAnts[i]);
                }
            }
        }

        return antsInsideCircle.ToArray();
    }

    public void AddNewAnt(Ant ant, bool addToAntUpdateJob)
    {
        int index = emptyIndexes[0];
        emptyIndexes.RemoveAt(0);

        allAntsArray[index] = ant;
        ant.UpdateJobIndex = index;

        existingAnts.Add(ant);

        antsCount += 1;
        
        ant.antBrain.SetBaseAutonomy(Random.Range(antBaseAutonomyRange.x, antBaseAutonomyRange.y));

        ant.antMovement.MutateTraits(antMutationStrength);

        if (addToAntUpdateJob)
        {
            iAntUpdatePosJobsManager.AddAnt(ant);
        }
    }

    public void RemoveAnt(Ant ant)
    {
        emptyIndexes.Add(ant.UpdateJobIndex);
        allAntsArray[ant.UpdateJobIndex] = null;

        existingAnts.Remove(ant);

        antsCount -= 1;
    }

    public void Initialize()
    {        
        allAntsArray = new Ant[maxAntsAmount];
        for (int i = 0; i < maxAntsAmount; i++)
        {
            emptyIndexes.Add(i);
        }
        MathExt.Shuffle(emptyIndexes);

        int gridArrayWidth = Mathf.CeilToInt((float)MapGenerator.mapWidthWithoutWalls / antGridSize) + 1;
        int gridArrayHeight = Mathf.CeilToInt((float)MapGenerator.mapHeightWithoutWalls / antGridSize) + 1;

        listOfAntsInGrid = new List<Ant>[gridArrayWidth, gridArrayHeight];

        for (int x = 0; x < gridArrayWidth; x++)
        {
            for (int y = 0; y < gridArrayHeight; y++)
            {
                listOfAntsInGrid[x, y] = new List<Ant>();
            }
        }

        iAntUpdatePosJobsManager.InitializeLists(allAntsArray);
    }

    public void AssignAntsToJobs()
    {
        iAntUpdatePosJobsManager.AssignAntsToJob(allAntsArray);
    }

    public Sprite GetAntSpriteBasedOffIndex(int index)
    {
        return GetAntSprite(Ant.AntType.Worker, index);
    }

    private void Update()
    {
        if (!GameManager.Instance.paused)
        {
            for (int i = 0; i < allAntsArray.Length; i++)
            {
                if (allAntsArray[i])
                {
                    allAntsArray[i].UpdateAnt();
                }
            }

            if (Time.time >= timeToUpdateAntsInGrid)
            {
                timeToUpdateAntsInGrid = Time.time + timeBetweenUpdatingAntsInGrid;

                ResetListOfAntsInGrid();

                ResetAntsNumberOnTiles();

                for (int i = 0; i < existingAnts.Count; i++)
                {
                    InsertAntIntoGrid(existingAnts[i]);
                }
            }

            UpdateAntUpdateJobs();
        }
    }

    public void ResetAntsNumberOnTiles()
    {
        IGridTileUpdater.Instance.ResetAntsOnTiles();
    }

    public void UpdateAntUpdateJobs()
    {
        iAntUpdatePosJobsManager.UpdatePositions(allAntsArray);
    }

    private void OnDestroy()
    {
        iAntUpdatePosJobsManager.DisposeOfArrays();
    }

    public Sprite GetAntSprite(Ant.AntType antType, int index)
    {
        index = index < 0 ? 0 : index;

        switch (antType)
        {
            case Ant.AntType.Queen:
                return antQueenSprites[index];
            case Ant.AntType.Soldier:
                return antSoldierSprites[index];
            default:
                return antSprites[index];
        }
    }
}
