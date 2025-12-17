using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Metadata;

public class BonfireWorldChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private string sceneToLoad = "SampleSceneDark"; // Change this or set in Inspector
    [SerializeField] private GameObject bonfireUI;
    private bool playerInside = false;
    private RectTransform loadingImageRectTransform;
    private GameObject loadingImageGameObject;


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Transform loadingImageTransform = bonfireUI.transform.Find("LoadingImage");
        loadingImageGameObject = bonfireUI.transform.Find("LoadingImage").gameObject;

        if (loadingImageTransform != null)
        {
            loadingImageRectTransform = loadingImageTransform as RectTransform;
            // Now use childYouWant however you need
            // Example: childYouWant.SetActive(true);
        }
        else
        {
            Debug.LogError("Child with name 'LoadingImage' not found under bonfireUI!");
        }
    }

    // Call this method to load the scene (e.g., from a button click, collision, etc.)
    public void LoadTargetScene()
    {
        loadingImageGameObject.SetActive(true);
        StartCoroutine(GrowAndRotateOverTime(20));
    }

    private IEnumerator GrowAndRotateOverTime(float duration) //isto tem de ser alterado
    {
        float growAmountPerSecond = 200f;             // how much wider/taller it gets per second
        float rotationSpeed = 360f + 180f;
        float elapsedTime = 0f;// degrees per second (positive = clockwise)
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            loadingImageRectTransform.sizeDelta = loadingImageRectTransform.sizeDelta + new Vector2(growAmountPerSecond, growAmountPerSecond) * Time.deltaTime;

            loadingImageRectTransform.rotation = Quaternion.Euler(0, 0, loadingImageRectTransform.eulerAngles.z + rotationSpeed * Time.deltaTime);

            yield return null;
        }
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
