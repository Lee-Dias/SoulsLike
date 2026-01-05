using UnityEngine;

public class LiminalManager : MonoBehaviour
{

    private PlayerController playerController;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    // Call this method to load the scene (e.g., from a button click, collision, etc.)
    public void LoadTargetScene()
    {
        bool targetWorldIsNormal = !playerController.playerBonfire.IsNormalWorld;

        LiminalWorldChanger[] allBonfires =
            FindObjectsOfType<LiminalWorldChanger>(true);

        foreach (var bonfire in allBonfires)
        {
            if (bonfire.BonfireID == playerController.playerBonfire.BonfireID &&
                bonfire.IsNormalWorld == targetWorldIsNormal)
            {
                TeleportPlayer(bonfire.SpawnPoint);
                return;
            }
        }

        Debug.LogError("Matching bonfire not found!");
        
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
}
