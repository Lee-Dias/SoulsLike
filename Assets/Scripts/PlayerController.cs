using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private CameraSettings cameraSettings;

    [SerializeField] private PlayerCombat playerCombat;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentDirection;

    // Input
    private Vector2 moveInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        moveInput = value.ReadValue<Vector2>();
    }
    
    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
    }

    void Update()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        if (!controller) return;

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();


        bool isLocked = cameraSettings != null && cameraSettings.currentLockTarget != null;
        Transform lockTarget = cameraSettings != null ? cameraSettings.currentLockTarget : null;

        animator.SetBool("cameraLocked", isLocked);

        Vector3 targetDirection = Vector3.zero;

        if (isLocked && lockTarget != null)
        {
            // Movement relative to camera, not target
            targetDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

            if (targetDirection.magnitude > 0.1f)
            {
                currentDirection = Vector3.Lerp(currentDirection, targetDirection, acceleration * Time.deltaTime);
            }
            else
            {
                currentDirection = Vector3.Lerp(currentDirection, Vector3.zero, acceleration * Time.deltaTime);
            }

            // --- Rotate player toward lock target ---
            Vector3 lookDir = (lockTarget.position - transform.position);
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
            }
        }
        else
        {
            // --- Free look rotation ---
            targetDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

            if (targetDirection.magnitude > 0.1f)
            {
                currentDirection = Vector3.Lerp(currentDirection, targetDirection, acceleration * Time.deltaTime);

                // Rotate character toward movement direction
                Quaternion targetRot = Quaternion.LookRotation(currentDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            }
            else
            {
                currentDirection = Vector3.Lerp(currentDirection, Vector3.zero, acceleration * Time.deltaTime);
            }
        }

        // --- Apply movement ---
        Vector3 move = currentDirection * moveSpeed;
        controller.Move(move * Time.deltaTime);

        // --- Gravity ---
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Animator movement inputs ---
        animator.SetFloat("x", moveInput.x);
        animator.SetFloat("y", moveInput.y);
    }
}
