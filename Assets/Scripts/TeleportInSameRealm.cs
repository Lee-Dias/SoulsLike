using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportInSameRealm : MonoBehaviour
{

    [SerializeField] private GameObject positionToTeleport;
    [SerializeField] private GameObject[] sceneToActivate;
     
    private Teleport teleportScript;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
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
        StartCoroutine(SetYawDelayed());
    }

    private IEnumerator SetYawDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        CameraSettings cam = FindFirstObjectByType<CameraSettings>();
        if (cam != null)
        {
            cam.SetYaw(positionToTeleport.transform.eulerAngles.y);
        }
    }
    public void OnBoss(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TeleportPlayer();
    }


}
