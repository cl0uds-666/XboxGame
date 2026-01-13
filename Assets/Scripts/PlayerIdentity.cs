using UnityEngine;

public class PlayerIdentity : MonoBehaviour
{
    public int playerIndex = -1;
    public Color playerColor = Color.white;

    [Header("existing spotlight on the player")]
    public Light playerSpotlight;

    public void ApplyColor(Color c)
    {
        playerColor = c;

        if (playerSpotlight != null)
            playerSpotlight.color = playerColor;
    }
}
