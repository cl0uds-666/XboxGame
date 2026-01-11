using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float currentHealth;

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
        // call ZombieAI.Die() here
        ZombieAI ai = GetComponent<ZombieAI>();
        if (ai != null)
        {
            ai.Die();
        }

        Destroy(gameObject);
    }
}
