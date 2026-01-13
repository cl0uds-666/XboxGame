using UnityEngine;

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


    void Awake()
    {
        currentHealth = maxHealth;
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

        SpawnGravestoneAtMyPosition();
        if (deathAudio != null)
        {
            deathAudio.Play();
        }


        if (isDead) return;
        isDead = true;

        Debug.Log($"{name} died.");

        // simplest "death" for now:
        // disable movement/shoot scripts so they stop acting
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        for (int i = 0; i < scripts.Length; i++)
        {
            if (scripts[i] != this)
                scripts[i].enabled = false;
        }
    }
}
