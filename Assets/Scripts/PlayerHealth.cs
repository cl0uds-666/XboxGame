using UnityEngine;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Death")]
    public bool isDead = false;


    [Header("Audio")]
    public AudioSource hurtAudio;
    public AudioSource deathAudio;

    [Header("Revive")]
    public GameObject gravestonePrefab;
    public bool canRespawn = false;

    private readonly Dictionary<MonoBehaviour, bool> initialEnabledStates = new Dictionary<MonoBehaviour, bool>();
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    void Awake()
    {
        currentHealth = maxHealth;
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        CacheInitialStates();
    }

    private void CacheInitialStates()
    {
        initialEnabledStates.Clear();
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        for (int i = 0; i < scripts.Length; i++)
        {
            MonoBehaviour script = scripts[i];
            if (script == null)
            {
                continue;
            }

            initialEnabledStates[script] = script.enabled;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (hurtAudio != null)
        {
            hurtAudio.Play();
        }


        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }


    void SpawnGravestoneAtMyPosition()
    {
        if (gravestonePrefab == null) return;

        GameObject graveObj = Instantiate(gravestonePrefab, transform.position, Quaternion.identity);

        GraveStonePickUp grave = graveObj.GetComponent<GraveStonePickUp>();
        if (grave != null)
        {
            grave.SetOwner(this);
        }
    }


    void Die()
    {
        canRespawn = false;

        SpawnGravestoneAtMyPosition();
        if (deathAudio != null)
        {
            deathAudio.Play();
        }


        if (isDead) return;
        isDead = true;

        Debug.Log($"{name} died.");

        // disable movement/shoot scripts so they stop acting
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        for (int i = 0; i < scripts.Length; i++)
        {
            MonoBehaviour script = scripts[i];
            if (script == this) continue;
            if (script is PlayerRevive) continue;
            if (script is PlayerRespawnUI) continue;
            if (script is UnityEngine.InputSystem.PlayerInput) continue;

            script.enabled = false;
        }
    }

    public void ResetToInitialState()
    {
        isDead = false;
        canRespawn = false;
        currentHealth = maxHealth;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        foreach (KeyValuePair<MonoBehaviour, bool> entry in initialEnabledStates)
        {
            if (entry.Key == null)
            {
                continue;
            }

            entry.Key.enabled = entry.Value;
        }
    }
}
