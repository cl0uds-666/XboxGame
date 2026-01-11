using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Cameras (size 4)")]
    public Camera[] cameras = new Camera[4];

    [Header("Players (size 4)")]
    public Transform[] players = new Transform[4];

    [Header("How many players are currently active")]
    [Range(1, 4)]
    public int activePlayers = 2;

    void Start()
    {
        ApplySplitScreen();
    }

    void OnValidate()
    {
        // Live preview in editor when you change activePlayers
        ApplySplitScreen();
    }

    public void ApplySplitScreen()
    {
        // Enable only the cameras we need
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == null) continue;
            cameras[i].gameObject.SetActive(i < activePlayers);
        }

        // Assign targets + viewports
        for (int i = 0; i < activePlayers; i++)
        {
            if (cameras[i] == null) continue;

            // Assign follow target if possible
            var follow = cameras[i].GetComponent<FollowTarget>();
            if (follow != null && i < players.Length)
            {
                follow.target = players[i];
            }

            cameras[i].rect = GetViewportRect(i, activePlayers);
        }
    }

    Rect GetViewportRect(int index, int count)
    {
        // index: 0..count-1

        // 1 player = full screen
        if (count == 1)
            return new Rect(0f, 0f, 1f, 1f);

        // 2 players = vertical split (left/right)
        if (count == 2)
            return (index == 0)
                ? new Rect(0f, 0f, 0.5f, 1f)
                : new Rect(0.5f, 0f, 0.5f, 1f);

        // 3 players = 2 on top, 1 full width bottom
        if (count == 3)
        {
            if (index == 0) return new Rect(0f, 0.5f, 0.5f, 0.5f);
            if (index == 1) return new Rect(0.5f, 0.5f, 0.5f, 0.5f);
            return new Rect(0f, 0f, 1f, 0.5f);
        }

        // 4 players = 2x2 grid
        // Top row: 0,1  Bottom row: 2,3
        if (index == 0) return new Rect(0f, 0.5f, 0.5f, 0.5f);
        if (index == 1) return new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        if (index == 2) return new Rect(0f, 0f, 0.5f, 0.5f);
        return new Rect(0.5f, 0f, 0.5f, 0.5f);
    }
}
