using System.Runtime.InteropServices;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private GameObject character;
    [SerializeField] private LayerMask ignore;
    [SerializeField] private LayerMask layerToApplyGroundVFX;
    [SerializeField] private GameObject hitVFX;   // <--- Your VFX prefab
    [SerializeField] private GameObject groundVFX; 

    private Item item;
    private PlayerAnimationsController playerAnimationsController;
    
    private BaseEnemyAI enemy;

    private float damage;
    
    private bool stop = false;
    public void SetCharacther(GameObject gameObject)
    {
        character = gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & ignore) != 0) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        if(character == null)
        {
            character = this.GetComponentInParent<BaseEnemyAI>()?.gameObject;
        }

        playerAnimationsController = character.GetComponent<PlayerAnimationsController>();
        enemy = character.GetComponent<BaseEnemyAI>();
        if (enemy == null)
        {
            enemy = character.GetComponentInParent<BaseEnemyAI>();
        }
        if(playerAnimationsController != null)
        {
            PlayerAttack(other);
        }
        else
        {
            EnemyAttack(other);
        }

        if(stop)
        {
            stop = false;
            return;
        }
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            if (health.CanHit())
            {
                health.GetHit(damage);
                if(hitPoint != null && damage > 0)
                    Instantiate(hitVFX, hitPoint, Quaternion.identity);
                if(this.GetComponent<OrbProjectile>() != null)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void EnemyAttack(Collider other)
    {
        PlayerAnimationsController player = other.GetComponent<PlayerAnimationsController>();
        PlayerController playerController = other.GetComponent<PlayerController>();
        if(player != null)
        {
            if (player.CanParry)
            {
                stop = true;
            }
        }
        if(playerController != null)
        {
            if(playerController.IsInvincible)
            {
                stop = true;
            }
        }
            


        damage = enemy.DamageToDeal();
    }
    private void PlayerAttack(Collider other)
    {
        if (playerAnimationsController.IsDoingParry)
        {
            if (TryParry(other))
            {
                return;
            }
            stop = true;
            return;
        }
        damage = playerAnimationsController.DamageToDeal();
        
    }
    private bool TryParry(Collider other)
    {
        BaseEnemyAI enemyAIOther = other.GetComponent<BaseEnemyAI>();
        if (enemyAIOther != null)
        {
            if (enemyAIOther.IsInAttackAnimation)
            {
                damage = 0;
                playerAnimationsController.PerformParry();
                return true;
            }
        }       
        return false;
    }

}
