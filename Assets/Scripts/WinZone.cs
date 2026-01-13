using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinZone : MonoBehaviour
{
    private HashSet<PlayerHealth> playersInside = new HashSet<PlayerHealth>();

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
        if (ph != null)
        {
            playersInside.Add(ph);
            CheckWin();
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
        if (ph != null)
        {
            playersInside.Remove(ph);
        }
    }

    void CheckWin()
    {
        // Find all players currently in the scene
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        int alivePlayers = 0;
        int aliveInside = 0;

        for (int i = 0; i < allPlayers.Length; i++)
        {
            PlayerHealth ph = allPlayers[i].GetComponent<PlayerHealth>();
            if (ph == null) continue;

            if (!ph.isDead)
            {
                alivePlayers++;
                if (playersInside.Contains(ph))
                    aliveInside++;
            }
        }

        if (alivePlayers > 0 && aliveInside == alivePlayers)
        {
            Debug.Log("WIN! All alive players are in the zone.");
            SceneManager.LoadScene("Win");
        }
    }
}
