using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject inventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closeButton.onClick.AddListener(CloseInventory);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            print("Espaço clicado");
            inventory.SetActive(!inventory.activeSelf);
            button.SetActive(!button.activeSelf);
        }
    }

    private void CloseInventory()
    {
        inventory.SetActive(false);
        button.SetActive(false);
    }
}
