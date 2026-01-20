using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pauseMenuRoot;
    [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.6f);
    [TextArea(2, 3)]
    [SerializeField] private string pauseMessage = "Paused\nPress Start or Esc to Resume";
    [TextArea(2, 3)]
    [SerializeField] private string disconnectMessage = "Controller disconnected\nReconnect to Resume";

    private TextMeshProUGUI messageText;
    private bool isPaused;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
        {
            return;
        }

        PauseMenuController existing = FindObjectOfType<PauseMenuController>();
        if (existing != null)
        {
            existing.EnsureUi();
            return;
        }

        GameObject root = new GameObject("PauseMenu");
        PauseMenuController controller = root.AddComponent<PauseMenuController>();
        controller.BuildDefaultUi();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        EnsureUi();
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += HandleDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }

    public void TogglePause()
    {
        SetPaused(!isPaused, false);
    }

    public void PauseForDisconnect()
    {
        SetPaused(true, true);
    }

    private void SetPaused(bool paused, bool disconnected)
    {
        isPaused = paused;

        if (pauseMenuRoot != null)
        {
            pauseMenuRoot.SetActive(paused);
        }

        if (messageText != null)
        {
            messageText.text = paused
                ? (disconnected ? disconnectMessage : pauseMessage)
                : pauseMessage;
        }

        Time.timeScale = paused ? 0f : 1f;
    }

    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is not Gamepad)
        {
            return;
        }

        if (change == InputDeviceChange.Disconnected || change == InputDeviceChange.Removed)
        {
            PauseForDisconnect();
        }
    }

    private void EnsureUi()
    {
        if (pauseMenuRoot == null)
        {
            BuildDefaultUi();
        }

        if (pauseMenuRoot == null)
        {
            return;
        }

        Canvas canvas = pauseMenuRoot.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.targetDisplay = 0;
        }

        RectTransform rectTransform = pauseMenuRoot.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        if (messageText == null)
        {
            messageText = pauseMenuRoot.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (pauseMenuRoot.activeSelf)
        {
            pauseMenuRoot.SetActive(false);
        }
    }

    private void BuildDefaultUi()
    {
        GameObject canvasObject = new GameObject("PauseMenuCanvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;

        GameObject panelObject = new GameObject("Panel");
        panelObject.transform.SetParent(canvasObject.transform, false);

        RectTransform panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelObject.AddComponent<Image>();
        panelImage.color = overlayColor;

        GameObject textObject = new GameObject("PauseText");
        textObject.transform.SetParent(panelObject.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(900f, 300f);

        messageText = textObject.AddComponent<TextMeshProUGUI>();
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.fontSize = 48;
        messageText.text = pauseMessage;
        messageText.color = Color.white;

        pauseMenuRoot = canvasObject;
        pauseMenuRoot.SetActive(false);
    }
}
