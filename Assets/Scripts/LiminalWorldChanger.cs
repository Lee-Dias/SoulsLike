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


    private PlayerState playerState;

    private void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
    }

    private void OnTriggerEnter(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            
            playerState.playerIsInBonfire = true;
            playerState.playerBonfire = this;
            playerState.bonfireLocation = gameObject.transform.position;
            playerState.CheckInteractionMessageState();
        }
    }

    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            
            playerState.playerIsInBonfire = false;
            playerState.playerBonfire = null;
            playerState.CheckInteractionMessageState();
        }
    }
}
