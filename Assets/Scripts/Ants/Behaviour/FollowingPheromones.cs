using UnityEngine;

public class FollowingPheromones : Behaviour
{
    private Ant ant;

    // TODO just make class DeltaTimer
    public float sensorCheckInterval = 0.3f;

    private float timeLastSensorCheck = 0;

    private float startingTimeBias;

    public FollowingPheromones(Ant ant, float startingTimeBias = 0)
    {
        this.ant = ant;
        this.startingTimeBias = startingTimeBias;
    }

    public override void Start()
    {
        timeLastSensorCheck = Time.time + startingTimeBias + Random.Range(0, sensorCheckInterval);

        ant.antBody.SetFullForce(false);
    }

    override public State GetState()
    {
        return State.FollowingPheromones;
    }

    override public void Update()
    {
        if (timeLastSensorCheck > Time.time - sensorCheckInterval)
            return;

        timeLastSensorCheck = Time.time;

        SetScanState();
    }

    public void SetScanState()
    {
        bool scanForFoodPheromone = !(ant.antBody.HasAnyFood || !ant.antBody.placer.hasPheromone);

        ant.antBrain.ScanState = (AntScanState) (scanForFoodPheromone? 1 : 2 );
    }

    public override void Stop()
    {

    }
}
