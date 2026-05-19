using UnityEngine;

public class OrbDeflect : MonoBehaviour
{
    private PlayerAnimationsController playerAnimationsController;
    private Transform Boss;

    private void Start()
    {
        playerAnimationsController = FindFirstObjectByType<PlayerAnimationsController>();
        Boss = GameObject.FindWithTag("Dratorsa").transform;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Sword") || other.gameObject.layer == LayerMask.NameToLayer("Shield"))
        {
            OrbProjectile orb = this.GetComponentInParent<OrbProjectile>();
            other.GetComponent<PlayerAnimationsController>();
            if(playerAnimationsController.IsAttacking || playerAnimationsController.IsDoingParry)
            {
                orb.SetTarget(Boss);
            }
            else
            {
                orb.SetTarget(Boss, false); 
            }
            orb.ChangeCharacter(playerAnimationsController.gameObject); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = this.transform.parent.position;
    }
}
