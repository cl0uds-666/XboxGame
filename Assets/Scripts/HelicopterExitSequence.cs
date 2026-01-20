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
    public string playerTag = "Player"; 
    public bool parentPlayers = true;
    public Transform playerMount;


    [Header("Scene")]
    public string finalSceneName = "Win"; 

    private bool started = false;
    private Transform[] snappedPlayers;
    private Transform activeMount;

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
            activeMount = playerMount != null ? playerMount : transform;
            snappedPlayers = new Transform[allPlayers.Length];

            for (int i = 0; i < allPlayers.Length; i++)
            {
                GameObject player = allPlayers[i];
                Transform playerTransform = player.transform;
                snappedPlayers[i] = playerTransform;

                DisablePlayerControl(player);

                playerTransform.SetParent(activeMount, false);
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
                SnapPlayersToMount();
                Vector3 dir = (target.position - transform.position);
                Vector3 step = dir.normalized * moveSpeed * Time.deltaTime;

                transform.position = transform.position + step;

                // rotate toward movement direction
                Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
                if (flatDir.sqrMagnitude > 0.001f)
                {
                    Quaternion look = Quaternion.LookRotation(flatDir.normalized, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);
                }

                yield return null;
            }

            transform.position = target.position;
            SnapPlayersToMount();
        }

        // done
        SceneManager.LoadScene(finalSceneName);
    }

    private void SnapPlayersToMount()
    {
        if (!parentPlayers || snappedPlayers == null || activeMount == null)
        {
            return;
        }

        for (int i = 0; i < snappedPlayers.Length; i++)
        {
            Transform playerTransform = snappedPlayers[i];
            if (playerTransform == null) continue;

            playerTransform.position = activeMount.position;
            playerTransform.rotation = activeMount.rotation;
        }
    }

    private void DisablePlayerControl(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        PlayerInputNew inputNew = player.GetComponent<PlayerInputNew>();
        if (inputNew != null)
        {
            inputNew.enabled = false;
        }

        UnityEngine.InputSystem.PlayerInput input = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (input != null)
        {
            input.enabled = false;
        }

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }
    }
}
