using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinZone : MonoBehaviour
{
    [Header("UI")]
    public GameObject uiPrefab;
    public TextMeshProUGUI statusText;

    [Header("Messages")]
    [TextArea(2, 3)]
    public string reviveTeammateMessage = "Revive your teammate before advancing";
    [TextArea(2, 3)]
    public string holdPositionMessage = "Hold down the position whilst awaiting your team";

    private HashSet<PlayerHealth> playersInside = new HashSet<PlayerHealth>();
    private readonly List<TextMeshProUGUI> statusTexts = new List<TextMeshProUGUI>();

    void Start()
    {
        EnsureStatusTexts();
    }

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
        EnsureStatusTexts();
        if (statusTexts.Count == 0) return;

        if (totalInside <= 0)
        {
            SetStatusActive(false);
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
            SetStatusActive(false);
            return;
        }

        SetStatusText(message);
        SetStatusActive(true);
    }

    private void EnsureStatusTexts()
    {
        if (statusTexts.Count > 0) return;

        if (statusText != null)
        {
            statusTexts.Add(statusText);
        }

        if (uiPrefab == null) return;

        SplitScreenRegistrar registrar = FindObjectOfType<SplitScreenRegistrar>();
        if (registrar != null && registrar.cameras != null && registrar.cameras.Length > 0)
        {
            for (int i = 0; i < registrar.cameras.Length; i++)
            {
                Camera cam = registrar.cameras[i];
                if (cam == null) continue;
                TextMeshProUGUI createdText = CreateTextForCamera(cam);
                if (createdText != null)
                {
                    statusTexts.Add(createdText);
                }
            }
        }
        else
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                TextMeshProUGUI createdText = CreateTextForCamera(mainCamera);
                if (createdText != null)
                {
                    statusTexts.Add(createdText);
                }
            }
        }
    }

    private TextMeshProUGUI CreateTextForCamera(Camera cam)
    {
        if (uiPrefab == null || cam == null) return null;

        GameObject uiInstance = Instantiate(uiPrefab);
        Canvas canvas = uiInstance.GetComponent<Canvas>();

        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = cam;
            canvas.planeDistance = 1f;
            canvas.targetDisplay = cam.targetDisplay;
            canvas.transform.SetParent(cam.transform, false);
        }
        else
        {
            uiInstance.transform.SetParent(cam.transform, false);
        }

        TextMeshProUGUI textComponent = uiInstance.GetComponentInChildren<TextMeshProUGUI>(true);
        if (textComponent != null)
        {
            textComponent.gameObject.SetActive(false);
        }

        return textComponent;
    }

    private void SetStatusText(string message)
    {
        for (int i = 0; i < statusTexts.Count; i++)
        {
            TextMeshProUGUI text = statusTexts[i];
            if (text == null) continue;

            if (text.text != message)
            {
                text.text = message;
            }
        }
    }

    private void SetStatusActive(bool isActive)
    {
        for (int i = 0; i < statusTexts.Count; i++)
        {
            TextMeshProUGUI text = statusTexts[i];
            if (text == null) continue;

            if (text.gameObject.activeSelf != isActive)
            {
                text.gameObject.SetActive(isActive);
            }
        }
    }
}
