using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationsController : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float comboResetTime = 5f;
    [SerializeField] private float inputBufferTime = 0.4f;
    [SerializeField] private float staminaToWastePerAttack = 25.5f;


    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private GameObject weaponHolder;
    


    [Header("Parry Settings")]
    [SerializeField] private ParticleSystem parryVfx;
    [SerializeField] private float parryRadius = 10f;
    [SerializeField] private float parryTimeScale = 0.1f;   // time slowdown strength
    [SerializeField] private float parryFreezeDuration = 3f; // how long ZA WARUDO lasts
    [SerializeField] private float parryDelay = 1f;

    [SerializeField] private LayerMask enemyLayer;           // which layer enemies are on

    [SerializeField] private CombatModifiers combatModifiers;
    [SerializeField] private LiminalUIController liminalUIController;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip timeStop;


    private AudioManager audioManager;
    private CameraSettings cameraSettings;
    
    

    private Item equippedWeapon;
    private Item equippedShield;

    private Animator anim;
    private Stamina stamina;
    private CombatAnimationManager animManager;
    private Health health;

    private GameObject objectSpawned;



    private bool attackQueued;
    private float bufferTimer;
    private bool isAttacking;
    private bool isDoingParry;
    private bool canParry = false; 

    private bool isDefending;


    private GameObject objectToSpawn; 
    private Vector3 positionToSpawnObject; 
    private bool spawned = false;
    private bool shakeCamera;
    private BoxCollider weaponCollider;
    private BoxCollider shieldCollider;
    private bool parryCalled;
    private bool rotatedPlayer;
    private float bonusCheat = 0;
    private bool activateTrail;
    private bool soundActivate;
    private bool holdDefend;

    public bool IsAttacking => isAttacking;
    public bool ActivateTrail => activateTrail;
    public bool IsDoingParry => isDoingParry;
    public bool CanParry => canParry;
    public Item EquippedWeapon => equippedWeapon;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        stamina = GetComponent<Stamina>();
        health = GetComponent<Health>();
        animManager = new CombatAnimationManager(anim);
        animManager.OnStepStarted += HandleAnimStepStarted;
        audioManager = FindFirstObjectByType<AudioManager>();
        cameraSettings = FindFirstObjectByType<CameraSettings>();
        playerState = FindFirstObjectByType<PlayerState>();
    }

    public void EnablePlayerAfterSitting()
    {
        liminalUIController.DisableSittingAfterDelay();
    }
    public void ChangeEquippedWeapon(Item item){
        equippedWeapon = item;
    }
    public void ChangeWeaponCollider(BoxCollider boxCollider)
    {
        weaponCollider = boxCollider;
    }
    public void ChangeShieldCollider(BoxCollider boxCollider)
    {
        shieldCollider = boxCollider;
    }
    public void ChangeEquippedShield(Item item){
        equippedShield = item;  
    }

    public void OnScaleDamage(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        if (playerStats == null) return;

        if(bonusCheat == 0)
        {
            bonusCheat = 100000;
        }else
        {
            bonusCheat = 0;
        }
    }


    private void Update()
    {         
        HandleDefense();
        
        // manually update combat animation manager
        animManager.UpdatePerFrame(Time.deltaTime);

        if(!health.CanAttack() && animManager.IsPlaying)
        {
            ResetCombatState();
            if(weaponCollider != null)
                weaponCollider.enabled = false;
        }

        // reset state if combo window expired and animation finished
        if (isAttacking && animManager.CurrentAnimation != null)
        {
            float normalizedTime;

            normalizedTime = animManager.Handle.GetNormalizedTime();

            if (normalizedTime >= 0.95f && !animManager.QueuedNext)
            {
                ResetCombatState();
            }
        }
        if (animManager.Handle != null)
        {
            if (animManager.Handle.ActivateTrail && animManager.IsPlaying)
            {
                activateTrail = true;
            }
            else
            {
                activateTrail = false;  
            }
            if (!animManager.Handle.SoundActivate)
            {
                soundActivate = false;
            }
            if(animManager.Handle.SoundActivate && !soundActivate)
            {
                audioManager.PlayAudio(null, animManager.Handle.SoundEffectCombo, 0f);
                soundActivate = true;
            }

            if (animManager.Handle.ActivateHitBox)
            {
                if (isDoingParry)
                {
                    if(shieldCollider != null)
                    {
                        shieldCollider.enabled = true;
                    }
                    else
                    {
                        if(weaponCollider != null)
                            weaponCollider.enabled = true;
                    } 
                    canParry = true;
                }
                else
                {
                    if(weaponCollider != null)
                        weaponCollider.enabled = true;
                }
            }
            else
            {
                if(weaponCollider != null)
                    weaponCollider.enabled = false;
                if(shieldCollider != null)
                    shieldCollider.enabled = false;
                if (isDoingParry)
                {
                    canParry = false;
                }
            }

            if (animManager.Handle.CameraShaked && !shakeCamera)
            {
                shakeCamera = true;
                cameraSettings.ShakeCamera(animManager.Handle.CameraShakeValue);
            }
            else if (!animManager.Handle.CameraShaked)
            {
                shakeCamera = false;
            }


            if (animManager.Handle.Spawn && objectToSpawn && !spawned)
            {
                Destroy(objectSpawned);
                spawned = true;
                objectSpawned = Instantiate(objectToSpawn, this.transform);
                objectSpawned.transform.localPosition = positionToSpawnObject;
                objectSpawned.transform.localRotation = Quaternion.identity;
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
    public float DamageToDeal()
    {
        return equippedWeapon.Damage + (playerStats.TotalStrength / 3) + (playerStats.TotalDexterity /6) + bonusCheat;
    }
    public bool PerformParry()
    {
        if(!canParry) return false;
        parryVfx.Play();
        canParry = false;
        StartCoroutine(DoTimeStop());
        audioManager.PlayAudio(timeStop);
        return true;
    }
    private IEnumerator DoTimeStop()
    {
        float finalTimeScale = parryTimeScale * combatModifiers.timeScaleMultiplier;

        float finalDuration = parryFreezeDuration * combatModifiers.durationMultiplier;

        float finalRadius = parryRadius * combatModifiers.radiusMultiplier;
            
        // Find enemies in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, finalRadius, enemyLayer);
        List<IEnemyTimeAffectable> affectedEnemies = new List<IEnemyTimeAffectable>();
        foreach (var col in hits)
        {
            if (col.TryGetComponent<Health>(out var enemy))
            { 
                enemy.GetHit(0);
            }
        }

        yield return new WaitForSecondsRealtime(parryDelay);

        foreach (var col in hits)
        {
            if (col.TryGetComponent<IEnemyTimeAffectable>(out var enemy))
            {
                
                enemy.SetTimeScale(finalTimeScale); // Slow down ONLY this enemy
                affectedEnemies.Add(enemy);
            }
        }

        // Optional: activate screen effect
        if (Camera.main.TryGetComponent<TimeStopEffect>(out var effect))
        {
            effect.Activate(parryFreezeDuration);
        }

        yield return new WaitForSecondsRealtime(finalDuration); // Wait in real time

        // Restore enemy speeds
        foreach (var enemy in affectedEnemies)
        {
            enemy.SetTimeScale(1f);
        }
    }

    // ----------------------------
    // Input bindings
    // ----------------------------
    public void OnLightAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        HandleAttackInput(equippedWeapon?.AnimationsData?.LightAttack);
    }
    public void OnHeavyAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        HandleAttackInput(equippedWeapon?.AnimationsData?.HeavyAttack);
    }
    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        HandleAttackInput(inventory?.GetItemOnArmourSlot()?.Animation);
    }
    public void OnParryAttack(InputAction.CallbackContext ctx)
    {
        parryCalled = true;
        if (equippedShield)
        {
            HandleAttackInput(equippedShield?.AnimationsData?.Parry);
        }
        else
        {
            HandleAttackInput(equippedWeapon?.AnimationsData?.Parry);
        }                   
    }
    public void OnDefend(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) holdDefend = true;
        else if (ctx.canceled) holdDefend = false;
    }

    private void HandleDefense()
    {
        // Só defende se: estiver a premir o botão, tiver escudo E não estiver a atacar
        if (holdDefend && equippedShield && !isAttacking)
        {
            if (!isDefending) // Evita chamar o SetBool repetidamente se já estiver a defender
            {
                anim.SetBool("Defend", true);
                isDefending = true;
            }
        }
        else
        {
            anim.SetBool("Defend", false);
            isDefending = false;
        }
        playerState.HandleDefense(isDefending);
    }
    public void OnConsumable()
    {
        HandleAttackInput(inventory?.GetItemOnConsumablesSlot()?.Animation);
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

    private void HandleAnimStepStarted(int stepIndex)
    {
        // Snap only if the camera is locked (same check you used before)
        if (playerController == null) return;


        // Optionally: only snap if this animation is an attack
        // If you want to limit to attack animations only:
        if (animManager.CurrentAnimation != null && !animManager.CurrentAnimation.IsAttackAnimation) 
            return;

        playerController.SnapRotateToTarget();
    }
    private void HandleAttackInput(CombatAnimations animData)
    {
        if (animData == null || !health.CanAttack() || !playerState.PlayerCanMove) return;

        if (inventoryManager != null)
            if(inventoryManager.IsActive)
                return;
        
        bool isParryAttack = false;
        

        // Detect if this is the parry animation
        if (animData.IsAttackAnimation)
        {
            if (parryCalled && equippedShield)
            {
                isParryAttack = animData == equippedShield.AnimationsData.Parry;  
            }
            else
            {
                isParryAttack = animData == equippedWeapon.AnimationsData.Parry;    
            }
            parryCalled = false;
                    
        }

        if (!animManager.IsPlaying && stamina.StaminaValue >= staminaToWastePerAttack 
            && !animManager.QueuedNext && !isAttacking)
        {
            if(animManager.Handle != null)
                if(animManager.Handle.IsFadingOut || animManager.Handle.IsFadingIn)
                    return;

            Debug.Log("attack called");
            animManager.Play(animData);
                
            if (animData.SpawnObject)
            {
                objectToSpawn = animData.ObjectToSpawn;
                positionToSpawnObject = animData.PositionToSpawnObject;
            }
            else
            {
                positionToSpawnObject = new Vector3();
                objectToSpawn = null;
            }
            isAttacking = true;

            // Set parry flag
            isDoingParry = isParryAttack;
            canParry = isParryAttack;

            stamina.TakeStamina(staminaToWastePerAttack);
            spawned = false; 
            return;
        }

        if (!attackQueued && stamina.StaminaValue >= staminaToWastePerAttack 
            && !animManager.QueuedNext && !animManager.Handle.IsFadingOut && !animManager.Handle.IsBlending)
        {
            Debug.Log("queue called");
            attackQueued = true;
            bufferTimer = inputBufferTime;
            animManager.TryQueueNextStep();

            if (animManager.QueuedNext)
            {
                stamina.TakeStamina(staminaToWastePerAttack);
                spawned = false; 
            }

            // If queued animation is parry
            if (isParryAttack)
                isDoingParry = true;
                canParry = true;
        }
        
    }


    public void ResetCombatState()
    {
        animManager.Stop();
        isAttacking = false;
        attackQueued = false;
        isDoingParry = false;
        canParry = false;
        bufferTimer = 0;
    }
}
