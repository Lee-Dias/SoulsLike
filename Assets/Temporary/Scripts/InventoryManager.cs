using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject inventory;
    [SerializeField] private List<GameObject> extras = new List<GameObject>();

    private PlayerController playerController;
    private bool isActive = false;
    public bool IsActive => isActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        closeButton.onClick.AddListener(CloseInventory);
    }

    public void OnInventory(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || playerController.IsOnBonfire) return;
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
            playerController.ChangeIsInInventoryState(true);
            playerController.PlayerCanMoveState(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerController.ChangeIsInInventoryState(false);
            playerController.PlayerCanMoveState(true);
        }
        button.SetActive(isActive);
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
        button.SetActive(isActive);
        foreach(GameObject extra in extras)
        {
            extra.SetActive(isActive);
        }
    }
}
