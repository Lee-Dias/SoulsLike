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
    private bool isActive = false;
    public bool IsActive => isActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeButton.onClick.AddListener(CloseInventory);
    }

    public void OnInventory(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
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
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
