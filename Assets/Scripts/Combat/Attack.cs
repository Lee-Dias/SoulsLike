using UnityEngine;

public class Attack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        // If the object is on the "Player" layer, ignore it
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) return;
        
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.GetHit();
        }
    }

}
