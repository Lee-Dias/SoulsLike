using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using Unity.Collections;


public class Health : MonoBehaviour
{
    [SerializeField] private float cooldownPerGetHit = 0.5f;
    [SerializeField] private float cooldownToAttack = 0.7f;
    [SerializeField] private float blockMovementFor = 0.2f;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float statsMultiplier = 1.5f; 
    [SerializeField] private float baseMaxHealth = 100f; 

    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float hurtVolume = 1f; 
    [SerializeField] private AudioClip hit; 
    [SerializeField] private Animator animateObject; 

    [SerializeField] private float playerRegenHp; 


    [SerializeField] private bool doDisolve; 
    [SerializeField] private float timeToDissolve; 

    [SerializeField] private GameObject activateAfterGetHit; 
    [SerializeField] private GameObject activateAfterDeath; 
    [SerializeField] private GameObject deActivateAfterDeath; 

    [Header("Soul")]
    [SerializeField] private GameObject soulPrefab;
    [SerializeField] private GameObject soulSpawnPoint;

    [SerializeField] private bool hasKnockBack = true;

    private PlayerState playerState;
    private AudioManager audioManager;

    private float maxHealth;
    private float health;
    private Animator animator;
    private float timePassedSinceLastHit;
    private float timePassedSinceLastHitForAttack;
    private float timePassedSinceLastBlockMovement;
    private bool healthScaled = false;
    private PlayerState playerstate;
    

    public float MaxHealth => maxHealth;
    public float HealthValue => health;

    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        playerstate = FindFirstObjectByType<PlayerState>();
        if (playerStats)
        {
            maxHealth += baseMaxHealth + (playerStats.TotalVitality * statsMultiplier);
        }
        else
        {
            maxHealth += baseMaxHealth ;
        }
            
