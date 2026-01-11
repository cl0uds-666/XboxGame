using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerColourLight : MonoBehaviour
{
    public Light playerLight;

    public Color[] colors =
    {
        Color.yellow,
        Color.red,
        Color.blue,
        Color.green
    };

    void Start()
    {
        var pi = GetComponent<PlayerInput>();
        int index = pi != null ? pi.playerIndex : 0;

        if (playerLight != null && colors.Length > 0)
        {
            playerLight.color = colors[index % colors.Length];
        }
    }
}
