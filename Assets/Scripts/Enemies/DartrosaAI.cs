using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DratorsaAI : BaseEnemyAI
{

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float delayToInstantiateProjectile = 1f;
    
    protected override void OnEnterAttack()
    {
        anim.SetTrigger("Attack");
        StartCoroutine(SpawnProjectile());
    }

    protected override void Attack()
    {
        
        
    }

    private IEnumerator SpawnProjectile()
    {
        yield return new WaitForSeconds(delayToInstantiateProjectile);
        Vector3 tr = this.transform.position;
        tr.y += 1f; 
        GameObject projectile = Instantiate(projectilePrefab, tr, this.transform.rotation, this.transform);
        projectile.GetComponent<OrbProjectile>().SetTarget(player.transform);
        attackEnded = true;
    }
    protected override void OnEnterFirstAttack()
    {
    }

    protected override void FirstAttack()
    {
        
    }

}
