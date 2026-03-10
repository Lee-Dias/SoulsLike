using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
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
    private BoxCollider weaponCollider;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerController playerController;
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
    [SerializeField] private AudioClip lightSwoosh;
    [SerializeField] private AudioClip HeavySwoosh;
    [SerializeField] private AudioClip parryHit;
    [SerializeField] private AudioClip timeStop;


    private AudioManager audioManager;
    private CameraSettings cameraSettings;
    

    private Item equippedWeapon;

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

    private GameObject instantiatedWeapon;
    private GameObject currentWeaponPrefab; 

    private GameObject objectToSpawn; 
    private Vector3 positionToSpawnObject; 
    private bool spawned = false;
    private bool shakeCamera;

    public bool IsAttacking => isAttacking;
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
    }

    public void EnablePlayerAfterSitting()
    {
        liminalUIController.DisableSittingAfterDelay();
    }

    private void Update()
    {
        equippedWeapon = inventory.GetItemOnRightHand();
        if (equippedWeapon != null)
        {
            // Check if the NEW prefab is different from the OLD prefab
            if (equippedWeapon.Weapon != currentWeaponPrefab) 
            {
                // 1. New weapon detected: Destroy the old instance(s)
                foreach (Transform child in weaponHolder.transform)
                {
                    Destroy(child.gameObject);
                }

                // 2. Instantiate the new weapon
                instantiatedWeapon = Instantiate(
                    equippedWeapon.Weapon,
                    weaponHolder.transform // To retain prefab's local position/rotation if it has one
                ); 
                instantiatedWeapon.GetComponent<Attack>().SetCharacther(this.gameObject);
                // 3. IMPORTANT: Update the reference variable to the NEW prefab
                currentWeaponPrefab = equippedWeapon.Weapon; 

                // 4. Get components
                weaponCollider = instantiatedWeapon.GetComponent<BoxCollider>();
            }
            // else: The equippedWeapon is the same as the currentWeaponPrefab, do nothing.
        }
        else 
        {
            // If nothing is equipped, destroy all children
            foreach (Transform child in weaponHolder.transform)
            {
                Destroy(child.gameObject);
            }
            // Also clear the reference to the last equipped prefab
            currentWeaponPrefab = null; 
        }

        
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
            if (animManager.Handle.ActivateHitBox)
            {
                if(weaponCollider != null)
                    weaponCollider.enabled = true;
                if (isDoingParry)
                {
                    canParry = true;
                }
            }
            else
            {
                if(weaponCollider != null)
                    weaponCollider.enabled = false;
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
        return equippedWeapon.Damage + (playerStats.TotalStrength / 3) + (playerStats.TotalDexterity /6);
    }
    public bool PerformParry()
    {
        if(!canParry) return false;
        audioManager.PlayAudio(parryHit);
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

        HandleAttackInput(equippedWeapon?.AnimationsData?.LightAttack, lightSwoosh);
    }
    public void OnHeavyAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        HandleAttackInput(equippedWeapon?.AnimationsData?.HeavyAttack, HeavySwoosh);
    }
    public void OnSpecialAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        HandleAttackInput(inventory?.GetItemOnArmourSlot()?.Animation);
    }
    public void OnParryAttack(InputAction.CallbackContext ctx)
    {
        
        HandleAttackInput(equippedWeapon?.AnimationsData?.Parry);
    }
    public void OnConsumable(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
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
    private void HandleAttackInput(CombatAnimations animData, AudioClip audioClip = null)
    {
        if (animData == null || !health.CanAttack() || !playerController.PlayerCanMove) return;

        if (inventoryManager != null)
            if(inventoryManager.IsActive)
                return;
        
        bool isParryAttack = false;
          

        // Detect if this is the parry animation
        if (animData.IsAttackAnimation)
        {
            isParryAttack = animData == equippedWeapon.AnimationsData.Parry;            
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


    private void ResetCombatState()
    {
        animManager.Stop();
        isAttacking = false;
        attackQueued = false;
        isDoingParry = false;
        canParry = false;
        bufferTimer = 0;
    }
}
