using UnityEngine;

public class LightFlasher : MonoBehaviour
{
    public Light targetLight;
    public float flashInterval = 0.5f;

    private float timer;

    void Update()
    {
        if (targetLight == null) return;

        timer += Time.deltaTime;

        if (timer >= flashInterval)
        {
            targetLight.enabled = !targetLight.enabled;
            timer = 0f;
        }
    }
}
