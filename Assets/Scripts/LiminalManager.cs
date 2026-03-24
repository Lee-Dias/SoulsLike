using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LiminalManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string normalWorldName;
    [SerializeField] private string darkWorldName;
    [SerializeField] private Material normalWorldSkyBox;
    [SerializeField] private Material darkWorldSkyBox;
    private PlayerController playerController;
    private GameObject normalWorldLight;
    private GameObject darkWorldLight;
    private GameObject normalWorld;
    private GameObject darkWorld;


    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        StartCoroutine(LoadScenesAndTeleport());
    }

    IEnumerator LoadScenesAndTeleport()
    {
        // Start loading both scenes at the same time
        AsyncOperation normalLoad =
            SceneManager.LoadSceneAsync(normalWorldName, LoadSceneMode.Additive);

        AsyncOperation darkLoad =
            SceneManager.LoadSceneAsync(darkWorldName, LoadSceneMode.Additive);

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
        TeleportPlayerToStartingPoint();
    }

    private void TeleportPlayerToStartingPoint()
    {

        int spawnLayer = LayerMask.NameToLayer("SpawnPoint");
        normalWorld = GameObject.FindWithTag("NormalWorld");
        darkWorld = GameObject.FindWithTag("DarkWorld");
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
        
        ChangeEnviorment();
        if (cc != null)
            cc.enabled = true;
    }

    public void ChangeEnviorment()
    {
        normalWorldLight = GameObject.FindWithTag("NormalLight");
        darkWorldLight = GameObject.FindWithTag("DarkLight");
        Light light;
        Material material;
        if (playerController.playerBonfire != null)
        {
            if (!playerController.playerBonfire.IsNormalWorld)
            {
                light = GameObject.FindWithTag("DarkLight").GetComponent<Light>();
                material = darkWorldSkyBox;
                darkWorldLight.GetComponent<Light>().enabled = true;
                normalWorldLight.GetComponent<Light>().enabled = false; 
                darkWorld.SetActive(true);
                normalWorld.SetActive(false);
            }else
            {
                light = GameObject.FindWithTag("NormalLight").GetComponent<Light>();     
                material = normalWorldSkyBox;   
                darkWorldLight.GetComponent<Light>().enabled = false;
                normalWorldLight.GetComponent<Light>().enabled = true;  
                darkWorld.SetActive(false);
                normalWorld.SetActive(true);
                
            }
        }
        else
        {
            light = GameObject.FindWithTag("NormalLight").GetComponent<Light>();     
            material = normalWorldSkyBox; 
            darkWorldLight.GetComponent<Light>().enabled = false;
            normalWorldLight.GetComponent<Light>().enabled = true; 
            darkWorld.SetActive(false);
            normalWorld.SetActive(true);
                 
        }
        RenderSettings.sun = light;
        RenderSettings.skybox = material;
        DynamicGI.UpdateEnvironment();
    }
}
