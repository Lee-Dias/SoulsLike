using System.Collections.Generic;
using UnityEngine;

public class AuraInventory : MonoBehaviour
{
    [SerializeField] private GameObject auraMainSlotParent;
    [SerializeField] private GameObject auraSlotsParent;
    [SerializeField] private GameObject auraSlotPrefab;
    [SerializeField] private InventoryItem itemPrefab;

    private List<InventorySlot> auraSlots = new();

    public void SpawnAuraItem(Item item)
    {
        InventorySlot slot = CreateAuraSlot();
        Instantiate(itemPrefab, slot.transform)
            .Initialize(item, slot);
    }

    private InventorySlot CreateAuraSlot()
    {
        GameObject slotGO = Instantiate(auraSlotPrefab, auraSlotsParent.transform);
        InventorySlot slot = slotGO.GetComponent<InventorySlot>();

        // Restrict this slot to Aura items
        slot.myType = Item.ItemType.Aura;

        auraSlots.Add(slot);
        return slot;
    }
    public Item GetAuraEquipped()
    {
        foreach (Transform child in auraMainSlotParent.transform)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null && slot.myItem != null)
            {
                return slot.myItem.myItem;
            }
        }
        return null;
    }
}
