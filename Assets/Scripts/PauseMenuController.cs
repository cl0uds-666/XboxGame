using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    private const string PlayerUiName = "PlayerUI";
    private const string PauseMenuName = "PauseMenu";
    private const string PauseMessageName = "PauseMessage";

    public static PauseMenuController Instance { get; private set; }

    [Header("UI")]
    public GameObject pauseMenuUI;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.6f);
    [TextArea(2, 3)]
    [SerializeField] private string pauseMessage = "Paused\nPress Start or Esc to Resume";
    [TextArea(2, 3)]
    [SerializeField] private string disconnectMessage = "Controller disconnected\nReconnect to Resume";

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
        controller.BuildFallbackUi();
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
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
        SetMessageText(pauseMessage);
    }

    public void Pause()
    {
        SetMessageText(pauseMessage);

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void PauseForDisconnect()
    {
        SetMessageText(disconnectMessage);

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
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
        if (pauseMenuUI == null)
        {
            TryUsePlayerUiPauseMenu();
        }

        if (pauseMenuUI == null)
        {
            BuildFallbackUi();
        }

        if (pauseMenuUI == null)
        {
            return;
        }

        Canvas canvas = pauseMenuUI.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.targetDisplay = 0;
        }

        RectTransform rectTransform = pauseMenuUI.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        if (messageText == null)
        {
            messageText = FindPauseMessageText();
            if (messageText == null)
            {
                messageText = CreatePauseMessageText();
            }
        }

        pauseMenuUI.SetActive(false);
    }

    private void TryUsePlayerUiPauseMenu()
    {
        GameObject playerUi = GameObject.Find(PlayerUiName);
        if (playerUi == null)
        {
            return;
        }

        Transform pauseMenuTransform = playerUi.transform.Find(PauseMenuName);
        if (pauseMenuTransform == null)
        {
            return;
        }

        pauseMenuUI = pauseMenuTransform.gameObject;
    }

    private TextMeshProUGUI FindPauseMessageText()
    {
        if (pauseMenuUI == null)
        {
            return null;
        }

        Transform messageTransform = pauseMenuUI.transform.Find(PauseMessageName);
        if (messageTransform == null)
        {
            return null;
        }

        return messageTransform.GetComponent<TextMeshProUGUI>();
    }

    private TextMeshProUGUI CreatePauseMessageText()
    {
        if (pauseMenuUI == null)
        {
            return null;
        }

        GameObject textObject = new GameObject(PauseMessageName);
        textObject.transform.SetParent(pauseMenuUI.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = new Vector2(0f, 160f);
        textRect.sizeDelta = new Vector2(900f, 200f);

        TextMeshProUGUI createdText = textObject.AddComponent<TextMeshProUGUI>();
        createdText.alignment = TextAlignmentOptions.Center;
        createdText.fontSize = 48;
        createdText.text = pauseMessage;
        createdText.color = Color.white;

        return createdText;
    }

    private void SetMessageText(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    private void BuildFallbackUi()
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

        pauseMenuUI = canvasObject;
        pauseMenuUI.SetActive(false);
    }
}
