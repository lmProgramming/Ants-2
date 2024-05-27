using UnityEngine;

public sealed class AntSpawner : MonoBehaviour
{
    public static AntSpawner Instance;

    public GameObject antPrefab;
    public GameObject antSoldierPrefab;
    public GameObject antQueenPrefab;

    private Transform antHolder;
    private int antsLeftToFillAntHolder = -1;
    private int antsPerAntHolder = 300;
    private AntsManager antsManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        antsManager = AntsManager.Instance;
    }

    public bool CheckIfCanSpawnAnt(Vector2 position)
    {
        return antsManager.AmountOfEmptyIndexes() > 0 && MapGenerator.Instance.TileAt(position).GetTileState() != TileState.Wall;
    }

    private string AntTypeToString(Ant.AntType antType)
    {
        switch (antType)
        {
            case Ant.AntType.Worker:
                return "worker";
            case Ant.AntType.Soldier:
                return "soldier";
            case Ant.AntType.Queen:
                return "queen";
            default:
                return "";
        }
    }

    private GameObject GetAntPrefabFromType(Ant.AntType antType)
    {
        switch (antType)
        {
            case Ant.AntType.Soldier:
                return antSoldierPrefab;
            case Ant.AntType.Queen:
                return antQueenPrefab;
            default:
                return antPrefab;
        }
    }

    public Ant SpawnAnt(Vector2 position, float zRotation, Civilization civilization, Ant.AntType antType, Sprite sprite = null, bool addToAntUpdaterJob = true, bool popAnimation = false)
    {
        if (CheckIfCanSpawnAnt(position))
        {
            Transform antParent;
            if (antsLeftToFillAntHolder <= 0)
            {
                antParent = new GameObject().transform;
                antHolder = antParent;
                antsLeftToFillAntHolder = antsPerAntHolder;
            }
            else
            {
                antParent = antHolder;
            }

            GameObject newAntObject = Instantiate(GetAntPrefabFromType(antType), antParent);

            antsLeftToFillAntHolder--;

            newAntObject.name = civilization.name + " ant " + AntTypeToString(antType) + " " + civilization.antsSpawnedNumber;

            newAntObject.transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, zRotation));

            Ant ant = newAntObject.GetComponent<Ant>();

            ant.antMovement.Position = position;
                        
            if (sprite == null)
            {
                sprite = antsManager.GetAntSprite(antType, civilization.civIndex);
            }
            ant.antBody.UpdateSprite(sprite);

            civilization.SpawnAnt(ant);

            antsManager.AddNewAnt(ant, addToAntUpdaterJob);

            if (popAnimation)
            {
                AnimationSpawner.AddPopAnimation(newAntObject);
            }

            return ant;
        }

        return null;
    }
}
