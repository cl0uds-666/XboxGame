using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 8f;
    public int enemiesPerWave = 4;
    public float spawnDelayInWave = 0.4f;

    [Header("Limits")]
    public int maxAlive = 12;

    [Header("Safety")]
    public float minDistanceFromPlayers = 6f;  // don't spawn too close
    public int triesPerEnemy = 10;             // how many spawn points we try before giving up

    private int alive = 0;
    private bool isSpawningWave = false;

    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            // wait until we have room
            while (alive >= maxAlive)
                yield return null;

            // do a wave
            if (!isSpawningWave)
                yield return StartCoroutine(SpawnWave());

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawningWave = true;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            if (alive >= maxAlive) break;

            bool spawned = TrySpawnOne();
            if (spawned)
                yield return new WaitForSeconds(spawnDelayInWave);
            else
                yield return null; // couldn't find a safe point this frame, try next enemy quickly
        }

        isSpawningWave = false;
    }

    bool TrySpawnOne()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
            return false;

        Transform[] players = FindPlayers();
        if (players.Length == 0)
            return false; // no players yet

        for (int t = 0; t < triesPerEnemy; t++)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (IsFarEnoughFromPlayers(sp.position, players))
            {
                GameObject e = Instantiate(enemyPrefab, sp.position, sp.rotation);

                EnemyHealth eh = e.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    eh.onDeath += () => { alive--; };
                }

                alive++;
                return true;
            }
        }

        // couldn't find a safe spawn point
        return false;
    }

    Transform[] FindPlayers()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");

        List<Transform> list = new List<Transform>();
        for (int i = 0; i < gos.Length; i++)
        {
            list.Add(gos[i].transform);
        }
        return list.ToArray();
    }

    bool IsFarEnoughFromPlayers(Vector3 spawnPos, Transform[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (Vector3.Distance(spawnPos, players[i].position) < minDistanceFromPlayers)
                return false;
        }
        return true;
    }
}
