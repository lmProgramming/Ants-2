using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grazing : Behaviour
{
    private float foodHarvestSpeedMultiplier = 0.2f;
    public override State GetState()
    {
        return State.Grazing;
    }

    public Grazing(Creature creature)
    {
        this.creature = creature;
    }

    public override void Start()
    {
        creature.body.SetFullForce(false);
    }

    public override void Update()
    {
        GridTile tile = creature.body.TileAt;

        if (tile.foodAmount > 0)
        {
            float foodAmount = tile.GetFoodWithoutDepletedPheromone(creature.body.GetCargoCapacity * Time.deltaTime * foodHarvestSpeedMultiplier * Mathf.Clamp01(tile.foodAmount / FoodSpawner.Instance.AverageFood));
            if (RotateBackOnFood())
            {
                creature.movement.RotateToBackSlow();
            }

            creature.body.GetFood(foodAmount * 2);
        }
    }

    private bool RotateBackOnFood()
    {
        return Random.value < 0.5 * Time.deltaTime;
    }

    public override void Stop()
    {

    }
}
