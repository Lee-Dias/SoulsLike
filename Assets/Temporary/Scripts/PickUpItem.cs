using NaughtyAttributes.Test;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    Inventory itemSpawner;
    private bool playerInside = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemSpawner = targetObject.GetComponent<Inventory>();
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
            itemSpawner.SpawnInventoryItem();
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
