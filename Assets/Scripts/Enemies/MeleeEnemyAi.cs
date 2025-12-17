using UnityEngine;

public class MeleeEnemyAI : BaseEnemyAI
{
    [Header("Probabilidades das Animações (%)")]
    [SerializeField, Range(0,100)] private float lightAttackChance = 25f;
    [SerializeField, Range(0,100)] private float heavyAttackChance = 25f;
    [SerializeField, Range(0,100)] private float specialAttackChance = 25f;
    [SerializeField, Range(0,100)] private float parryChance = 25f;
    
    private bool rotated = true;
    private int lastComboIndex = -1;
    private Quaternion cachedAttackRotation;
    private Vector3 cachedDir;

    
    protected override void OnEnterAttack()
    {

        isInAttackAnimation = true;
        animManager = new CombatAnimationManager(anim);
        animManager.EnableAutoCombo();
        ChooseAnimation();
        agent.isStopped = true;  
        rotated = false;
        
    }

    private void ChooseAnimation()
    {
        float total = lightAttackChance + heavyAttackChance + specialAttackChance + parryChance;
        float rand = Random.Range(0f, total);

        if (rand < lightAttackChance)
        {
            animManager.Play(item.AnimationsData.LightAttack);
            return;
        }

        rand -= lightAttackChance;
        if (rand < heavyAttackChance)
        {
            animManager.Play(item.AnimationsData.HeavyAttack);
            return;
        }

        rand -= heavyAttackChance;
        if (rand < specialAttackChance)
        {
            animManager.Play(item.AnimationsData.SpecialAttack);
            return;
        }

        // Se chegou aqui, cai no último (Parry)
        animManager.Play(item.AnimationsData.Parry);
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
                360f * (Time.deltaTime * timeScale)
            );

            if (Quaternion.Angle(transform.rotation, cachedAttackRotation) < 1f)
                rotated = true;

        }


        // --- Animation-driven movement during the swing ---
        animManager?.UpdatePerFrame(Time.deltaTime * timeScale);

        Vector2 animMove = animManager.GetMovementFromCurrentAnimation();
        Vector3 move = transform.right * animMove.x + transform.forward * animMove.y;
        transform.position += move * (Time.deltaTime * timeScale);


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