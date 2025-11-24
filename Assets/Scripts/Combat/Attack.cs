using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField]private GameObject Ignore;
    [SerializeField]private float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Ignore) return;
        
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.GetHit(damage);
        }
    }

}
