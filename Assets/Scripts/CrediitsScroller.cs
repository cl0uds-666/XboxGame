using UnityEngine;
using TMPro;

public class CreditsScroller : MonoBehaviour
{
    [Header("Target")]
    public RectTransform creditsRect;     // the RectTransform of the scrolling text

    [Header("Scroll Settings")]
    public float scrollSpeed = 60f;       // pixels per second
    public float startY = -600f;          // where it starts (off screen)
    public float endY = 600f;             // where it ends (off screen)

    [Header("Behaviour")]
    public bool loop = true;

    void OnEnable()
    {
        if (creditsRect == null)
            creditsRect = GetComponent<RectTransform>();

        // Reset to start whenever credits panel opens
        Vector2 pos = creditsRect.anchoredPosition;
        pos.y = startY;
        creditsRect.anchoredPosition = pos;
    }

    void Update()
    {
        if (creditsRect == null) return;

        creditsRect.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);

        if (creditsRect.anchoredPosition.y >= endY)
        {
            if (loop)
            {
                Vector2 pos = creditsRect.anchoredPosition;
                pos.y = startY;
                creditsRect.anchoredPosition = pos;
            }
            else
            {
                enabled = false;
            }
        }
    }
}
