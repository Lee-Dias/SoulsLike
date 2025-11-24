using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float comboResetTime = 5f;
    [SerializeField] private float inputBufferTime = 0.4f;
    [SerializeField] private float staminaToWastePerAttack = 25.5f;


    [Header("References")]
    [SerializeField] private WeaponAnimationsData equippedWeapon;
    [SerializeField] private BoxCollider weaponCollider;
    [SerializeField] private InventoryManager inventoryManager;

    private Animator anim;
    private Stamina stamina;
    private CombatAnimationManager animManager;
    private Health health;



    private bool attackQueued;
    private float bufferTimer;
    private bool isAttacking;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        stamina = GetComponent<Stamina>();
        health = GetComponent<Health>();
        animManager = new CombatAnimationManager(anim);
    }

    private void Update()
    {
        // manually update combat animation manager
        animManager.UpdatePerFrame(Time.deltaTime);

        if(!health.CanAttack() && animManager.IsPlaying)
        {
            ResetCombatState();
            weaponCollider.enabled = false;
        }

        // reset state if combo window expired and animation finished
        if (isAttacking && animManager.CurrentAnimation != null)
        {
            float normalizedTime;

            normalizedTime = animManager.Handle.GetNormalizedTime();

            if (normalizedTime >= 1.1f && !animManager.QueuedNext)
            {
                ResetCombatState();
            }
        }
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
        
        // input buffer timeout
        if (attackQueued)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
                attackQueued = false;
        }
    }

    // ----------------------------
    // Input bindings
    // ----------------------------
    public void OnLightAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        HandleAttackInput(equippedWeapon?.LightAttack);
    }

    public void OnHeavyAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        HandleAttackInput(equippedWeapon?.HeavyAttack);
    }

    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        HandleAttackInput(equippedWeapon?.SpecialAttack);
    }

    
    public bool ShouldBlockMovement(out Vector3 animMotion)
    {
        animMotion = Vector3.zero;

        if(health.ShouldBlockMovement())
            return true;

        var anim = animManager.CurrentAnimation;
        if (anim == null)
            return false;

        if (!anim.StopControlledMovement)
            return false;

        // MovementCurve always determines the movement
        if (anim.WalkDuringAnimation)
        {
            Vector2 movement = animManager.GetMovementFromCurrentAnimation();

            animMotion =
                transform.right * movement.x +
                transform.forward * movement.y;

            return true;
        }

        return true; // block movement fully
    }

    // ----------------------------
    // Core combat logic
    // ----------------------------
    private void HandleAttackInput(CombatAnimations animData)
    {
        if (animData == null || !health.CanAttack()) return;
        
        if (inventoryManager != null)
            if(inventoryManager.IsActive)
                return;
        

        if (!animManager.IsPlaying && stamina.StaminaValue >= staminaToWastePerAttack && !animManager.QueuedNext && !isAttacking )
        {
            if(animManager.Handle != null)
                if(animManager.Handle.IsFadingOut || animManager.Handle.IsFadingIn)
                    return;
            
            Debug.Log("attack called");
            animManager.Play(animData);
            isAttacking = true;
            stamina.TakeStamina(staminaToWastePerAttack);
            return;
        }

        // if attacking → queue next combo
        if (!attackQueued && stamina.StaminaValue >= staminaToWastePerAttack && !animManager.QueuedNext && !animManager.Handle.IsFadingOut && !animManager.Handle.IsBlending) 
        {
            Debug.Log("queue called");
            attackQueued = true;
            bufferTimer = inputBufferTime;
            animManager.TryQueueNextStep();
            if (animManager.QueuedNext)
            {
                stamina.TakeStamina(staminaToWastePerAttack);
            }
            
        }
    }

    private void ResetCombatState()
    {
        animManager.Stop();
        isAttacking = false;
        attackQueued = false;
        bufferTimer = 0;
    }
}
