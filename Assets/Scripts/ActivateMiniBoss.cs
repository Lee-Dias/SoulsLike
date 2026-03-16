using System.Collections;
using UnityEditor.Animations;
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
    [SerializeField] private float timeToActivateAfter = 2f;
    private PlayerState playerState;
    private PlayerController playerController;
    private bool playerInside = false;
    private bool done;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    public void CanPickUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !playerInside) return;

        if ((!playerState.EnemyAround ) && !playerController.playerIsInBonfire && !playerController.IsOnBonfire)
        {
            Instantiate(vfxStart, vfxPostion.position, Quaternion.identity);
            if(doorsAnimator != null)
                doorsAnimator.SetTrigger("Close");
            done = true;
            playerController.ChangeInteractionMessageState(false);
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
            if (!playerState.EnemyAround && playerController.playerIsInBonfire && !playerController.IsOnBonfire)
            {
                playerController.ChangeInteractionMessageState(true);
            }
        }
    }
    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerInside = false;
            playerController.ChangeInteractionMessageState(false);
        }
    }
    private void Update()
    {
        if (done) return;
        if (playerInside)
        {
            if (!playerState.EnemyAround && !playerController.playerIsInBonfire && !playerController.IsOnBonfire)
            {
                playerController.ChangeInteractionMessageState(true);
            }
            else
            {
                playerController.ChangeInteractionMessageState(false);
            }
        }
    }
}
