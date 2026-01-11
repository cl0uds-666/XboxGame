using UnityEngine;

public class PlayerSpinAim : MonoBehaviour
{
    [Header("Spin Buttons")]
    public string spinLeftButton = "SpinLeft";
    public string spinRightButton = "SpinRight";

    [Header("Settings")]
    public float spinSpeed = 180f;

    void Update()
    {
        float spin = 0f;

        if (Input.GetButton(spinLeftButton)) spin -= 1f;
        if (Input.GetButton(spinRightButton)) spin += 1f;

        if (spin != 0f)
        {
            float amount = spin * spinSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * amount);

            Debug.Log($"{name}: Rotating {amount} degrees this frame");
        }
    }
}
