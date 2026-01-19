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
            InputAction respawnAction = playerInput.actions.FindAction("Respawn", true);
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
        health.isDead = false;
        health.canRespawn = false;
        health.currentHealth = health.maxHealth;

        // re-enable scripts
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        for (int i = 0; i < scripts.Length; i++)
        {
            scripts[i].enabled = true;
        }

        Debug.Log($"{name}: Respawned!");
    }
}
