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
    private PlayerCombat playerCombat;
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

        playerCombat = character.GetComponent<PlayerCombat>();
        enemy = character.GetComponent<BaseEnemyAI>();
        if(playerCombat != null)
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
            }
        }
    }

    private void EnemyAttack(Collider other)
    {
        PlayerCombat player = other.GetComponent<PlayerCombat>();
        if(player != null)
        {
            if (player.CanParry)
            {
                stop = true;
            }
        }

        damage = 20;
    }
    private void PlayerAttack(Collider other)
    {
        if (playerCombat.IsDoingParry)
        {
            if (TryParry(other))
            {
                return;
            }
            stop = true;
            return;
        }
        damage = playerCombat.DamageToDeal();
        
    }
    private bool TryParry(Collider other)
    {
        BaseEnemyAI enemyAIOther = other.GetComponent<BaseEnemyAI>();
        if (enemyAIOther != null)
        {
            if (enemyAIOther.IsInAttackAnimation)
            {
                damage = 0;
                playerCombat.PerformParry();
                return true;
            }
        }       
        return false;
    }

}
