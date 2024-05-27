using UnityEngine;

public class PheromonePlacer : MonoBehaviour
{
    // actual values
    private float actualPheromonePlacementInterval;

    // how often shoud we place a pheromone?
    public float pheromonePlacementInterval;
    public Ant ant;
    private float timeLastPheromonePlaced = 0.2f;

    public float PheromoneLeft { get => pheromoneLeft; private set => pheromoneLeft = value; }
    public bool hasPheromone;
    private float pheromoneLeft;
    public float pheromoneCapacity = 800;

    public bool taintedByDepletedFoodPheromone = false;

    // Use this for initialization
    private void Start()
    {
        actualPheromonePlacementInterval = pheromonePlacementInterval;

        RechargeAtBase();

        ant.antBody.visitedColonyAction += RechargeAtBase;
    }

    public void RechargeAtBase()
    {
        hasPheromone = true;

        pheromoneLeft = pheromoneCapacity;
    }

    // Update is called once per frame
    public void UpdatePheromone()
    {
        if (timeLastPheromonePlaced < Time.time - actualPheromonePlacementInterval)
        {
            if (pheromoneLeft > 0)
            {
                float pheromoneSpent = pheromoneLeft / pheromoneCapacity + 0.5f;
                PlacePheromone(ant.antBody.TileAt, pheromoneSpent);
                pheromoneLeft -= pheromoneSpent;
            }
            else
            {
                hasPheromone = false;
            }

            timeLastPheromonePlaced = Time.time;

            if (taintedByDepletedFoodPheromone)
            {
                ant.antBody.TileAt.MultiplyFoodPheromoneInNeighbours(0.75f, ant.CivIndex);
            }
        }
    }

    private void PlacePheromone(GridTile tileAt, float amount)
    {
        MapGenerator.Instance.AddPheromoneAt(tileAt, ant.CivIndex, ant.antBody.HasAnyFood, amount);
    }
}
