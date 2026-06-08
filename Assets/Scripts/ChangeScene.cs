using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    private PlayerState playerState;
    private bool playerInside;

    private void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
    }
    public void InteractWithLastBonfire(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (playerInside)
        {
            SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
        }

    }
    private void OnTriggerEnter(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerState.ChangeInteractionMessageState(true);
            playerInside = true;
        }
    }
    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player")){
            playerState.ChangeInteractionMessageState(false);
            playerInside = false;
        }
    }
}
