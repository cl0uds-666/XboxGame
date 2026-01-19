using UnityEngine;

public class SpinY : MonoBehaviour
{
    public float speed = 90f; // degrees per second

    void Update()
    {
        transform.Rotate(0f, speed * Time.deltaTime, 0f);
    }
}
