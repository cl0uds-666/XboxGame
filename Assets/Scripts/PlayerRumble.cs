using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRumble : MonoBehaviour
{
    private Gamepad pad;
    private Coroutine rumbleRoutine;
    private Coroutine heartbeatRoutine;
    private PlayerHealth health;

    [Header("Heartbeat Rumble")]
    public bool heartbeatEnabled = true;
    public float heartbeatStartHealth = 40f;
    public float heartbeatFastHealth = 20f;
    public float heartbeatPulseDuration = 0.12f;
    public float heartbeatSlowInterval = 1.2f;
    public float heartbeatFastInterval = 0.6f;
    public float heartbeatLowFrequency = 0.25f;
    public float heartbeatHighFrequency = 0.6f;

    void Awake()
    {
        // Grab the gamepad paired to THIS player (important for splitscreen)
        PlayerInput pi = GetComponent<PlayerInput>();
        if (pi != null)
        {
            pad = pi.GetDevice<Gamepad>();
        }

        health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (!heartbeatEnabled || pad == null || health == null || health.isDead)
        {
            StopHeartbeat();
            return;
        }

        if (health.currentHealth <= heartbeatStartHealth)
        {
            if (heartbeatRoutine == null)
            {
                heartbeatRoutine = StartCoroutine(HeartbeatLoop());
            }
        }
        else
        {
            StopHeartbeat();
        }
    }

    public void Rumble(float lowFrequency, float highFrequency, float duration)
    {
        if (pad == null) return;

        // stop any previous rumble so it doesn't stack weirdly
        if (rumbleRoutine != null) StopCoroutine(rumbleRoutine);

        rumbleRoutine = StartCoroutine(RumbleRoutine(lowFrequency, highFrequency, duration));
    }

    IEnumerator RumbleRoutine(float lowFrequency, float highFrequency, float duration)
    {
        pad.SetMotorSpeeds(lowFrequency, highFrequency);
        yield return new WaitForSeconds(duration);
        pad.SetMotorSpeeds(0f, 0f);
        rumbleRoutine = null;
    }

    IEnumerator HeartbeatLoop()
    {
        while (heartbeatEnabled && pad != null && health != null && !health.isDead && health.currentHealth <= heartbeatStartHealth)
        {
            float interval = health.currentHealth <= heartbeatFastHealth ? heartbeatFastInterval : heartbeatSlowInterval;
            pad.SetMotorSpeeds(heartbeatLowFrequency, heartbeatHighFrequency);
            yield return new WaitForSeconds(heartbeatPulseDuration);
            pad.SetMotorSpeeds(0f, 0f);

            float pause = Mathf.Max(0.01f, interval - heartbeatPulseDuration);
            yield return new WaitForSeconds(pause);
        }

        heartbeatRoutine = null;
        if (pad != null)
        {
            pad.SetMotorSpeeds(0f, 0f);
        }
    }

    private void StopHeartbeat()
    {
        if (heartbeatRoutine != null)
        {
            StopCoroutine(heartbeatRoutine);
            heartbeatRoutine = null;
        }

        if (pad != null)
        {
            pad.SetMotorSpeeds(0f, 0f);
        }
    }

    void OnDisable()
    {
        StopHeartbeat();
        if (pad != null) pad.SetMotorSpeeds(0f, 0f);
    }

    void OnDestroy()
    {
        StopHeartbeat();
        if (pad != null) pad.SetMotorSpeeds(0f, 0f);
    }
}
