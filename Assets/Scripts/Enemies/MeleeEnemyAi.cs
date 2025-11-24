using UnityEngine;

public class MeleeEnemyAI : BaseEnemyAI
{

    [SerializeField]private float timeToGoBackToCircleAfterLight = 3;
    [SerializeField]private CombatAnimations animData;
    private bool rotated = true;
    private int lastComboIndex = -1;
    private Quaternion cachedAttackRotation;
    private Vector3 cachedDir;

    
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
        // --- Detect new attack in the combo ---
        if (animManager.Handle != null && animManager.Handle.ComboIndex != lastComboIndex)
        {
            if (!IsInPreferredRange())
            {
                animManager.Stop();
                weaponCollider.enabled = false;
                isInAttackAnimation = false;
                attackEnded = true;
                agent.isStopped = false;
                lastComboIndex = -1;
                return;
            }
            lastComboIndex = animManager.Handle.ComboIndex;
            rotated = false;

            // Freeze direction at the moment the swing begins
            Vector3 dir = (player.position - transform.position);
            dir.y = 0;
            cachedDir = dir.normalized;

            cachedAttackRotation = Quaternion.LookRotation(cachedDir);
        }


        // --- Rotate toward the frozen rotation (not the moving player) ---
        if (!rotated)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                cachedAttackRotation,
                360f * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, cachedAttackRotation) < 1f)
                rotated = true;

        }


        // --- Animation-driven movement during the swing ---
        animManager?.UpdatePerFrame(Time.deltaTime);

        Vector2 animMove = animManager.GetMovementFromCurrentAnimation();
        Vector3 move = transform.right * animMove.x + transform.forward * animMove.y;
        transform.position += move * Time.deltaTime;


        // --- Hitbox activation ---
        if (animManager.Handle != null)
            weaponCollider.enabled = animManager.Handle.ActivateHitBox;


        // --- End of attack ---
        if ((!animManager.IsPlaying && !animManager.Handle.IsFadingOut) || !health.CanAttack())
        {
            if(!health.CanAttack())
                animManager.Stop();

            weaponCollider.enabled = false;
            isInAttackAnimation = false;
            attackEnded = true;
            agent.isStopped = false;

            lastComboIndex = -1;
        }
    }

    
}