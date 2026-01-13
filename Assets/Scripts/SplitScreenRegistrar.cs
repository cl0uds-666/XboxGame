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

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex; 

        if (index < 0 || index >= cameras.Length)
        {
            Debug.LogWarning($"Player index {index} out of range");
            return;
        }

        var id = playerInput.GetComponent<PlayerIdentity>();
        if (id != null)
        {
            id.playerIndex = index;
        }

        PlayerRespawnUI ui = playerInput.GetComponent<PlayerRespawnUI>();
        if (ui != null)
        {
            ui.SetCamera(cameras[index]);
        }

        players[index] = playerInput.transform;

        // Find FollowTarget even if it's on a parent/child rig object
        FollowTarget follow = cameras[index].GetComponentInParent<FollowTarget>();
        if (follow == null)
            follow = cameras[index].GetComponentInChildren<FollowTarget>();

        if (follow != null)
        {
            follow.target = players[index];
            Debug.Log($"Cam {index + 1}: FollowTarget found on '{follow.gameObject.name}', now following '{players[index].name}'");
        }
        else
        {
            Debug.LogWarning($"Cam {index + 1}: NO FollowTarget found on camera object/parent/children. Add FollowTarget to the camera rig.");
        }

        Debug.Log("OnPlayerJoined fired!");
    }

}
