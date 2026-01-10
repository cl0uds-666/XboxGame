using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 6f;             // Walking speed
    public float jumpHeight = 1.0f;      // How high the player can jump
    public float gravity = -9.81f;       // Gravity value (negative)

    [Header("Ground Check")]
    public Transform groundCheck;        // Empty object at player's feet
    public float groundDistance = 0.4f;  // Radius of the sphere check
    public LayerMask groundMask;         // Which layers count as ground

    [Header("Input Axis Names")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";
    public string jumpButton = "Jump";

    private CharacterController controller;
    private Vector3 velocity;            // For vertical velocity (gravity, jumps)
    private bool isGrounded;             // Check if player is on ground

    void Start()
    {
        // Get the CharacterController component on this GameObject
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // Check if we're on the ground (simple sphere check)
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            // Fallback if you didn't assign a groundCheck
            isGrounded = controller.isGrounded;
        }

        // Reset vertical velocity if grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get WASD input
        float x = Input.GetAxis(horizontalAxis); // A/D or Left/Right
        float z = Input.GetAxis(verticalAxis);   // W/S or Up/Down

        // Calculate direction relative to player orientation
        Vector3 move = transform.right * x + transform.forward * z;

        // Move the controller
        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown(jumpButton) && isGrounded)
        {
            // v = sqrt(2 * jumpHeight * -gravity)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move vertically
        controller.Move(velocity * Time.deltaTime);
    }
}
