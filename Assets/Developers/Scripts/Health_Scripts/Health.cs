using UnityEngine;

// Author: Salem
// Created: [23-11-2026]


// Script that manages the health of a game object and implements the IDamageable interface
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private NPCDamageFlash damageFlash;

    private void Awake()
    {
        currentHealth = maxHealth;
        damageFlash = GetComponent<NPCDamageFlash>();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
            return;

        currentHealth = Mathf.Max(
            0, currentHealth - damage
        );

        Debug.Log(
            $"{name}: HP {currentHealth}/{maxHealth}"
        );

        damageFlash?.Flash();

        if (currentHealth == 0)
            Die();
    }

    private void Die()
    {
        GetComponent<NPCDeath>()?.Die();
        Debug.Log($"{name} has died!");
    }
}
