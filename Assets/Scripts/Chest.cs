using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private GameObject itemToSpawn;
    [SerializeField] private BoxCollider triggerCollider;

    private PlayerState playerState;

    private bool isIn;
    private bool hasBeenOpened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
        animator = GetComponent<Animator>();
        isIn = false;
    }

    // Update is called once per frame
    public void OnOpen(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !isIn) return;
        if (hasBeenOpened) return;
        triggerCollider.enabled = false;
        playerState.ChangeInteractionMessageState(false);
        hasBeenOpened = true;
        animator.SetTrigger("Open");
        itemToSpawn.SetActive(true);
    }

    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isIn = true;
            playerState.ChangeInteractionMessageState(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isIn = false;
            playerState.ChangeInteractionMessageState(false);
        }
    }
}
