using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportInSameRealm : MonoBehaviour
{

    [SerializeField] private GameObject positionToTeleport;
    [SerializeField] private GameObject[] sceneToActivate;
     
    private Teleport teleportScript;

    private void OnTriggerEnter(Collider other)
    {
        TeleportPlayer();
    }
    public void TeleportPlayer()
    {
        teleportScript = FindFirstObjectByType<Teleport>();
        foreach (GameObject scene in sceneToActivate)
        {
            scene.SetActive(true);
        }
        teleportScript.TeleportPlayer(positionToTeleport.transform);
        
    } 
    public void OnBoss(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TeleportPlayer();
        Debug.Log("Boss");
    }


}
