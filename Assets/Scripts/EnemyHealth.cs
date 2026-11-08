using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;

    private int currentHealth;

    private void OnEnable()
    {
        // Restore health when leaving the pool. Start does not run again.
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
        // The explosion stays in the world and the enemy goes back to the pool.
        CombatVfx.PlayExplosion(transform.position);
        gameObject.SetActive(false);
    }
}