        health = maxHealth;
        playerState = GetComponent<PlayerState>();
        animator = GetComponent<Animator>();
        timePassedSinceLastHit = cooldownPerGetHit;
        timePassedSinceLastHitForAttack = cooldownToAttack; 
    }

    public void Heal(float healAmount)
    {
        health += healAmount;
        health = Mathf.Clamp(health, 0f, maxHealth);
    }

    public void OnScaleHealth(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        if (playerStats == null) return;

        if (!healthScaled)
        {
            maxHealth *= 1000f;
            health *= 1000f;
            healthScaled = true;
        }
        else
        {
            maxHealth /= 1000f;
            health /= 1000f;
            healthScaled = false;
        }

        health = Mathf.Clamp(health, 0f, maxHealth);
    }


    public void OnRestartGame(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        SceneManager.LoadScene("ManagerScene", LoadSceneMode.Single);
    }

    private void FixedUpdate()
    {
        
        timePassedSinceLastHit += Time.deltaTime;
        timePassedSinceLastHitForAttack += Time.deltaTime;
        timePassedSinceLastBlockMovement += Time.deltaTime;
    }
    private void TakeDamage(float damage, bool shield)
    {
        damage = damage - (playerStats != null ? (playerStats.TotalDefense * 0.5f) : 0) - (playerState != null ? playerState.getAmountToDefend() : 0);
        if (damage < 0)
        {
            damage = 5;
        }
        health -= damage;
    }
    public void GetHit(float damage, bool hitShield )
    {
        if (activateAfterGetHit)
        {
            activateAfterGetHit.SetActive(true);
        }
        TakeDamage(damage, hitShield);
        if (health > 0 && hasKnockBack && playerState == null) animator.SetTrigger("GetHit");

        audioManager.PlayAudio(audioClip, null, 0 , hurtVolume, 0.9f, 1.1f);
        audioManager.PlayAudio(hit, null, 0 , 1, 0.9f, 1.1f);
        if (playerState != null) if(!playerstate.IsDefending()) animator.SetTrigger("GetHit");
        
        
        if (health <= 0)
        {
            if (this.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene("ManagerScene", LoadSceneMode.Single);
            }
            if (this.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (activateAfterDeath != null)
                {
                    activateAfterDeath.SetActive(true);
                }
                if(deActivateAfterDeath != null)
                {
                    deActivateAfterDeath.SetActive(false);
                }
                playerstate.DownEnemiesChasingPlayer();
                this.gameObject.layer = LayerMask.NameToLayer("Default");
                animator.SetTrigger("Die");
                FindFirstObjectByType<PlayerStats>().GiveCrystalShards(this.GetComponent<BaseEnemyAI>().AuraValue);
               if (TryGetComponent<MeleeEnemyAI>(out var meleeAI))
                {
                    meleeAI.enabled = false;
                }
                GetComponent<MeleeEnemyAI>()?.DeactivateCollider();
                GetComponent<CapsuleCollider>().enabled = false;
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                agent.ResetPath();
                GetComponent<NavMeshAgent>().enabled = false;
                
                if (activateAfterGetHit)
                {
                    activateAfterGetHit.SetActive(false);
                }
                //GetComponent<Animator>().enabled = false;
                //Destroy(GetComponentInChildren<Attack>().gameObject);
                //Destroy(GetComponentInChildren<Billboard>().gameObject);
                int a = 0;
                foreach (Transform child in transform)
                {
                    Destructible destructible = child.GetComponent<Destructible>();
                    
                    if (destructible != null)
                    {
                        a+= 1;
                        destructible.DestroyObject();
                    }
                }
                
                DestroyFloor destroyFloor = GetComponent<DestroyFloor>();
                if (destroyFloor != null)
                {
                    destroyFloor.DestroyFloorr();
                }
                
                if(animateObject != null)
                {
                    animateObject.SetTrigger("Open");
                }
                Instantiate(soulPrefab, soulSpawnPoint.transform.position, Quaternion.identity);
                if(doDisolve == true)
                {
                    StartCoroutine(DissolveOverTime());
                }else
                {
                    this.gameObject.SetActive(false);
                }
                
            }
            
        }
        
        
        timePassedSinceLastHit = 0;
        timePassedSinceLastHitForAttack = 0;
        timePassedSinceLastBlockMovement = 0;
    }

    IEnumerator DissolveOverTime()
    {
        float duration = 3.0f; // Strict 3 second window
        float currentTime = 0f;
        
        // 1. Collect all materials from children to avoid calling GetComponent every frame
        List<Material> materials = new List<Material>();

        // O "true" dentro dos parênteses serve para incluir objetos desativados (opcional)
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in allRenderers)
        {
            materials.Add(rend.material);
        }

        // 2. Loop until duration is met
        while (currentTime < timeToDissolve)
        {
            currentTime += Time.deltaTime;
            
            // Calculate progress (0 to 1)
            float progress = Mathf.Clamp01(currentTime / duration);

            // 3. Update all collected materials
            foreach (Material mat in materials)
            {
                mat.SetFloat("_DissolveAmount", progress);
            }

            yield return null; // Wait for the next frame
        }

        // 4. Ensure it lands exactly on 1.0 at the end
        foreach (Material mat in materials)
        {
            mat.SetFloat("_DissolveAmount", 1.0f);
            this.gameObject.SetActive(false);
        }
    }



    private void Update()
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (health < maxHealth)
            {
                health += playerRegenHp;
            }
        }
    }
    public bool CanHit()
    {
        return cooldownPerGetHit <= timePassedSinceLastHit;
    }
    public bool CanAttack()
    {
        if (cooldownToAttack <= timePassedSinceLastHitForAttack)
        {
            return true;
        }
        return false;
    }
    public bool ShouldBlockMovement()
    {
        if (blockMovementFor <= timePassedSinceLastBlockMovement)
        {
            return false;
        } 
        return true;
    }

    internal void ResetHealth()
    {
        health = maxHealth;
        if(animator != null)animator.ResetControllerState();
        
        //GetComponent<Rigidbody>().WakeUp();

        GetComponent<MeleeEnemyAI>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        

        this.gameObject.layer = LayerMask.NameToLayer("Enemy");

        List<Material> materials = new List<Material>();

        // O "true" dentro dos parênteses serve para incluir objetos desativados (opcional)
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in allRenderers)
        {
            materials.Add(rend.material);
        }

        foreach (Material mat in materials)
        {
            mat.SetFloat("_DissolveAmount", 0);
        }
    }
}
