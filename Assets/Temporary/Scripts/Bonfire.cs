using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BonfireWorldChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int bonfireID;
    [SerializeField] private bool isNormalWorld;
    [SerializeField] private GameObject bonfireUI;
    [SerializeField] private Transform SpawnPoint;
    private bool playerInside = false;

    // Call this method to load the scene (e.g., from a button click, collision, etc.)
    public void LoadTargetScene()
    {
        bool targetWorldIsNormal = !isNormalWorld;

        BonfireWorldChanger[] allBonfires =
            FindObjectsOfType<BonfireWorldChanger>(true);

        foreach (var bonfire in allBonfires)
        {
            if (bonfire.bonfireID == bonfireID &&
                bonfire.isNormalWorld == targetWorldIsNormal)
            {
                TeleportPlayer(bonfire.GetSpawnPoint());
                return;
            }
        }

        Debug.LogError("Matching bonfire not found!");
        
    }
    public Transform GetSpawnPoint()
    {
        return SpawnPoint;
    }
    private void TeleportPlayer(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint is missing!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        player.transform.SetPositionAndRotation(
            spawnPoint.position,
            spawnPoint.rotation
        );

        if (cc != null)
            cc.enabled = true;
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
