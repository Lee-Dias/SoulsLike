using UnityEngine;
using UnityEngine.EventSystems;
using static Item;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem myItem { get; set; }

    public ItemType myType;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (Inventory.carriedItem == null) return;
            if(myType != ItemType.None && Inventory.carriedItem.myItem.itemTypePublic != myType) return;
            SetItem(Inventory.carriedItem);
        }
    }

    public void SetItem(InventoryItem item)
    {
        Inventory.carriedItem = null;

        //reset old slot
        item.activeSlot.myItem = null;

        //set current slot
        myItem = item;
        myItem.activeSlot = this;
        myItem.transform.SetParent(transform);
        myItem.canvasGroup.blocksRaycasts = true;

        if(myType != ItemType.None)
        {
            Inventory.Singleton.EquipEquipment(myType, myItem);
        }
    }
}
