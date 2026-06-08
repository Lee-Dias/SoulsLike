using UnityEngine;
using UnityEngine.Rendering;

public class GroundChecker : MonoBehaviour
{

    [SerializeField] private GameObject groundVFX; 
    [SerializeField] private AudioClip hitGroundSound;
    [SerializeField] private float volume;
    private Vector3 hitPoint;
    private AudioManager audioManager;
    private float justPlayed = 0.5f;

    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    private void Update()
    {
        justPlayed += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        hitPoint = other.ClosestPoint(transform.position);
        if (other.gameObject.layer== LayerMask.NameToLayer("Ground"))
        {
            if(justPlayed > 0.5f)
            {
                justPlayed = 0f;
                if(groundVFX != null)
                    Instantiate(groundVFX, hitPoint, Quaternion.identity);
                if(hitGroundSound != null)
                    audioManager.PlayAudio(hitGroundSound, null, 0,volume);            
            }

        }
    }
}
