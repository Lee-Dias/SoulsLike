using UnityEngine;
using UnityEngine.InputSystem;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject destroyedVersion;
    [SerializeField] private float explosionForce = 0.5f;
    [SerializeField] private float explosionRadius = 0.01f;
    [SerializeField] private float upwardModifier = 0.05f;

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    GameObject broken = Instantiate(destroyedVersion, transform.position, transform.rotation);

                    Rigidbody[] bodies = broken.GetComponentsInChildren<Rigidbody>();

                    foreach (Rigidbody rb in bodies)
                    {
                        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardModifier, ForceMode.Impulse);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}