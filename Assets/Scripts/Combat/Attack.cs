using UnityEngine;

public class WeaponHit : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask ignore;
    [SerializeField] private GameObject hitVFX;   // <--- Your VFX prefab
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & ignore) != 0) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);

        // spawn VFX
        

        Health health = other.GetComponent<Health>();

        if (health != null)
        {
            if (health.CanHit())
            {
                health.GetHit(damage);
                Instantiate(hitVFX, hitPoint, Quaternion.identity);
            }
        }
    }
}
