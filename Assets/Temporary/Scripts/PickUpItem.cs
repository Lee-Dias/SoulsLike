using NaughtyAttributes.Test;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private Item objectToGive;
    private Inventory inventory;
    private bool playerInside = false;
    private PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CanPickUp(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (playerInside)
        {
            inventory.SpawnInventoryItem(objectToGive);
            playerController.ChangeInteractionMessageState(false);
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerInside = true;
            inventory = tag.GetComponent<Inventory>();
            playerController.ChangeInteractionMessageState(true);
        }
    }

    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerInside = false;
            inventory = null;
            playerController.ChangeInteractionMessageState(true);
        }
    }
}
