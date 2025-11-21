using UnityEngine;

public class MeleeEnemyAI : BaseEnemyAI
{
    protected override void OnEnterAttack()
    {
        stateAfterCircle = null;
        agent.isStopped = true;
        Debug.Log("Melee enemy preparing to attack!");
    }

    protected override void Attack()
    {
        attacked = true;
        Debug.Log("Melee enemy attacks!");
    }
    
}