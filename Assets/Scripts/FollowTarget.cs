using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    [Header("Follow")]
    public Vector3 localOffset = new Vector3(0f, 14f, -8f);
    public float followSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.TransformPoint(localOffset);
        transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);

        // Look slightly ahead of the player so it's not staring at their feet
        Vector3 lookPoint = target.position + target.forward * 2f;
        transform.LookAt(lookPoint);
    }
}
