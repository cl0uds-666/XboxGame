using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;

    public Action onDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        onDeath?.Invoke();

        
        ZombieAI ai = GetComponent<ZombieAI>();
        if (ai != null)
        {
            ai.Die();
        }

        Destroy(gameObject);
    }
}
