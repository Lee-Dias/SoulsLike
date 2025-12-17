using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Item;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;
    
    
    [SerializeField] private GameObject inventorySlotsParent;
    [SerializeField] private GameObject inventorySlot;
    

    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] private Item[] items;

    [Header("In Hand")]
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject consumablesSlot;
    [SerializeField] private GameObject armourSlot;

    private List<InventorySlot> inventorySlots = new List<InventorySlot>();

    private Item[] rightHandItems = new Item[3];
    private Item[] leftHandItems = new Item[3];
    private Item[] consumableItems = new Item[3];
    private Item armourSet = null;

    private int rightSelectedItemNum = 0; 
    private int leftSelectedItemNum = 0;
    private int consumableSelectedItemNum = 0;
    
    void Awake()
    {
        Singleton = this;
    }

    private InventorySlot CreateInventorySlot()
    {
        GameObject slotGO = Instantiate(inventorySlot, inventorySlotsParent.transform);
        InventorySlot slot = slotGO.GetComponent<InventorySlot>();

        inventorySlots.Add(slot);
        return slot;
    }
    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;

        if (_item == null)
        {
            int random = Random.Range(0, items.Length);
            _item = items[random];
        }

        // Create a new slot
        InventorySlot newSlot = CreateInventorySlot();

        // Create the item in that slot
        Instantiate(itemPrefab, newSlot.transform)
            .Initialize(_item, newSlot);
    }

    // Update is called once per frame
    void Update()
    {
        if(carriedItem != null)
        {
            //carriedItem.transform.position = Input.mousePosition;
        }
        UpdateSlots();
    }
    public void UpdateSlots()
    {
        int r = 0;
        foreach(Transform item in rightHand.transform)
        {
            if(item.GetComponent<InventorySlot>() != null)
            {
                if(item.GetComponent<InventorySlot>().myItem != null)
                {
                    rightHandItems[r] = item.GetComponent<InventorySlot>().myItem.myItem;
                }else
                {
                    rightHandItems[r] = null;
                }
            }
            r+=1;
        }
        r= 0;
        foreach(Transform item in leftHand.transform)
        {
            if(item.GetComponent<InventorySlot>() != null)
            {
                if(item.GetComponent<InventorySlot>().myItem != null)
                {
                    leftHandItems[r] = item.GetComponent<InventorySlot>().myItem.myItem;
                }else
                {
                    leftHandItems[r] = null;
                }
            }
            r+=1;
        }
        r= 0;
        foreach(Transform item in consumablesSlot.transform)
        {
            if(item.GetComponent<InventorySlot>() != null)
            {
                if(item.GetComponent<InventorySlot>().myItem != null)
                {
                    consumableItems[r] = item.GetComponent<InventorySlot>().myItem.myItem;
                }else
                {
                    consumableItems[r] = null;
                }
            }
            r+=1;
        }
        if(armourSlot.GetComponentInChildren<InventorySlot>().myItem != null)
        {
            armourSet = armourSlot.GetComponentInChildren<InventorySlot>().myItem.myItem;
        }
        else
        {
            armourSet = null;
        }
    }

    public Item GetItemOnRightHand()
    {
        if (rightHandItems[rightSelectedItemNum] != null)
        {
            return rightHandItems[rightSelectedItemNum];
        }
        return null;    
    }
    public Item GetItemOnLeftHand()
    {
        if (leftHandItems[leftSelectedItemNum] != null)
        {
            return leftHandItems[leftSelectedItemNum];
        }
        return null;    
    }
    public Item GetItemOnConsumablesSlot()
    {
        if (consumableItems[consumableSelectedItemNum] != null)
        {
            return consumableItems[consumableSelectedItemNum];
        }
        return null;    
    }
    public Item GetItemOnArmourSlot()
    {
        if (armourSet != null)
        {
            return armourSet;
        }
        return null;    
    }


    public void SetCarriedItem(InventoryItem item)
    {
        if(carriedItem != null)
        {
            if (item.activeSlot.myType != ItemType.None && item.activeSlot.myType != carriedItem.myItem.itemTypePublic) return;
            item.activeSlot.SetItem(carriedItem);
        }   

        if(item.activeSlot.myType != ItemType.None)
        {
            EquipEquipment(item.activeSlot.myType, null);
        }

        carriedItem = item;
        carriedItem.canvasGroup.blocksRaycasts = false;
        item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(ItemType type, InventoryItem item = null)
    {
        switch(type)
        {
            case ItemType.Weapon:
                if(item != null)
                {
                    Debug.Log("Equipped weapon: " + item.myItem.name + " on " + tag);
                }
                else
                {
                    Debug.Log("Unequipped weapon on " + tag);
                }
                    break;
            case ItemType.Equipment:
                break;
            case ItemType.Consumable:
                break;
            default:
                break;
        }//Debug.Log("Equipped " + (item != null ? item.myItem.Name : "Nothing") + " to " + type.ToString());
    }
}
