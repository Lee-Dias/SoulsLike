using UnityEngine;
using UnityEngine.InputSystem;

public class LiminalDoor : MonoBehaviour
{
    [SerializeField] private OpenDoor openDoor;
    [SerializeField] private AudioClip audio;

    private PlayerState playerState;
    private AudioManager audioManager;

    private bool isIn;
    private bool hasBeenOpened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
        audioManager = FindFirstObjectByType<AudioManager>();
        isIn = false;
    }

    // Update is called once per frame
    public void OnOpen(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !isIn) return;
        if (hasBeenOpened) return;
        playerState.ChangeInteractionMessageState(false);
        audioManager.PlayAudio(audio, null , 0 , 0.4f);
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
