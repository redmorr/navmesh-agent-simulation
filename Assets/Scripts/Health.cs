using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] private int healthPoints;

    public void DealDamage(int amount)
    {
        healthPoints -= amount;
    }
}
