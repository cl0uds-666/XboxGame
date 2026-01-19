using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinZone : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI statusText;

    [Header("Messages")]
    [TextArea(2, 3)]
    public string reviveTeammateMessage = "Revive your teammate before advancing";
    [TextArea(2, 3)]
    public string holdPositionMessage = "Hold down the position whilst awaiting your team";

    private HashSet<PlayerHealth> playersInside = new HashSet<PlayerHealth>();
    private readonly List<TextMeshProUGUI> statusTargets = new List<TextMeshProUGUI>();

    void Update()
    {
        CheckWin();
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth ph = other.GetComponentInParent<PlayerHealth>();
        if (ph != null)
        {
            playersInside.Add(ph);
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
        if (allPlayers.Length == 0) return;

        int totalPlayers = 0;
        int totalInside = 0;
        bool deadPlayerOutside = false;
        bool alivePlayerOutside = false;

        for (int i = 0; i < allPlayers.Length; i++)
        {
            PlayerHealth ph = allPlayers[i].GetComponent<PlayerHealth>();
            if (ph == null) continue;

            totalPlayers++;
            bool isInside = playersInside.Contains(ph);

            if (isInside)
            {
                totalInside++;
            }
            else
            {
                if (ph.isDead)
                {
                    deadPlayerOutside = true;
                }
                else
                {
                    alivePlayerOutside = true;
                }
            }
        }

        UpdateStatus(deadPlayerOutside, alivePlayerOutside, totalInside);

        if (totalPlayers > 0 && totalInside == totalPlayers)
        {
            Debug.Log("WIN! All players are in the zone.");
            SceneManager.LoadScene("Win");
        }
    }

    private void UpdateStatus(bool deadPlayerOutside, bool alivePlayerOutside, int totalInside)
    {
        List<TextMeshProUGUI> targets = GetStatusTargets();
        if (targets.Count == 0) return;

        if (totalInside <= 0)
        {
            SetTargetsActive(targets, false, null);
            return;
        }

        string message = null;

        if (deadPlayerOutside)
        {
            message = reviveTeammateMessage;
        }
        else if (alivePlayerOutside)
        {
            message = holdPositionMessage;
        }

        if (string.IsNullOrEmpty(message))
        {
            SetTargetsActive(targets, false, null);
            return;
        }

        SetTargetsActive(targets, true, message);
    }

    private List<TextMeshProUGUI> GetStatusTargets()
    {
        statusTargets.Clear();

        if (statusText != null)
        {
            statusTargets.Add(statusText);
        }

        PlayerRespawnUI[] uiInstances = FindObjectsOfType<PlayerRespawnUI>(true);
        for (int i = 0; i < uiInstances.Length; i++)
        {
            PlayerRespawnUI ui = uiInstances[i];
            if (ui == null || ui.winStatusText == null)
            {
                continue;
            }

            if (!statusTargets.Contains(ui.winStatusText))
            {
                statusTargets.Add(ui.winStatusText);
            }
        }

        return statusTargets;
    }

    private void SetTargetsActive(List<TextMeshProUGUI> targets, bool isActive, string message)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            TextMeshProUGUI target = targets[i];
            if (target == null)
            {
                continue;
            }

            if (!string.IsNullOrEmpty(message) && target.text != message)
            {
                target.text = message;
            }

            if (target.gameObject.activeSelf != isActive)
            {
                target.gameObject.SetActive(isActive);
            }
        }
    }
}
