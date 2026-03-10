using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LiminalManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string normalWorld;
    [SerializeField] private string darkWorld;
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        StartCoroutine(LoadScenesAndTeleport());
    }

    IEnumerator LoadScenesAndTeleport()
    {
        // Start loading both scenes at the same time
        AsyncOperation normalLoad =
            SceneManager.LoadSceneAsync(normalWorld, LoadSceneMode.Additive);

        AsyncOperation darkLoad =
            SceneManager.LoadSceneAsync(darkWorld, LoadSceneMode.Additive);

        // Optional: prevent automatic scene activation
        // normalLoad.allowSceneActivation = true;
        // darkLoad.allowSceneActivation = true;

        // Wait until BOTH scenes are fully loaded
        while (!normalLoad.isDone || !darkLoad.isDone)
        {
            yield return null;
        }

        // (Optional but recommended) Set an active scene
        Scene normalScene = SceneManager.GetSceneByName("normal");
        if (normalScene.IsValid())
        {
            SceneManager.SetActiveScene(normalScene);
        }

        // Now teleport the player
        TeleportPlayerToStartingBonfire();
    }

    private void TeleportPlayerToStartingBonfire()
    {

        int spawnLayer = LayerMask.NameToLayer("SpawnPoint");

        GameObject spawnPoint = FindObjectsOfType<GameObject>()
            .FirstOrDefault(obj => obj.layer == spawnLayer);

        TeleportPlayer(spawnPoint.transform);

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
                animator.SetTrigger("Change");
                StartCoroutine(TeleportAfterDelay(bonfire.SpawnPoint, 1f));
                return;
            }
        }

        Debug.LogError("Matching bonfire not found!");
        
    }
    private IEnumerator TeleportAfterDelay(Transform spawnPoint, float delay)
    {
        yield return new WaitForSeconds(delay);
        TeleportPlayer(spawnPoint);
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
