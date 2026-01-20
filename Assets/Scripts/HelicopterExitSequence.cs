using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelicopterExitSequence : MonoBehaviour
{
    [Header("Path")]
    public Transform[] points;          // Waypoints in order
    public float moveSpeed = 6f;        // units per second
    public float rotateSpeed = 6f;      // how fast it turns to face direction

    [Header("Players")]
    public string playerTag = "Player"; // your players are tagged Player
    public bool parentPlayers = true;
    public Transform playerMount;


    [Header("Scene")]
    public string finalSceneName = "Win"; // change to your final scene name

    private bool started = false;

    public void StartSequence()
    {
        if (started) return;
        started = true;

        StartCoroutine(DoSequence());
    }

    private IEnumerator DoSequence()
    {
        // parent players so cameras follow along
        if (parentPlayers)
        {
            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag(playerTag);
            Transform mount = playerMount != null ? playerMount : transform;

            for (int i = 0; i < allPlayers.Length; i++)
            {
                Transform playerTransform = allPlayers[i].transform;
                playerTransform.SetParent(mount, false);
                playerTransform.localPosition = Vector3.zero;
                playerTransform.localRotation = Quaternion.identity;
            }
        }

        // move along points
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning("HelicopterExitSequence: No points assigned.");
            SceneManager.LoadScene(finalSceneName);
            yield break;
        }

        for (int i = 0; i < points.Length; i++)
        {
            Transform target = points[i];
            if (target == null) continue;

            while (Vector3.Distance(transform.position, target.position) > 0.05f)
            {
                Vector3 dir = (target.position - transform.position);
                Vector3 step = dir.normalized * moveSpeed * Time.deltaTime;

                transform.position = transform.position + step;

                // rotate toward movement direction (optional but looks nicer)
                Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
                if (flatDir.sqrMagnitude > 0.001f)
                {
                    Quaternion look = Quaternion.LookRotation(flatDir.normalized, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
                }

                yield return null;
            }

            transform.position = target.position;
        }

        // done
        SceneManager.LoadScene(finalSceneName);
    }
}
