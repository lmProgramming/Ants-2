using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AntData
{
    public AntMovement movement;
    public AntBrain brain;
    public AntBody body;

    public int UpdateJobIndex { get; set; }
    public Civilization Civilization { get; set; }
    public int CivIndex => Civilization.civIndex;
    public Vector2 Position
    {
        get => movement.Position;
        set => movement.Position = value;
    }

    public Ant enemyAnt;

    public List<int> enemyCivIndexes = new List<int>();

    public Behaviour.State s;

    public AntData()
    {

    }

    public AntData(Ant ant)
    {

    }
}
