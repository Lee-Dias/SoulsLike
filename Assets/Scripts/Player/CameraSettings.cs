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

    private float currentCameraDistance;
    private float yaw;
    private float pitch;

    private Vector2 lookInput;
    public Transform currentLockTarget { get; private set; }

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

        pitch = lockOnPitch;
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

        if (currentLockTarget != null)
        {
            Vector3 toEnemy = currentLockTarget.position - targetPosition;
            Vector3 flatDir = new Vector3(toEnemy.x, 0, toEnemy.z).normalized;

            yaw = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;
            pitch = lockOnPitch;

            rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        // --- Calculate camera position ---
        Vector3 desiredDirection = (rotation * offset).normalized;
        float desiredDistance = offset.magnitude;
        float correctedDistance = desiredDistance;

        if (Application.isPlaying)
        {
            // Loop through all wall layers and check if the object is in any of them
            foreach (var layer in wallLayers)
            {
                if (Physics.SphereCast(targetPosition, cameraCollisionRadius, desiredDirection, out RaycastHit hit, desiredDistance, layer))
                {
                    // Apply the collision when the camera is against a wall
                    correctedDistance = hit.distance - 0.1f;
                    if (correctedDistance < 0f) correctedDistance = 0f;
                    break; // Stop checking further layers once we hit something
                }
            }

            currentCameraDistance = Mathf.Lerp(currentCameraDistance, correctedDistance, Time.deltaTime * cameraCollisionSmooth);
            transform.position = targetPosition + desiredDirection * currentCameraDistance - Vector3.up * verticalCameraShift;
        }
        else
        {
            // Editor preview
            transform.position = targetPosition + desiredDirection * desiredDistance - Vector3.up * verticalCameraShift;
        }

        // Look at target
        if (currentLockTarget)
            transform.LookAt(currentLockTarget.position + Vector3.up * 0.8f);
        else
            transform.LookAt(targetPosition + Vector3.up * lookHeight);
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
