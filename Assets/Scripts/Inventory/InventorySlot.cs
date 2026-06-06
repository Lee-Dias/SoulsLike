using UnityEngine;
using UnityEngine.EventSystems;
using static Item;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{		
    public InventoryItem myItem { get; set; }   

	public ItemType myType;

    private InfoMouseFollowerUI mouseFollower;
    private TextMeshProUGUI mouseText;

    private void Awake()
    {
        if(mouseFollower == null || mouseText == null)
        {
            mouseFollower = GameObject.Find("MouseFollower").GetComponent<InfoMouseFollowerUI>();
            mouseText = mouseFollower.GetComponentInChildren<TextMeshProUGUI>();
        }
    }


    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            print("Childs: " + transform.childCount + "\nI am: " + name);

            if (transform.childCount == 1) return;
            if (Inventory.carriedItem == null) return;
            if(myType != ItemType.None && Inventory.carriedItem.myItem.itemTypePublic != myType) return;
            
            SetItem(Inventory.carriedItem);
        }
    }

    public void SetItem(InventoryItem item)
    {
        Destroy(item.imageObj);
        Inventory.carriedItem = null;

        //reset old slot
        item.activeSlot.myItem = null;

        //set current slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        if (myItem.canvasGroup != null)
        {
            myItem.canvasGroup.blocksRaycasts = true;
        }

        if(myType != ItemType.None)
        {
            Inventory.Singleton.EquipEquipment(myType, myItem);
        }
        EquipmentManager.Singleton.UpdateUpdateEquippedAura();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (myItem == null) return;
        mouseFollower.isHoveringItem = true;
        mouseText.text = myItem.myItem.ItemName;
        print("\nHovering " + myItem.myItem.ItemName + "\n\n\n");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseFollower.isHoveringItem = false;
        mouseText.text = "";
    }
}
