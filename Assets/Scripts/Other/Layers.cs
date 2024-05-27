using UnityEngine;

public sealed class Layers : MonoBehaviour
{
    private void Awake()
    {
        antLayer = 1 << LayerMask.NameToLayer("Ants");
        foodLayer = 1 << LayerMask.NameToLayer("Food");
        colonyLayer = 1 << LayerMask.NameToLayer("Colony");
        foodPheromoneLayer = 1 << LayerMask.NameToLayer("FoodPheromone");
        pathPheromoneLayer = 1 << LayerMask.NameToLayer("PathPheromone");
        wallLayer = 1 << LayerMask.NameToLayer("Wall");

        colonyAndFoodLayer = colonyLayer | foodLayer;
    }

    public static int antLayer;
    public static int foodLayer;
    public static int colonyLayer;
    public static int foodPheromoneLayer;
    public static int pathPheromoneLayer;
    public static int wallLayer;

    public static int colonyAndFoodLayer;
}
