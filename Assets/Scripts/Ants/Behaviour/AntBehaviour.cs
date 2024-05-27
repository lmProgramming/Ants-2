using UnityEngine;

public abstract class Behaviour
{
    protected Creature creature;

    public enum State
    {
        FollowingPheromones,
        FoundColony,
        FoundFood,
        GoingTowardsEnemy,
        Fighting,
        SearchingForEnemies,
        Wandering,
        AttackColony,
        TryToFlee,
        Grazing
    };

    public abstract State GetState();

    public abstract void Start();

    public abstract void Update();

    public abstract void Stop();

    public virtual void DebugBehaviour() 
    {
        Debug.Log(GetState());
    }
}