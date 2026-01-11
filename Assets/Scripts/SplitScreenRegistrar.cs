using UnityEngine;
using UnityEngine.InputSystem;

public class SplitScreenRegistrar : MonoBehaviour
{
    public Camera[] cameras;      // Cam1..Cam4
    public Transform[] players;   // auto-filled

    void Awake()
    {
        if (players == null || players.Length != cameras.Length)
            players = new Transform[cameras.Length];
    }

    // Called automatically by PlayerInputManager (Send Messages)
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex; // 0..3

        if (index < 0 || index >= cameras.Length)
        {
            Debug.LogWarning($"Player index {index} out of range");
            return;
        }

        players[index] = playerInput.transform;

        // THIS is the important bit
        FollowTarget follow = cameras[index].GetComponent<FollowTarget>();
        if (follow != null)
        {
            follow.target = players[index];
        }

        Debug.Log($"Camera {index + 1} now following {players[index].name}");
    }
}
