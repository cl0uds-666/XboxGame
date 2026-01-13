using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRevive : MonoBehaviour
{
    private PlayerHealth health;

    void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    public void OnRespawn(InputValue value)
    {
        if (value.Get<float>() < 0.5f) return;
        if (health == null) return;

        if (!health.isDead) return;

        if (!health.canRespawn)
        {
            Debug.Log($"{name}: Can't respawn yet — teammate must pick up gravestone.");
            return;
        }

        Respawn();
    }

    void Respawn()
    {
        health.isDead = false;
        health.canRespawn = false;
        health.currentHealth = health.maxHealth;

        // re-enable scripts
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].enabled = true;
        }

        Debug.Log($"{name}: Respawned!");
    }
}
