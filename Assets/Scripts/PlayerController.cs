using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;         // velocidade máxima
    [SerializeField] private float acceleration = 8f;      // aceleração até atingir moveSpeed
    [SerializeField] private float gravity = -9.81f;       // gravidade

    [Header("References")]
    [SerializeField] private Transform cameraTransform;    // referência à câmera (3ª pessoa)
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentDirection;

    // Input
    private Vector2 moveInput;           // recebido do Input System

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    // Chamado automaticamente pelo PlayerInput
    public void OnMove(InputAction.CallbackContext value)
    {
        moveInput = value.ReadValue<Vector2>();
        //moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
    // --- MOVEMENT RELATIVE TO CAMERA ---
    Vector3 camForward = cameraTransform.forward;
    Vector3 camRight = cameraTransform.right;
    camForward.y = 0f;
    camRight.y = 0f;
    camForward.Normalize();
    camRight.Normalize();

    // Desired horizontal direction
    Vector3 targetDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

    if (controller.isGrounded)
    {
        // Animator updates
        animator.SetFloat("x", moveInput.x);
        animator.SetFloat("y", moveInput.y);

        // Only allow movement on ground
        if (targetDirection.magnitude > 0.1f)
        {
            currentDirection = Vector3.Lerp(currentDirection, targetDirection, acceleration * Time.deltaTime);

            // Rotate character
            Quaternion targetRot = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
        else
        {
            currentDirection = Vector3.Lerp(currentDirection, Vector3.zero, acceleration * Time.deltaTime);
        }

        // Reset small downward force when grounded
        if (velocity.y < 0)
                velocity.y = -2f;
        }
        else
        {
            // In the air → no horizontal control
            currentDirection = Vector3.zero;
        }

        // --- APPLY MOVEMENT ---
        Vector3 move = currentDirection * moveSpeed;
        controller.Move(move * Time.deltaTime);

        // --- GRAVITY ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
