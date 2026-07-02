using UnityEngine;
using UnityEngine.InputSystem;

public class LiminalDoor : MonoBehaviour
{
    [SerializeField] private OpenDoor openDoor;

    private PlayerState playerState;
    private Inventory inventory;

    private bool isIn;
    private bool hasBeenOpened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
        inventory = FindFirstObjectByType<Inventory>();
        isIn = false;
    }

    // Update is called once per frame
    public void OnOpen(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !isIn) return;
        if (hasBeenOpened) return;
        playerState.ChangeInteractionMessageState(false);
        hasBeenOpened = true;
        openDoor.Open();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(hasBeenOpened) return;
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
