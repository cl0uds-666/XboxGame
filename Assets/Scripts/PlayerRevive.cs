using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRevive : MonoBehaviour
{
    private PlayerHealth health;
    private PlayerInput playerInput;

    void Awake()
    {
        health = GetComponent<PlayerHealth>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnRespawn(InputValue value)
    {
        if (value.Get<float>() < 0.5f) return;
        if (health == null) return;

        if (!health.isDead) return;

        if (!health.canRespawn)
        {
            Debug.Log($"{name}: Can't respawn yet — teammate must pick up gravestone.");
            return;
        }

        Respawn();
    }

    void Update()
    {
        if (health == null) return;
        if (!health.isDead || !health.canRespawn) return;

        if (IsRespawnPressed())
        {
            Respawn();
        }
    }

    private bool IsRespawnPressed()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            InputAction respawnAction = playerInput.actions.FindAction("Respawn", false);
            if (respawnAction != null && respawnAction.WasPressedThisFrame())
            {
                return true;
            }
        }

        if (playerInput == null) return false;

        foreach (InputDevice device in playerInput.devices)
        {
            if (device is Gamepad gamepad && gamepad.buttonSouth.wasPressedThisFrame)
            {
                return true;
            }

            if (device is Keyboard keyboard && keyboard.enterKey.wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }

    void Respawn()
    {
        health.ResetToInitialState();

        Debug.Log($"{name}: Respawned!");
    }
}
