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

        if (uiPrefab != null)
        {
            GameObject ui = Instantiate(uiPrefab);
            Canvas canvas = ui.GetComponent<Canvas>();

            // Force correct mode even if the prefab is Overlay
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = myCamera;
                canvas.planeDistance = 1f; // helps avoid weird depth behaviour
                if (myCamera != null)
                {
                    canvas.targetDisplay = myCamera.targetDisplay;
                }
            }
            if (myCamera != null)
            {
                ui.transform.SetParent(myCamera.transform, false);
            }

            respawnText = ui.GetComponentInChildren<TextMeshProUGUI>(true);
            if (respawnText != null)
            {
                respawnText.gameObject.SetActive(false);
            }
        }
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

        // if UI already exists, retarget it
        Canvas canvas = respawnText != null ? respawnText.GetComponentInParent<Canvas>() : null;
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = myCamera;
            if (myCamera != null)
            {
                canvas.targetDisplay = myCamera.targetDisplay;
            }
        }
        if (myCamera != null && canvas != null)
        {
            canvas.transform.SetParent(myCamera.transform, false);
        }
    }

}
