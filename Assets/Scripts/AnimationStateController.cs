using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AnimationStateController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float gravity = -9.81f;
    public float rotationSpeed = 15f;   // wie schnell er sich zur Maus dreht

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
        // 1) Movement + Animation
        Vector2 move2D = input.Movement.Move.ReadValue<Vector2>();
        Vector3 move   = new Vector3(move2D.x, 0f, move2D.y);

        bool isMoving = move.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        // 2) Blickrichtung zur Maus
        RotateTowardsMouse();
    }

    private void RotateTowardsMouse()
    {
        if (Camera.main == null || Mouse.current == null)
            return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Vector3 target = hit.point;
            target.y = transform.position.y;

            Vector3 dir = target - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = targetRot;   // direkt, kein Slerp
        }
    }

}