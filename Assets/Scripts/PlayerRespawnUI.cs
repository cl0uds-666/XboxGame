using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerRespawnUI : MonoBehaviour
{
    [Header("UI Prefab")]
    public GameObject uiPrefab;

    [Header("UI Reference (auto)")]
    public TextMeshProUGUI respawnText;
    public TextMeshProUGUI winStatusText;
    public TextMeshProUGUI healthText;

    [Header("Health HUD")]
    public Vector2 healthOffset = new Vector2(16f, -16f);
    public int healthFontSize = 24;

    [Header("Messages")]
    [TextArea(2, 3)]
    public string respawnMessage = "Press A to Respawn";
    [TextArea(2, 3)]
    public string gameOverMessage = "Press A to Restart\nPress B to return to Main Menu";

    private PlayerHealth health;
    private PlayerInput playerInput;
    private Camera myCamera;
    private GameObject uiInstance;
    private static bool gameOverTriggered;

    void Awake()
    {
        health = GetComponent<PlayerHealth>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        // Only try to find a child camera if registrar hasn't assigned one already
        if (myCamera == null)
        {
            myCamera = GetComponentInChildren<Camera>();
            if (myCamera == null)
            {
                Debug.LogWarning($"{name}: No camera found in children. If you use scene cameras, assign via registrar.");
            }
        }

        EnsureUi();
    }


    void Update()
    {
        if (health == null) return;

        UpdateHealthText();

        bool allDead = AreAllPlayersDead();
        if (allDead)
        {
            if (respawnText.text != gameOverMessage)
            {
                respawnText.text = gameOverMessage;
            }

            if (!respawnText.gameObject.activeSelf)
            {
                respawnText.gameObject.SetActive(true);
            }

            if (!gameOverTriggered)
            {
                if (IsRestartPressed())
                {
                    gameOverTriggered = true;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else if (IsMainMenuPressed())
                {
                    gameOverTriggered = true;
                    SceneManager.LoadScene(0);
                }
            }

            return;
        }

        if (gameOverTriggered)
        {
            gameOverTriggered = false;
        }

        bool show = respawnText != null && health.isDead && health.canRespawn;
        if (show && respawnText.text != respawnMessage)
        {
            respawnText.text = respawnMessage;
        }
        if (respawnText != null && respawnText.gameObject.activeSelf != show)
        {
            respawnText.gameObject.SetActive(show);
        }
    }

    public void SetCamera(Camera cam)
    {
        myCamera = cam;

        EnsureUi();
    }

    private void EnsureUi()
    {
        if (uiInstance == null && uiPrefab != null && myCamera != null)
        {
            uiInstance = Instantiate(uiPrefab);
            Canvas canvas = uiInstance.GetComponent<Canvas>();

            // Force correct mode even if the prefab is Overlay
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = myCamera;
                canvas.planeDistance = 1f; // helps avoid weird depth behaviour
                canvas.targetDisplay = myCamera.targetDisplay;
                canvas.transform.SetParent(myCamera.transform, false);
            }
            else
            {
                uiInstance.transform.SetParent(myCamera.transform, false);
            }

            CacheUiTextReferences(uiInstance);
            if (respawnText != null)
            {
                if (string.IsNullOrWhiteSpace(respawnMessage))
                {
                    respawnMessage = respawnText.text;
                }
                respawnText.gameObject.SetActive(false);
            }
            if (winStatusText != null)
            {
                winStatusText.gameObject.SetActive(false);
            }
        }

        Canvas existingCanvas = respawnText != null ? respawnText.GetComponentInParent<Canvas>() : null;
        if (existingCanvas != null && myCamera != null)
        {
            existingCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            existingCanvas.worldCamera = myCamera;
            existingCanvas.targetDisplay = myCamera.targetDisplay;
            existingCanvas.transform.SetParent(myCamera.transform, false);
        }

        if (existingCanvas != null)
        {
            CacheUiTextReferences(existingCanvas.gameObject);
        }

        if (healthText == null)
        {
            CreateHealthText();
        }
    }

    private bool AreAllPlayersDead()
    {
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        if (players.Length == 0) return false;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null && !players[i].isDead)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsRestartPressed()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            if (device is Gamepad gamepad && gamepad.buttonSouth.wasPressedThisFrame)
            {
                return true;
            }

            if (device is Keyboard keyboard && (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsMainMenuPressed()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            if (device is Gamepad gamepad && gamepad.buttonEast.wasPressedThisFrame)
            {
                return true;
            }

            if (device is Keyboard keyboard && keyboard.escapeKey.wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }

    private void CacheUiTextReferences(GameObject root)
    {
        if (root == null)
        {
            return;
        }

        TextMeshProUGUI[] texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            TextMeshProUGUI text = texts[i];
            if (text == null)
            {
                continue;
            }

            if (text.gameObject.name == "Respawn")
            {
                respawnText = text;
            }
            else if (text.gameObject.name == "WinStatus")
            {
                winStatusText = text;
            }
            else if (text.gameObject.name == "Health")
            {
                healthText = text;
            }
        }

        if (respawnText == null && texts.Length > 0)
        {
            respawnText = texts[0];
        }
    }

    private void CreateHealthText()
    {
        Transform parentTransform = null;

        if (respawnText != null)
        {
            parentTransform = respawnText.transform.parent;
        }
        else if (uiInstance != null)
        {
            parentTransform = uiInstance.transform;
        }
        else if (myCamera != null)
        {
            parentTransform = myCamera.transform;
        }

        if (parentTransform == null)
        {
            return;
        }

        // UI not working again, make sure to change

        GameObject healthObject = new GameObject("Health");
        healthObject.transform.SetParent(parentTransform, false);

        RectTransform rect = healthObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = healthOffset;
        rect.sizeDelta = new Vector2(300f, 40f);

        healthText = healthObject.AddComponent<TextMeshProUGUI>();
        healthText.fontSize = healthFontSize;
        healthText.alignment = TextAlignmentOptions.TopLeft;
    }

    private void UpdateHealthText()
    {
        if (healthText == null)
        {
            return;
        }

        string status = health.isDead
            ? "DEAD"
            : $"{Mathf.CeilToInt(health.currentHealth)}/{Mathf.CeilToInt(health.maxHealth)}";

        healthText.text = $"HP: {status}";

        PlayerIdentity identity = GetComponent<PlayerIdentity>();
        if (identity != null)
        {
            healthText.color = identity.playerColor;
        }
        else
        {
            healthText.color = Color.white;
        }
    }

}
