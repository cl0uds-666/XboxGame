using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputNew : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement movement; // drag your existing script (or leave null)
    public PlayerShoot shoot;       // drag your PlayerShoot

    [Header("Turn Settings")]
    public float turnSpeed = 180f;

    private Vector2 moveInput;
    private float turnInput;

    void Update()
    {
        DoMove();
        DoTurn();
    }

    void DoMove()
    {
        float speed = movement != null ? movement.speed : 6f;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        GetComponent<CharacterController>().Move(move * speed * Time.deltaTime);
    }

    void DoTurn()
    {
        if (Mathf.Abs(turnInput) > 0.15f)
        {
            transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);
        }
    }

    // Called automatically by PlayerInput (Send Messages)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnTurn(InputValue value)
    {
        Vector2 stick = value.Get<Vector2>();
        turnInput = stick.x;
    }


    public void OnShoot(InputValue value)
    {
        if (value.Get<float>() > 0.5f)
        {
            shoot?.TryFire();
        }
    }


    public void OnPause(InputValue value)
    {
        if (value.isPressed && PauseMenuController.Instance != null)
        {
            PauseMenuController.Instance.TogglePause();
        }
    }

    public void OnQuit(InputValue value)
    {
        if (value.isPressed && PauseMenuController.Instance != null && PauseMenuController.Instance.IsPaused)
        {
            PauseMenuController.Instance.QuitToMainMenu();
        }
    }
}
