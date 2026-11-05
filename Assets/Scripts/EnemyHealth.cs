using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    private int currentHealth;

    private void OnEnable()
    {
        //Initialize the enemy's health when it is enabled
        currentHealth = enemyData.health;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Handle enemy death (e.g., play death animation, drop loot, etc.)
        gameObject.SetActive(false);
    }
}
