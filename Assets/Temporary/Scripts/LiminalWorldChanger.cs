using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LiminalWorldChanger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int bonfireID;
    [SerializeField] private bool isNormalWorld;
    [SerializeField] private Transform spawnPoint;

    public int BonfireID => bonfireID;
    public bool IsNormalWorld => isNormalWorld;
    public Transform SpawnPoint => spawnPoint;


    private PlayerController playerController;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnTriggerEnter(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            
            playerController.playerIsInBonfire = true;
            playerController.playerBonfire = this;
            playerController.CheckInteractionMessageState();
        }
    }

    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            
            playerController.playerIsInBonfire = false;
            playerController.playerBonfire = null;
            playerController.CheckInteractionMessageState();
        }
    }
}
