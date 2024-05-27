using UnityEngine;

public class FoundFood : Behaviour
{
    private AntBody body;
    private Ant ant;

    private GridTile foodTile;

    public FoundFood(GridTile foodTile, Ant ant)
    {
        this.foodTile = foodTile;
        this.ant = ant;
        body = ant.antBody;
    }

    override public State GetState()
    {
        return State.FoundFood;
    }

    public override void Start()
    {
        body.SetFullForce(true);

        ant.antMovement.SetTarget(foodTile.position);
    }

    override public void Update()
    {
        bool canPickUpFood = false;

        body.antMovement.SetTarget(foodTile.position);  

        if (body.TileAt.GetTileState() == TileState.Food)
        {
            foodTile = body.TileAt;
            canPickUpFood = true;
        }
        else if (Vector2.Distance(body.Position, foodTile.position) < 1.5f)
        {
            canPickUpFood = true;
        }

        if (canPickUpFood)
        {
            if (foodTile.foodAmount > 0)
            {
                body.GetFood(foodTile);

                if (!body.CargoFull())
                {
                    body.GetMoreFood(MapGenerator.Instance.GatherFoodAroundTile(foodTile, body.RemainingCargoCapacity(), ant.CivIndex));
                }

                ant.antMovement.RotateToBack();
                ant.antMovement.UpdatePosition(true);

                ant.antBrain.SetDefaultBehaviour();
            }
            else
            {
                GridTile anotherfoodTile = ant.antBody.ScanForFoodAroundTile(foodTile);
                if (anotherfoodTile != null)
                {
                    foodTile = anotherfoodTile;
                    body.antMovement.SetTarget(foodTile.position);
                }
                else
                {
                    ant.antBrain.SetDefaultBehaviour();
                }
            }
        }
    }

    public override void DebugBehaviour()
    {
        Debug.Log(foodTile.gridPos);
    }

    public override void Stop()
    {

    }
}
