using UnityEngine;
using UnityEngine.SceneManagement;


public class Health : MonoBehaviour
{
    [SerializeField] private float cooldownPerGetHit = 0.5f;
    [SerializeField] private float cooldownToAttack = 0.7f;
    [SerializeField] private float blockMovementFor = 0.2f;
    [SerializeField] private CharacterProfileBase characterProfile;
    [SerializeField] private float statsMultiplier = 1.5f; 
    [SerializeField] private float baseMaxHealth = 100f; 

    private float maxHealth;
    private float health;
    private Animator animator;
    private float timePassedSinceLastHit;
    private float timePassedSinceLastHitForAttack;
    private float timePassedSinceLastBlockMovement;

    void Start()
    {
        maxHealth += baseMaxHealth + (characterProfile.BaseVitlaity * statsMultiplier);
        health = maxHealth;

        animator = GetComponent<Animator>();
        timePassedSinceLastHit = cooldownPerGetHit;
        timePassedSinceLastHitForAttack = cooldownToAttack; 
    }
    private void FixedUpdate()
    {
        timePassedSinceLastHit += Time.deltaTime;
        timePassedSinceLastHitForAttack += Time.deltaTime;
        timePassedSinceLastBlockMovement += Time.deltaTime;
    }

    public void GetHit(float damage)
    {

        if (cooldownPerGetHit <= timePassedSinceLastHit )
        {
            health -= damage;
            if (health <= 0)
            {
                if (this.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Scene currentScene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(currentScene.name);
                }
                Destroy(this.gameObject);
                
            }
            animator.SetTrigger("GetHit");
            timePassedSinceLastHit = 0;
            timePassedSinceLastHitForAttack = 0;
            timePassedSinceLastBlockMovement = 0;
        }
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
