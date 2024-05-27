public class Wandering : Behaviour
{
    public float timeLeftToWander;

    private bool infiniteWander = false;

    public Wandering(float timeLeftToWanderSetNegativeForInfinite, Creature creature)
    {
        timeLeftToWander = timeLeftToWanderSetNegativeForInfinite;

        this.creature = creature;

        infiniteWander = timeLeftToWander < 0;
    }

    public override State GetState()
    {
        return State.Wandering;
    }

    public override void Start()
    {

    }

    public override void Update()
    {
        timeLeftToWander -= GameInput.deltaTime;
        if (!infiniteWander && timeLeftToWander <= 0)
        {
            creature.brain.SetDefaultBehaviour();
        }
    }

    public override void Stop()
    {

    }
}
