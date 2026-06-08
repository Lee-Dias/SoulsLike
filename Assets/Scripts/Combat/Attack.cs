using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Attack : MonoBehaviour
{
    [SerializeField] private GameObject character;
    [SerializeField] private LayerMask ignore;
    [SerializeField] private LayerMask layerToApplyGroundVFX;
    [SerializeField] private GameObject hitVFX;   // <--- Your VFX prefab
    [SerializeField] private GameObject sparkVFX;  // <--- Your spark VFX prefab


    private Item item;
    private PlayerAnimationsController playerAnimationsController;
    private AudioManager audioManager;
    
    private BaseEnemyAI enemy;

    private float damage;
    private Vector3 hitPoint;
    
    private bool stop = false;
    public void SetCharacther(GameObject gameObject)
    {
        character = gameObject;
        ignore = 1 << gameObject.layer;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & ignore) != 0) return;

        hitPoint = other.ClosestPoint(transform.position);
        if(character == null)
        {
            character = FindFirstObjectByType<BaseEnemyAI>()?.gameObject;
        }


        playerAnimationsController = character.GetComponent<PlayerAnimationsController>();
        enemy = character.GetComponent<BaseEnemyAI>();
        if (enemy == null)
        {
            enemy = FindFirstObjectByType<BaseEnemyAI>();
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
                if(other.GetComponent<Shield>() != null)
                {
                    other.GetComponent<Shield>().TakeShieldDamage(damage);
                }
                else
                {
                    if(other.gameObject.layer == LayerMask.GetMask("Shield"))
                        health.GetHit(damage, true);
                    health.GetHit(damage, false);
                }
                
                if(hitPoint != null && damage > 0 && hitVFX != null)
                {
                    if(other.CompareTag("Dratorsa") && this.GetComponent<OrbProjectile>() != null)
                    {
                        Destroy(this.gameObject);
                        return;
                    }
                        
                    Instantiate(hitVFX, hitPoint, Quaternion.identity);
                }

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
        if(other.gameObject.layer == LayerMask.GetMask("Shield"))
        {
            playerController = other.GetComponentInParent<PlayerController>();
            player = other.GetComponentInParent<PlayerAnimationsController>();
        }
        if(player != null)
        {
            if (player.CanParry)
            {
                stop = true;
            }
            if (player.IsDefending)
            {
                Instantiate(sparkVFX, hitPoint, Quaternion.identity);
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
        }else if (playerAnimationsController.IsDefending)
        {
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
                if(playerAnimationsController.PerformParry())
                {
                    if(sparkVFX != null)
                        Instantiate(sparkVFX, hitPoint, Quaternion.identity);
                }
                return true;
            }
        }       
        return false;
    }

}
