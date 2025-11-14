using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AnimationStateController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float gravity   = -9.81f;

    private Animator animator;
    private CharacterController controller;
    private PlayerInputActions input;
    private float verticalVelocity;

    private void Awake()
    {
        animator   = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        input      = new PlayerInputActions();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        // Input (WASD)
        Vector2 move2D = input.Movement.Move.ReadValue<Vector2>();
        Vector3 move   = new Vector3(move2D.x, 0f, move2D.y);

        bool isMoving = move.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        // Gravity
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f; // leicht nach unten, damit er am Boden "klebt"
        }

        verticalVelocity += gravity * Time.deltaTime;

        // horizontale + vertikale Bewegung kombinieren
        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
