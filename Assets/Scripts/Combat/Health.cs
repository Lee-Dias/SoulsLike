using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.AI;


public class Health : MonoBehaviour
{
    [SerializeField] private float cooldownPerGetHit = 0.5f;
    [SerializeField] private float cooldownToAttack = 0.7f;
    [SerializeField] private float blockMovementFor = 0.2f;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private float statsMultiplier = 1.5f; 
    [SerializeField] private float baseMaxHealth = 100f; 

    [SerializeField] private AudioClip audioClip; 
    [SerializeField] private AudioClip hit; 

    private AudioManager audioManager;

    private float maxHealth;
    private float health;
    private Animator animator;
    private float timePassedSinceLastHit;
    private float timePassedSinceLastHitForAttack;
    private float timePassedSinceLastBlockMovement;
    private bool healthScaled = false;
    

    public float MaxHealth => maxHealth;
    public float HealthValue => health;

    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        if (playerStats)
        {
            maxHealth += baseMaxHealth + (playerStats.TotalVitality * statsMultiplier);
        }
        else
        {
            maxHealth += baseMaxHealth ;
        }
            

        health = maxHealth;

        animator = GetComponent<Animator>();
        timePassedSinceLastHit = cooldownPerGetHit;
        timePassedSinceLastHitForAttack = cooldownToAttack; 
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

    public void GetHit(float damage)
    {
        health -= damage;
        audioManager.PlayAudio(audioClip, null, 0 , 1, 0.9f, 1.1f);
        audioManager.PlayAudio(hit, null, 0 , 1, 0.9f, 1.1f);
        if (health <= 0)
        {
            if (this.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene("ManagerScene", LoadSceneMode.Single);
            }
            if (this.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                FindFirstObjectByType<PlayerStats>().GiveCrystalShards(this.GetComponent<BaseEnemyAI>().AuraValue);
                GetComponent<MeleeEnemyAI>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;
                Destroy(GetComponent<Rigidbody>());
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<Animator>().enabled = false;
                Destroy(GetComponentInChildren<Attack>().gameObject);
                Destroy(GetComponentInChildren<Billboard>().gameObject);
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
                Destroy(this.gameObject);
                
            }
            
        }
        
        animator.SetTrigger("GetHit");
        timePassedSinceLastHit = 0;
        timePassedSinceLastHitForAttack = 0;
        timePassedSinceLastBlockMovement = 0;
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
}
