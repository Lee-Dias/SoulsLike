using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Item itemToGive;
    [SerializeField] private BoxCollider triggerCollider;

    private PlayerState playerState;
    private Inventory inventory;

    private bool isIn;
    private bool hasBeenOpened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
        inventory = FindFirstObjectByType<Inventory>();
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
        inventory.SpawnInventoryItem(itemToGive);
        if (ItemPickedUp.Instance != null) {
                ItemPickedUp.Instance.ShowItem(itemToGive);
        }

        //itemToSpawn.SetActive(true);
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
