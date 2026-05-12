using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateMiniBoss : MonoBehaviour
{
    [SerializeField] private MeleeEnemyAI meleeEnemyAI;
    [SerializeField] private Health health;
    [SerializeField] private GameObject enemyCanvas;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Animator doorsAnimator;
    [SerializeField] private GameObject vfxStart;
    [SerializeField] private Transform vfxPostion;
    [SerializeField] private float timeToActivateAfter = 1f;
    [SerializeField] private AudioClip audioToPlay;

    private AudioManager audioManager;
    private PlayerState playerState;
    private PlayerController playerController;
    private bool playerInside = false;
    private bool done;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
        playerController = FindFirstObjectByType<PlayerController>();
        audioManager = FindFirstObjectByType<AudioManager>();

    }

    public void CanPickUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !playerInside) return;

        if ((!playerState.EnemyAround ) && !playerState.playerIsInBonfire && !playerState.IsOnBonfire)
        {
            Instantiate(vfxStart, vfxPostion.position, Quaternion.identity);
            if(doorsAnimator != null)
                doorsAnimator.SetTrigger("Close");
            audioManager.PlayAudio(audioToPlay, null , 0.4f);
            done = true;
            playerState.ChangeInteractionMessageState(false);
            enemyAnimator.SetTrigger("Start");
            
            StartCoroutine(EnableEnemyAfterDelay());
        }
    }

    private IEnumerator EnableEnemyAfterDelay()
    {
        yield return new WaitForSeconds(timeToActivateAfter);
        
        enemy.layer = LayerMask.NameToLayer("Enemy");
        meleeEnemyAI.enabled = true;
        enemyCanvas.SetActive(true);
        
        Destroy(this);
    }
    private void OnTriggerEnter(Collider tag)
    {

        if (tag.CompareTag("Player"))
        {
            playerInside = true;
            if (!playerState.EnemyAround && playerState.playerIsInBonfire && !playerState.IsOnBonfire)
            {
                playerState.ChangeInteractionMessageState(true);
            }
        }
    }
    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerInside = false;
            playerState.ChangeInteractionMessageState(false);
        }
    }
    private void Update()
    {
        if (done)
        {
            
            return;
        }
        
        if (playerInside)
        {


            if (!playerState.EnemyAround && !playerState.playerIsInBonfire && !playerState.IsOnBonfire)
            {
                playerState.ChangeInteractionMessageState(true);
            }
            else
            {
                playerState .ChangeInteractionMessageState(false);
            }
        }
    }
}
