using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BonfireWorldChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private string sceneToLoad = "SampleSceneDark"; // Change this or set in Inspector
    [SerializeField] private GameObject bonfireUI;
    private bool playerInside = false;

    // Call this method to load the scene (e.g., from a button click, collision, etc.)
    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void TurnOn(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (playerInside)
        {
            bonfireUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    public void TurnOff()
    {
        bonfireUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
