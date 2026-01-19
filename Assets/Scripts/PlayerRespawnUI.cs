using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerRespawnUI : MonoBehaviour
{
    [Header("UI Prefab")]
    public GameObject uiPrefab;

    [Header("UI Reference (auto)")]
    public TextMeshProUGUI respawnText;

    private PlayerHealth health;
    private PlayerInput playerInput;
    private Camera myCamera;
    private GameObject uiInstance;

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
        if (health == null || respawnText == null) return;

        bool show = health.isDead && health.canRespawn;
        if (respawnText.gameObject.activeSelf != show)
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

            respawnText = uiInstance.GetComponentInChildren<TextMeshProUGUI>(true);
            if (respawnText != null)
            {
                respawnText.gameObject.SetActive(false);
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
    }

}
