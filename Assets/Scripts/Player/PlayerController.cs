using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float sprintMultiplier = 1.3f;
    [SerializeField] private float lockedDivider = 1.5f;
    [SerializeField] private float staminaToTakeWhileSprinting = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.5f;
    [SerializeField] private MeshTrail meshTrail;
    [SerializeField] private float dashCooldown = 1.0f;
    [SerializeField] private float staminaToWasteOnDash = 20f;


    
    

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private CameraSettings cameraSettings;


    [SerializeField] private Inventory inventory;



    [SerializeField] private PlayerAnimationsController playerAnimationsController;
    [SerializeField] private float walkAfterConsumable;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] walk;
    [SerializeField] private AudioClip dash;
    [SerializeField] private float playWalkEvery = 0.5f;

    [Header("Animation Settings")]
    [SerializeField] private float walkStopDelay = 0.15f; // O tempo de "carência" antes de parar
    private float walkStopTimer;

    private float walkSoundTimer;

    private AudioManager audioManager;
    private PlayerState playerState;
     
    
    private bool isSprinting = false;
    private bool canDash = true;


    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentDirection;
    private Stamina stamina;

    private bool isDashing = false;

    private bool isInvincible = false;

    public bool IsInvincible => isInvincible;




    // Input
    private Vector2 moveInput;

    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        walkSoundTimer = 0f;
        controller = GetComponent<CharacterController>();
        stamina = GetComponent<Stamina>();
        playerState = GetComponent<PlayerState>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    

    public void OnMove(InputAction.CallbackContext value)
    {
        moveInput = value.ReadValue<Vector2>();
    }
    public void OnBoss(InputAction.CallbackContext value)
    {
        SceneManager.LoadScene("BossTest", LoadSceneMode.Single);
    }


    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!canDash || isDashing || !playerState.PlayerCanMove || playerAnimationsController.IsAttacking|| stamina.StaminaValue < staminaToWasteOnDash ) return;

        canDash = false;
        stamina.TakeStamina(staminaToWasteOnDash);

        meshTrail.Trail(0.6f);
        StartCoroutine(Dash());
        StartCoroutine(DashCooldown());
        audioManager.PlayAudio(dash, null, 0 , 1, 0.9f, 1.1f);
        animator.SetTrigger("DoDodge");
        animator.SetBool("Dodge", true);
    }
    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if(stamina.StaminaValue <= 10) return;

        if (context.started)
            isSprinting = true;
        if (context.canceled)
            isSprinting = false;

        animator.SetBool("IsSprinting",isSprinting);
    }

    void Update()
    {
        if(stamina.StaminaValue <= 1)
        {
            isSprinting = false;
            animator.SetBool("IsSprinting", isSprinting);
        }

        if (!isSprinting)
        {
            stamina.ChangeAmountOfStaminaToTake(0);
        }else
        {
            stamina.ChangeAmountOfStaminaToTake(staminaToTakeWhileSprinting);
        }
        

        if (moveInput.sqrMagnitude > 0.01f && playerState.PlayerCanMove)
        {
            // Se houver input, anda e reseta o timer
            animator.SetBool("IsWalking", true);
            walkStopTimer = walkStopDelay; 
        }
        else
        {
            // Se não houver input, começa a contar o tempo para desligar
            walkStopTimer -= Time.deltaTime;

            if (walkStopTimer <= 0)
            {
                animator.SetBool("IsWalking", false);
            }
        }
        HandleWalkAudio();
        MoveCharacter();
    }
    private void ResetMovementState()
    {
        moveInput = Vector2.zero;
        currentDirection = Vector3.zero;
        velocity = Vector3.zero;

        animator.SetFloat("x", 0);
        animator.SetFloat("y", 0);
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsSprinting", false);

        isSprinting = false;
    }
    private void HandleWalkAudio()
    {
        bool isWalking =
            playerState.PlayerCanMove &&
            moveInput.sqrMagnitude > 0.1f &&
            !isDashing;

        if (!isWalking)
        {
            walkSoundTimer = 0f;
            return;
        }

        walkSoundTimer -= Time.deltaTime;

        if (walkSoundTimer <= 0f)
        {
            audioManager.PlayAudio(null , walk, 0 , 0.05f, 0.9f, 1.1f);
            walkSoundTimer = playWalkEvery;
            bool isLocked = cameraSettings != null && cameraSettings.currentLockTarget != null;
            if (isLocked)
            {
                walkSoundTimer = playWalkEvery * lockedDivider;
            }
            if (isSprinting)
            {
                walkSoundTimer = playWalkEvery / sprintMultiplier;
            }
        }
    }

    private void MoveCharacter()
    {
        if (!controller) return;

        if (playerAnimationsController.ShouldBlockMovement(out Vector3 animWalk) || !playerState.PlayerCanMove )
        {
            controller.Move(animWalk * Time.deltaTime);
            ResetMovementState();
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
    public void SnapRotateToTarget()
    {
        if (cameraSettings == null) return;
        if (cameraSettings.currentLockTarget == null) return;

        Vector3 dir = cameraSettings.currentLockTarget.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude <= 0.001f) return;

        transform.rotation = Quaternion.LookRotation(dir);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;
        

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

        animator.SetBool("Dodge", false);


        isInvincible = false;
        isDashing = false;
    }

}
