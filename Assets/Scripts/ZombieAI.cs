using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    private float nextPathUpdateTime = 0f;
    private float pathUpdateInterval = 0.2f;
    private bool isDead = false;

    public float randomOffsetRange = 2f;

    [Header("Targeting")]
    public string playerTag = "Player";
    public float retargetInterval = 0.5f;
    private float nextRetargetTime = 0f;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackDamage = 20f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Zombie Sounds")]
    public AudioSource audioSource;
    public AudioClip[] zombieSounds;
    public float minSoundInterval = 3f;
    public float maxSoundInterval = 8f;
    private float nextSoundTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Set first random sound time
        nextSoundTime = Time.time + Random.Range(minSoundInterval, maxSoundInterval);
    }

    void Update()
    {
        if (isDead || agent == null || !agent.enabled) return;

        // Re-pick target occasionally (supports multiplayer)
        if (Time.time >= nextRetargetTime)
        {
            nextRetargetTime = Time.time + retargetInterval;
            target = FindNearestPlayer();
        }

        if (target != null && agent.enabled)
        {
            if (Time.time >= nextPathUpdateTime)
            {
                nextPathUpdateTime = Time.time + pathUpdateInterval;

                Vector3 randomOffset = new Vector3(
                    Random.Range(-randomOffsetRange, randomOffsetRange),
                    0,
                    Random.Range(-randomOffsetRange, randomOffsetRange)
                );

                Vector3 newTargetPosition = target.position + randomOffset;

                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(newTargetPosition);
                }
            }

            // Check if the zombie is close enough to attack
            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                TryAttack();
            }
        }

        PlayRandomZombieSound();
    }

    Transform FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        if (players == null || players.Length == 0) return null;

        Transform nearest = null;
        float bestDist = float.MaxValue;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;

            float d = Vector3.SqrMagnitude(players[i].transform.position - transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                nearest = players[i].transform;
            }
        }

        return nearest;
    }

    void TryAttack()
    {
        if (isDead) return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Debug.Log("Zombie Attacks! Damage: " + attackDamage);

            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

        }
    }

    void PlayRandomZombieSound()
    {
        if (Time.time >= nextSoundTime && zombieSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomClip = zombieSounds[Random.Range(0, zombieSounds.Length)];
            audioSource.PlayOneShot(randomClip);
            nextSoundTime = Time.time + Random.Range(minSoundInterval, maxSoundInterval);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null && agent.enabled)
        {
            agent.enabled = false;
        }

        this.enabled = false;
    }
}
