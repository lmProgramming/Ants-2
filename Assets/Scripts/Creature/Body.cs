using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Body : MonoBehaviour
{
    public Creature creature;

    public Movement movement;

    public UnityAction hitAWallAction;
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    public bool FullForce { private set; get; }

    [SerializeField]
    protected float cargoCapacity = 50f;
    public float GetCargoCapacity { get => cargoCapacity; }

    public bool HasAnyFood => foodAmount > 0;

    [SerializeField]
    protected float foodAmount = 0f;

    public virtual void SetFullForce(bool fullForce)
    {
        FullForce = fullForce;
    }
    
    public Vector2 Position
    {
        get => movement.Position;
        set => movement.Position = value;
    }

    public Vector2 foodLeftAfterDeathRange;

    public float Damage { get => damage; private set => damage = value; }
    public float NextDamageDealTime { get => nextDamageDealTime; private set => nextDamageDealTime = value; }
    public float DefaultDamageCooldown { get => defaultDamageCooldown; private set => defaultDamageCooldown = value; }

    public float health = 10;
    public float maxHealth = 10;
    [SerializeField]
    protected float damage = 2;
    protected float nextDamageDealTime = 0;
    protected float previousDamageDealTime = 0;
    [SerializeField]
    protected float defaultDamageCooldown = 1;

    protected Vector2 lastPosition;

    public List<Creature> enemiesFightingThisCreature = new List<Creature>();

    protected bool hitAWallLastFrame = false;

    public GridTile TileAt { get; protected set; }

    public void UpdateSprite(Sprite newSprite) => spriteRenderer.sprite = newSprite;

    public void ResetDamageCooldown()
    {
        previousDamageDealTime = nextDamageDealTime;
        nextDamageDealTime += DefaultDamageCooldown;
    }

    protected void Start()
    {
        TileAt = MapGenerator.Instance.TileAtAssumeInsideMap(Position);
        lastPosition = Position;
    }

    // returns if is the one who killed it
    public bool GetDamaged(float damage)
    {
        // already killed
        if (creature.IsDead)
        {
            return false;
        }

        health -= damage;

        if (health <= 0)
        {
            creature.Die();
            return true;
        }
        return false;
    }

    public virtual void GetFood(float foodGatheredAmount)
    {

    }

    public void GetFood(GridTile foodObject)
    {
        float foodGatheredAmount = foodObject.GetFood(cargoCapacity, creature.CivIndex);
        GetFood(foodGatheredAmount);
    }

    public void Heal()
    {
        health = maxHealth;
    }

    public void SnapToLastPosition()
    {
        Position = MapGenerator.CentralizeVectorPosition(lastPosition);
    }

    public virtual void DropFoodOnDeath()
    {
        float foodAmountPerTile = 300;

        float foodLeftAfterDeath = Random.Range(foodLeftAfterDeathRange.x, foodLeftAfterDeathRange.y);  

        float radius = Mathf.Sqrt(foodLeftAfterDeath / (Mathf.PI * foodAmountPerTile));

        GridTile[] tiles = MapGenerator.Instance.CircleAroundExcludingMapBorderWalls(TileAt.gridPos, radius);

        for (int i = 0; i < tiles.Length; i++)
        {
            float foodAdded = (Random.value + 0.5f) * foodAmountPerTile;
            if (tiles[i].AddOrChangeToFoodIfFoodable(foodAdded))
            {
                foodLeftAfterDeath -= foodAdded;
                if (foodLeftAfterDeath <= 0)
                {
                    break;
                }
            }
        }

        if (foodLeftAfterDeath > 0) 
        {
            TileAt.AddOrChangeToFoodIfFoodable(foodLeftAfterDeath);
        }
    }

    public virtual void UpdateCreature()
    {
        if (!movement.Frozen)
        {
            movement.UpdatePosition(FullForce);
        }
        else
        {
            FightingVisual();
        }

        if (CheckIfOnNewTile())
        {
            if (TileAt.GetTileState() == TileState.Wall)
            {
                SnapToLastPosition();

                movement.RotateToRandomBackDir();

                hitAWallAction.Invoke();

                hitAWallLastFrame = true;

                return;
            }
            else
            {
                hitAWallLastFrame = false;

                lastPosition = Position;
            }
        }
    }

    virtual protected bool CheckIfOnNewTile()
    {
        GridTile newTile = MapGenerator.Instance.TileAtAssumeInsideMap(Position);
        if (newTile != TileAt)
        {
            TileAt = newTile;
            TileAt.SetCreatureOnTile(creature);
            return true;
        }

        return false;
    }

    public void AddEnemyFightingThisAnt(Creature enemy)
    {
        enemiesFightingThisCreature.Add(enemy);
    }

    public virtual void StartedFighting()
    {
        nextDamageDealTime = Time.time + DefaultDamageCooldown;
        previousDamageDealTime = Time.time;
    }

    public AnimationCurve fightAnimation;
    public float fightAnimationDistanceMultiplier = 0.5f;
    public void FightingVisual()
    {
        float curveEvaluationX = (Time.time - previousDamageDealTime) / DefaultDamageCooldown;
                
        transform.position = Position + (Vector2)(-transform.right) * fightAnimation.Evaluate(curveEvaluationX) * fightAnimationDistanceMultiplier;

        // ideally this check would be gone...
        if (creature.enemy != null)
        {
            float angleDeg = MathExt.AngleBetweenTwoPoints(creature.enemy.Position, Position);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angleDeg), GameInput.deltaTime * 5f);
        }
    }
}
