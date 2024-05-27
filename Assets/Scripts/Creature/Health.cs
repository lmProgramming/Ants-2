using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health
{
    public float health = 10;
    public float maxHealth = 10;

    public Creature creature;

    public void Heal()
    {
        health = maxHealth;
    }

    public bool GetDamaged(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            creature.Die();
            return true;
        }
        return false;
    }
}
