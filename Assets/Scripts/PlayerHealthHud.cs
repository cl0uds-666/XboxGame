using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthHud : MonoBehaviour
{
    [Header("Players")]
    public string playerTag = "Player";
    public int maxPlayers = 4;

    [Header("Layout")]
    public Vector2 startOffset = new Vector2(16f, -16f);
    public float lineSpacing = 28f;
    public int fontSize = 24;

    private readonly List<PlayerHealth> players = new List<PlayerHealth>();
    private readonly List<TextMeshProUGUI> labels = new List<TextMeshProUGUI>();
    private RectTransform container;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        if (FindObjectOfType<PlayerHealthHud>() != null)
        {
            return;
        }

        GameObject hud = new GameObject("PlayerHealthHud");
        hud.AddComponent<PlayerHealthHud>();
    }

    private void Awake()
    {
        BuildCanvas();
        RefreshPlayers();
        RefreshLabels();
    }

    private void Update()
    {
        if (RefreshPlayers())
        {
            RefreshLabels();
        }

        UpdateLabels();
    }

    private void BuildCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
        }

        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        GameObject containerObject = new GameObject("PlayerHealthContainer");
        containerObject.transform.SetParent(transform, false);
        container = containerObject.AddComponent<RectTransform>();
        container.anchorMin = new Vector2(0f, 1f);
        container.anchorMax = new Vector2(0f, 1f);
        container.pivot = new Vector2(0f, 1f);
        container.anchoredPosition = startOffset;
    }

    private bool RefreshPlayers()
    {
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag(playerTag);
        List<PlayerHealth> sorted = new List<PlayerHealth>(foundPlayers.Length);

        for (int i = 0; i < foundPlayers.Length; i++)
        {
            PlayerHealth health = foundPlayers[i].GetComponent<PlayerHealth>();
            if (health != null)
            {
                sorted.Add(health);
            }
        }

        sorted.Sort((a, b) => GetPlayerIndex(a).CompareTo(GetPlayerIndex(b)));

        bool changed = sorted.Count != players.Count;
        if (!changed)
        {
            for (int i = 0; i < sorted.Count; i++)
            {
                if (sorted[i] != players[i])
                {
                    changed = true;
                    break;
                }
            }
        }

        if (changed)
        {
            players.Clear();
            players.AddRange(sorted);
        }

        return changed;
    }

    private int GetPlayerIndex(PlayerHealth health)
    {
        PlayerIdentity identity = health.GetComponent<PlayerIdentity>();
        if (identity != null && identity.playerIndex >= 0)
        {
            return identity.playerIndex;
        }

        UnityEngine.InputSystem.PlayerInput input = health.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (input != null)
        {
            return input.playerIndex;
        }

        return 0;
    }

    private void RefreshLabels()
    {
        int targetCount = Mathf.Min(players.Count, maxPlayers);

        for (int i = labels.Count - 1; i >= targetCount; i--)
        {
            if (labels[i] != null)
            {
                Destroy(labels[i].gameObject);
            }

            labels.RemoveAt(i);
        }

        for (int i = labels.Count; i < targetCount; i++)
        {
            labels.Add(CreateLabel(i));
        }
    }

    private TextMeshProUGUI CreateLabel(int index)
    {
        GameObject labelObject = new GameObject($"PlayerHealthText_{index + 1}");
        labelObject.transform.SetParent(container, false);

        RectTransform rect = labelObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(0f, -index * lineSpacing);
        rect.sizeDelta = new Vector2(400f, lineSpacing);

        TextMeshProUGUI text = labelObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.text = "Player";

        return text;
    }

    private void UpdateLabels()
    {
        int count = Mathf.Min(players.Count, labels.Count);

        for (int i = 0; i < count; i++)
        {
            PlayerHealth health = players[i];
            TextMeshProUGUI label = labels[i];
            if (health == null || label == null)
            {
                continue;
            }

            int playerNumber = GetPlayerIndex(health) + 1;
            string status = health.isDead ? "DEAD" : $"{Mathf.CeilToInt(health.currentHealth)}/{Mathf.CeilToInt(health.maxHealth)}";
            label.text = $"P{playerNumber} HP: {status}";

            PlayerIdentity identity = health.GetComponent<PlayerIdentity>();
            label.color = identity != null ? identity.playerColor : Color.white;
        }
    }
}
