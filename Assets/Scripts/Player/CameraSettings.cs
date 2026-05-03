using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[ExecuteAlways]
public class CameraSettings : MonoBehaviour
{
    [Header("Camera Targeting")]
    [SerializeField] private float lookHeight = 1.2f;
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -4);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-20f, 45f);
    [SerializeField] private float verticalCameraShift = 0f; // new variable

    [Header("Initial Rotation")]
    [SerializeField] private float initialYaw = 0f;
    [SerializeField] private float initialPitch = 15f;

    [Header("Lock-On Settings")]
    [SerializeField] private float rangeToLock = 20f;
    [SerializeField] private float distanceToUnlock = 25f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Lock-On Camera Angle")]
    [SerializeField] private float lockOnPitch = 15f;   // fixed lock-on height

    [Header("Camera Collision")]
    [SerializeField] private float cameraCollisionRadius = 0.3f;
    [SerializeField] private float cameraCollisionSmooth = 15f;
    [SerializeField] private LayerMask[] wallLayers; // New variable for the wall layer

    [Header("Lock-On Smoothness")]
    [SerializeField] private float lockOnSmoothSpeed = 5f; // Velocidade de transição para o Lock-On

    private Vector3 shakeOffset;
    private Coroutine shakeCoroutine;

    private float currentCameraDistance;
    private float yaw;
    private float pitch;
    private PlayerState playerState;

    private Vector2 lookInput;
    public Transform currentLockTarget { get; private set; }
    private Transform tempTarget;

    private void OnEnable()
    {
        yaw = initialYaw;
        pitch = initialPitch;
        currentCameraDistance = offset.magnitude;
        UpdateCameraTransform(editMode: true);
    }

    private void OnValidate()
    {
        yaw = initialYaw;
        pitch = initialPitch;
        currentCameraDistance = offset.magnitude;
        UpdateCameraTransform(editMode: true);
    }

    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
        if (Application.isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yaw = initialYaw;
            pitch = initialPitch;
            currentCameraDistance = offset.magnitude;
            UpdateCameraTransform();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!playerState.PlayerCanMove)
        {
            lookInput = new Vector2(0, 0);
            return;
        }
        
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnLock(InputAction.CallbackContext context)
    {
        if (!context.performed || !playerState.PlayerCanMove) return;

        if (currentLockTarget == null)
            TryLockOn();
        else
            Unlock();
    }

    private void TryLockOn()
    {
        if (!target) return;

        Collider[] enemies = Physics.OverlapSphere(target.position, rangeToLock, enemyLayer);
        if (enemies.Length == 0) return;

        Camera cam = Camera.main;
        if (!cam) return;

        var bestTarget = enemies
            .Select(e => e.transform)
            .OrderBy(e =>
            {
                Vector3 screenPos = cam.WorldToViewportPoint(e.position);
                if (screenPos.z < 0) return float.MaxValue;
                return Vector2.Distance(new Vector2(screenPos.x, screenPos.y), new Vector2(0.5f, 0.5f));
            })
            .FirstOrDefault();

        currentLockTarget = bestTarget;

        ActivateSymbol();

        

    }

    private void ActivateSymbol()
    {
        currentLockTarget.gameObject.GetComponent<LockOnSymbol>()?.ActivateSymbol();
    }

    private void Unlock()
    {
        ActivateSymbol();
        currentLockTarget = null;
    }

    void LateUpdate()
    {
        if (!target) return;

        if (Application.isPlaying)
        {
            if (currentLockTarget)
            {
                if (currentLockTarget.gameObject.layer != LayerMask.NameToLayer("Enemy"))
                {
                    Unlock();
                    return;                    
                }

                float dist = Vector3.Distance(target.position, currentLockTarget.position);
                if (dist > distanceToUnlock)
                    Unlock();
            }

            if (currentLockTarget == null)
            {
                yaw += lookInput.x * rotationSpeed;
                pitch -= lookInput.y * rotationSpeed;
                pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
            }

            UpdateCameraTransform();
        }
        else
        {
            UpdateCameraTransform(editMode: true);
        }
    }

    private void UpdateCameraTransform(bool editMode = false)
    {
        if (!target) return;

        Vector3 targetPosition = target.position;

        // 1. Lógica de Interpolação de ângulos (apenas se houver lock)
        if (currentLockTarget != null && !editMode)
        {
            Vector3 toEnemy = currentLockTarget.position - targetPosition;
            Vector3 flatDir = new Vector3(toEnemy.x, 0, toEnemy.z).normalized;

            float targetYaw = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;
            float targetPitch = lockOnPitch;

            // Movemos os valores atuais em direção ao alvo suavemente
            yaw = Mathf.MoveTowardsAngle(yaw, targetYaw, Time.deltaTime * lockOnSmoothSpeed * 50f);
            pitch = Mathf.MoveTowards(pitch, targetPitch, Time.deltaTime * lockOnSmoothSpeed * 50f);
        }

        // 2. Definir a rotação com base no yaw/pitch atual (que já está a ser interpolado)
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // --- Calcular a posição da câmera ---
        Vector3 desiredDirection = (rotation * offset).normalized;
        float desiredDistance = offset.magnitude;
        float correctedDistance = desiredDistance;

        if (Application.isPlaying)
        {
            foreach (var layer in wallLayers)
            {
                if (Physics.SphereCast(targetPosition, cameraCollisionRadius, desiredDirection, out RaycastHit hit, desiredDistance, layer))
                {
                    correctedDistance = Mathf.Clamp(hit.distance - 0.1f, 0f, desiredDistance);
                    break;
                }
            }

            currentCameraDistance = Mathf.Lerp(currentCameraDistance, correctedDistance, Time.deltaTime * cameraCollisionSmooth);
            transform.position = targetPosition + desiredDirection * currentCameraDistance - Vector3.up * verticalCameraShift + shakeOffset;
        }
        else
        {
            transform.position = targetPosition + desiredDirection * desiredDistance - Vector3.up * verticalCameraShift;
        }

        // 3. ROTAÇÃO FINAL
        // Importante: NÃO uses LookAt se queres que a interpolação que fizemos acima funcione.
        // O LookAt anula o trabalho do 'rotation' calculado.
        transform.rotation = rotation;
    }

    
    public void ShakeCamera(float intensity, float duration = 1.2f, float speed = 15f)
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(DoShake(intensity, duration, speed));
    }

    private System.Collections.IEnumerator DoShake(float intensity, float duration, float speed)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {

            float oscillation = Mathf.Sin(elapsed * speed);

            float fade = 1f - (elapsed / duration);

            float yOffset = oscillation * intensity * fade;

            shakeOffset = new Vector3(0, yOffset, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Garante que termina exatamente em zero
        shakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (target)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, rangeToLock);
        }
    }

    public void ChangeToBonfire(Vector3 bonfirePosition)
    {
        tempTarget = target;
        target.position = bonfirePosition;
        UpdateCameraTransform();
    }

    public void ReturnFromBonfire()
    {
        target = tempTarget;
        UpdateCameraTransform();
    }
}
