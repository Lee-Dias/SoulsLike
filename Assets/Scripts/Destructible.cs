using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject destroyedVersion;
    [SerializeField] private float explosionForce = 0.5f;
    [SerializeField] private float explosionRadius = 0.01f;
    [SerializeField] private float upwardModifier = 0.05f;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float audioDelay = 0f;
    [SerializeField] private float audioVolume = 1f;
    [SerializeField] private string layerToDestroy; // e.g., "Player"


    private Health health;
    private AudioManager audioManager;
    private bool _isDestroyed = false; // Prevents multiple triggers

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        health = GetComponentInParent<Health>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if it has already been destroyed
        // 2. Check if the object entering is on the correct layer
        if (!_isDestroyed && other.gameObject.layer == LayerMask.NameToLayer(layerToDestroy))
        {
            if(health != null)
            {
            }
            else
            {
                DestroyObject();
            }
            
        }
    }

    public void DestroyObject()
    {
        _isDestroyed = true; // Lock the state

        GameObject broken = Instantiate(destroyedVersion, transform.position, transform.rotation, this.transform.parent);
        Rigidbody[] bodies = broken.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in bodies)
        {
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardModifier, ForceMode.Impulse);
        }

        if (audioManager != null)
        {
            audioManager.PlayAudio(audioClip, null, audioDelay, audioVolume);
        }

        Destroy(gameObject);
    }
}