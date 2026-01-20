using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Fire Settings")]
    public float damage = 25f;
    public float range = 50f;
    public float fireRate = 8f; // shots per second

    [Header("Origin")]
    public Transform firePoint; 

    [Header("Audio")]
    public AudioSource shootAudio;


    private float nextFireTime;

    public void TryFire()
    {

        if (shootAudio != null)
        {
            shootAudio.Play();
        }

        GetComponent<PlayerRumble>()?.Rumble(0.2f, 0.6f, 0.08f);

        Vector3 origin = transform.position + Vector3.up * 1.0f;   // locked height
        Vector3 dir = transform.forward;
        dir.y = 0f;
        dir.Normalize();

        float radius = 0.4f; // forgiveness 

        Debug.Log($"{name}: Firing from {origin} in direction {dir} (radius {radius})");
        Debug.DrawRay(origin, dir * range, Color.red, 0.2f);

        if (Physics.SphereCast(origin, radius, dir, out RaycastHit hit, range))
        {
            Debug.Log($"{name}: Hit {hit.collider.name} at {hit.point}");

            EnemyHealth eh = hit.collider.GetComponentInParent<EnemyHealth>();
            if (eh != null)
            {
                Debug.Log($"{name}: EnemyHealth found, applying damage");
                eh.TakeDamage(damage);
            }
            else
            {
                Debug.Log($"{name}: Hit object has NO EnemyHealth");
            }
        }
        else
        {
            Debug.Log($"{name}: SphereCast did NOT hit anything");
        }
    }
}
