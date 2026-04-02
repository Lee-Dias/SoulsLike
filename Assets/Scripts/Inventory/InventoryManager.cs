using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    [SerializeField] private List<GameObject> extras = new List<GameObject>();

    private PlayerState playerState;
    private bool isActive = false;
    public bool IsActive => isActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
    }

    public void OnInventory(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || playerState.IsOnBonfire || playerState.IsInSettings) return;
        UpdateSetActives();

    }
    // Update is called once per frame
    public void UpdateSetActives()
    {
        isActive = !isActive;
        if (isActive)
        {            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerState.ChangeIsInInventoryState(true);
            playerState.PlayerCanMoveState(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerState.ChangeIsInInventoryState(false);
            playerState.PlayerCanMoveState(true);
        }
        inventory.SetActive(isActive);
        foreach (GameObject extra in extras)
        {
            extra.SetActive(isActive);
        }
    }

    private void CloseInventory()
    {
        isActive = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inventory.SetActive(isActive);
        foreach(GameObject extra in extras)
        {
            extra.SetActive(isActive);
        }
    }
}
