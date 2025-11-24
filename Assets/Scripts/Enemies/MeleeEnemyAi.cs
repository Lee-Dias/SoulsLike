using UnityEngine;

public class MeleeEnemyAI : BaseEnemyAI
{

    [SerializeField]private float timeToGoBackToCircleAfterLight = 3;
    [SerializeField]private CombatAnimations animData;
    private bool rotated = true;
    
    protected override void OnEnterAttack()
    {

        isInAttackAnimation = true;
        animManager = new CombatAnimationManager(anim);
        animManager.EnableAutoCombo();
        animManager.Play(animData);
        agent.isStopped = true;  
        rotated = false;
        
           
    }

    protected override void Attack()
    {
        if (!rotated)
        {
            RotateTowardPlayer();
            if (RotateTowardPlayerEnded())
                rotated = true; 
            return;
        }

        animManager?.UpdatePerFrame(Time.deltaTime);
        Vector2 animMove = animManager.GetMovementFromCurrentAnimation();
        Vector3 move = transform.right * animMove.x + transform.forward * animMove.y;
        transform.position += move * Time.deltaTime;

        if (animManager.Handle != null)
        {
            if (animManager.Handle.ActivateHitBox)
            {
                weaponCollider.enabled = true;
            }
            else
            {
                weaponCollider.enabled = false;
            }
                
        }

        if (!animManager.IsPlaying && !animManager.Handle.IsFadingOut || !health.CanAttack())
        {
            if(!health.CanAttack())
                animManager.Stop();
            weaponCollider.enabled = false;
            isInAttackAnimation = false;
            attackEnded = true;
            agent.isStopped = false;   
        }
    }
    
}