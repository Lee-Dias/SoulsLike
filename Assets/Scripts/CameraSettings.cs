using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[ExecuteAlways]
public class CameraSettings : MonoBehaviour
{
    [Header("Camera Targeting")]
    [SerializeField, Tooltip("Vertical offset from player’s position for the camera to look at")]
    private float lookHeight = 1.2f;
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -4);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-20f, 45f);

    [Header("Initial Rotation")]
    [SerializeField] private float initialYaw = 0f;
    [SerializeField] private float initialPitch = 15f;

    [Header("Lock-On Settings")]
    [SerializeField] private float rangeToLock = 20f;
    [SerializeField] private float distanceToUnlock = 25f;
    [SerializeField] private LayerMask enemyLayer;

    private float yaw;
    private float pitch;

    private Vector2 lookInput;
    
    public Transform currentLockTarget { get; private set; }

    private void OnEnable()
    {
        yaw = initialYaw;
        pitch = initialPitch;
        UpdateCameraTransform(editMode: true);
    }

    private void OnValidate()
    {
        yaw = initialYaw;
        pitch = initialPitch;
        UpdateCameraTransform(editMode: true);
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            yaw = initialYaw;
            pitch = initialPitch;
            UpdateCameraTransform();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnLock(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

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
    }

    private void Unlock()
    {
        currentLockTarget = null;
    }

    void LateUpdate()
    {
        if (!target) return;

        if (Application.isPlaying)
        {
            if (currentLockTarget)
            {
                float dist = Vector3.Distance(target.position, currentLockTarget.position);
                if (dist > distanceToUnlock)
                    Unlock();
            }

            if (currentLockTarget == null)
            {
                // Normal free rotation
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

        Quaternion rotation;
        Vector3 desiredPosition;

        if (currentLockTarget != null)
        {
            // --- Souls-like lock-on orbit behavior ---

            // 1. Find direction from player to enemy (flat on XZ plane)
            Vector3 toEnemy = currentLockTarget.position - targetPosition;
            Vector3 flatDir = new Vector3(toEnemy.x, 0, toEnemy.z).normalized;

            // 2. Get yaw from that direction
            yaw = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;

            // (Optional) smoothly interpolate yaw for smoother orbit
            // yaw = Mathf.LerpAngle(yaw, Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg, Time.deltaTime * followSpeed);

            // Keep pitch limited (you can also make pitch auto-adjust slightly)
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

            // 3. Build rotation from yaw & pitch
            rotation = Quaternion.Euler(pitch, yaw, 0f);

            // 4. Camera stays same offset relative to player (behind player, not enemy)
            desiredPosition = targetPosition + rotation * offset;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // 5. Look directly at the enemy’s upper body
            Vector3 enemyLookPos = currentLockTarget.position + Vector3.up * 0.8f;
            transform.LookAt(enemyLookPos);
        }
        else
        {
            // --- Normal free camera behavior ---

            rotation = Quaternion.Euler(pitch, yaw, 0f);
            desiredPosition = targetPosition + rotation * offset;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
            transform.LookAt(targetPosition + Vector3.up * lookHeight);
        }

        // For edit mode preview (when not playing)
        if (editMode)
        {
            transform.position = targetPosition + Quaternion.Euler(pitch, yaw, 0f) * offset;
            transform.LookAt(targetPosition + Vector3.up * lookHeight);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (target)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, rangeToLock);
        }
    }
}
