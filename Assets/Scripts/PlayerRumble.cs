using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRumble : MonoBehaviour
{
    private Gamepad pad;
    private Coroutine rumbleRoutine;

    void Awake()
    {
        // Grab the gamepad paired to THIS player (important for splitscreen)
        PlayerInput pi = GetComponent<PlayerInput>();
        if (pi != null)
        {
            pad = pi.GetDevice<Gamepad>();
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

    void OnDisable()
    {
        if (pad != null) pad.SetMotorSpeeds(0f, 0f);
    }

    void OnDestroy()
    {
        if (pad != null) pad.SetMotorSpeeds(0f, 0f);
    }
}
