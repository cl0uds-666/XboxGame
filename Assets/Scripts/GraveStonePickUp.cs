using UnityEngine;

public class GraveStonePickUp : MonoBehaviour
{
    public Light spotlight;

    private PlayerHealth ownerHealth;
    private int ownerIndex = -1;

    public void SetOwner(PlayerHealth owner)
    {
        ownerHealth = owner;

        // use the same index colour logic already used via PlayerInput
        var pi = owner.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        ownerIndex = pi != null ? pi.playerIndex : 0;

        // copy the player's light colour directly
        var pcl = owner.GetComponent<PlayerColourLight>();
        if (spotlight != null)
        {
            if (pcl != null && pcl.playerLight != null)
                spotlight.color = pcl.playerLight.color;   // exact same colour
            else if (pcl != null && pcl.colors.Length > 0)
                spotlight.color = pcl.colors[ownerIndex % pcl.colors.Length];
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth picker = other.GetComponentInParent<PlayerHealth>();
        if (picker == null) return;
        if (picker.isDead) return;

        // prevent owner picking up their own stone
        // causes slight bug where player sometimes starts floating
        var pickerPI = picker.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        int pickerIndex = pickerPI != null ? pickerPI.playerIndex : -999;
        if (pickerIndex == ownerIndex) return;

        if (ownerHealth != null && ownerHealth.isDead)
        {
            ownerHealth.canRespawn = true;
            Debug.Log($"Gravestone picked up! Player {ownerIndex + 1} can now press A to respawn.");
        }

        Destroy(gameObject);
    }
}
