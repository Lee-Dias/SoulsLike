using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    Image itemIcon;
    public CanvasGroup canvasGroup { get; private set; }

    public Item myItem { get; set; }
    public InventorySlot activeSlot { get; set; }

    Sprite iconSaver;

    private Inventory inventory;

    public bool canRemove = true;

    public GameObject imageObj;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        itemIcon = GetComponent<Image>();
        
        if(iconSaver != null)
            itemIcon.sprite = iconSaver;

        inventory = FindAnyObjectByType<Inventory>();
    }

    public void Initialize(Item item, InventorySlot parent)
    {
        activeSlot = parent;
        activeSlot.myItem = this;
        myItem = item;
        if (itemIcon == null)
        {
            iconSaver = item.Icon;
        }
        else
        {
            itemIcon.sprite = item.Icon;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Temporary
        if (!canRemove) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Inventory.Singleton.SetCarriedItem(this);
            
                CreateImage("Images/Border");
            
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            this.activeSlot.myItem = null;
            inventory.SpawnInventoryItem(myItem,false);
            Destroy(this.gameObject);
        }
    }

    public void CreateImage(string imagePath)
    {
        // Create a new GameObject as a child
        imageObj = new GameObject("ImageChild");
        imageObj.transform.SetParent(transform, false);

        // Add Image component
        Image image = imageObj.AddComponent<Image>();

        // Load the sprite from path (must be inside a Resources folder)
        Sprite sprite = Resources.Load<Sprite>(imagePath);

        if (sprite != null)
            image.sprite = sprite;
        else
            print("Image not found at path: " + imagePath);
    }
}
