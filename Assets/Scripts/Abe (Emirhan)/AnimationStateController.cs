using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AnimationStateController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float gravity   = -9.81f;
    public float rotationSpeed = 15f;

    [System.Serializable]
    public class SkillSlot
    {
        public SkillKey key;            // Q/E/R
        public GameObject skillPrefab;  // Meteor, Meteorregen, etc.
        public float spawnHeight = 15f; // Höhe über Boden
        public int manaCost = 10;       // Mana-Kosten
    }

    public enum SkillKey
    {
        Q,
        E,
        R
    }

    [Header("Skills")]
    public SkillSlot[] skillSlots;

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
        HandleMovement();
        HandleSkillInput();
        RotateTowardsMouse();
    }

    private void HandleMovement()
    {
        Vector2 move2D = input.Movement.Move.ReadValue<Vector2>();

        Vector3 move = Vector3.zero;

        if (Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            move = camRight * move2D.x + camForward * move2D.y;
        }
        else
        {
            move = new Vector3(move2D.x, 0f, move2D.y);
        }

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
    }

    private void HandleSkillInput()
    {
        if (skillSlots == null) return;
        if (Keyboard.current == null) return;

        foreach (var slot in skillSlots)
        {
            if (slot == null) continue;

            if (IsKeyPressed(slot.key))
            {
                CastSkillPrefab(slot);
            }
        }
    }

    private bool IsKeyPressed(SkillKey key)
    {
        switch (key)
        {
            case SkillKey.Q: return Keyboard.current.qKey.wasPressedThisFrame;
            case SkillKey.E: return Keyboard.current.eKey.wasPressedThisFrame;
            case SkillKey.R: return Keyboard.current.rKey.wasPressedThisFrame;
            default: return false;
        }
    }

    private void CastSkillPrefab(SkillSlot slot)
    {
        if (slot.skillPrefab == null) return;
        if (Camera.main == null || Mouse.current == null) return;

        // 1) Mana abziehen über Tiny HealthSystem
        if (HealthSystem.Instance != null && slot.manaCost > 0)
        {
            Debug.Log($"[Skill] {slot.key} cast – trying to use {slot.manaCost} mana. Current: {HealthSystem.Instance.manaPoint}");

            HealthSystem.Instance.UseMana(slot.manaCost);

            Debug.Log($"[Skill] New mana after cast: {HealthSystem.Instance.manaPoint}");
        }
        else
        {
            Debug.LogWarning("[Skill] HealthSystem.Instance ist null ODER manaCost == 0");
        }

        // 2) Zielpunkt unter der Maus bestimmen
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Vector3 targetPos = hit.point;
            Vector3 spawnPos  = targetPos + Vector3.up * slot.spawnHeight;

            Instantiate(slot.skillPrefab, spawnPos, Quaternion.identity);
        }
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
            transform.rotation = targetRot;
        }
    }
}
