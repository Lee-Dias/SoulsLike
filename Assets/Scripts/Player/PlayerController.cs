using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float sprintMultiplier = 1.3f;
    [SerializeField] private float lockedDivider = 1.5f;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private SkinnedMeshRenderer[] renderers;
    [SerializeField] private MeshRenderer axeRenderer; 
    

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private CameraSettings cameraSettings;



    [SerializeField] private PlayerCombat playerCombat;
    
    private bool isSprinting = false;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentDirection;

    private bool isDashing = false;
    private bool isInvincible = false;
    private float stopGraceTime = 0.1f; // 80ms grace
    private float stopTimer = 0f;


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
        if (isDashing) return;

        StartCoroutine(Dash());
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            isSprinting = true;
        if (context.canceled)
            isSprinting = false;

        animator.SetBool("IsSprinting",isSprinting);
    }

    void Update()
    {
        if(moveInput.magnitude == 0)
        {
            stopTimer += Time.deltaTime;
            if (stopTimer > stopGraceTime)
            {
                animator.SetBool("IsWalking",false);
            }
            
        }
        else
        {
            stopTimer = 0;
            animator.SetBool("IsWalking",true);
        }
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        if (!controller) return;

        if (playerCombat.ShouldBlockMovement(out Vector3 animWalk))
        {
            controller.Move(animWalk * Time.deltaTime);
            animator.SetFloat("x", 0);
            animator.SetFloat("y", 0);
            return;
        }
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

        if (isLocked && lockTarget != null&& !isSprinting)
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
        float speed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        if (isLocked)
        {
            speed /= lockedDivider;
        }
        Vector3 move = currentDirection * speed;
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

    private IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;

        // Hide the player model
        foreach (var r in renderers)
            r.enabled = false;
        axeRenderer.enabled = false;
        trail.emitting = true;

        Vector3 dashDir = Vector3.zero;

        if (moveInput.sqrMagnitude > 0.1f)
        {
            dashDir = (cameraTransform.forward * moveInput.y + cameraTransform.right * moveInput.x).normalized;
        }
        else
        {
            dashDir = -transform.forward; // default to facing direction
        }
        dashDir.y = 0f;

        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + dashDir * dashDistance;

        while (elapsed < dashDuration)
        {
            // Smoothly interpolate between start and target
            Vector3 next = Vector3.Lerp(start, target, elapsed / dashDuration);
            controller.Move(next - transform.position);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final snap to target position
        controller.Move(target - transform.position);

        // Reveal model again
        foreach (var r in renderers)
            r.enabled = true;
        axeRenderer.enabled = true;
        trail.emitting = false;

        isInvincible = false;
        isDashing = false;
    }
}
