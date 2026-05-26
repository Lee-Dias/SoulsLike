using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Item;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;
    
    
    [SerializeField] private GameObject inventorySlotsParent;
    [SerializeField] private GameObject inventorySlot;

    [Header("Aura Inventory Reference")]
    [SerializeField] private AuraInventory auraInventory;
    

    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] private Item[] items;

    [Header("In Hand")]
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject consumablesSlot;
    [SerializeField] private GameObject armourSlot;

    [SerializeField] private Item giveItemAtStart;
    [SerializeField] private Item giveLeftItemAtStart;

    private Animator animator;

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
        animator = GetComponent<Animator>();

    }
    void Start()
    {
        EquipItemAtStart();
        EquipLeftItemAtStart();
        UpdateSlots();
        Item itemOnHand = GetItemOnRightHand();
        if(itemOnHand.ItemName == "Straight Sword")
        {
            animator.SetBool("StraightSword", true);
        }
        else
        {
            animator.SetBool("StraightSword", false);
            animator.SetBool("GreatSword", true);
        }
        UpdateEquippedItems();
        
    }

    public void ChangeRightHandEquipped(InputAction.CallbackContext ctx)
    {
        return;

        rightSelectedItemNum =
            (rightSelectedItemNum + 1) % rightHandItems.Length;

        Debug.Log("Right hand slot selected: " + rightSelectedItemNum);

        InventorySlot slot =
            rightHand.transform
                .GetChild(rightSelectedItemNum)
                .GetComponent<InventorySlot>();

        EquipEquipment(ItemType.Weapon, slot?.myItem);
    }


    public void ChangeLeftHandEquipped(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        leftSelectedItemNum =
            (leftSelectedItemNum + 1) % leftHandItems.Length;

        Debug.Log("Left hand slot selected: " + leftSelectedItemNum);

        InventorySlot slot =
            leftHand.transform
                .GetChild(leftSelectedItemNum)
                .GetComponent<InventorySlot>();

        EquipEquipment(ItemType.Weapon, slot?.myItem);
    }


    public void ChangeConsumableEquipped(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        consumableSelectedItemNum =
            (consumableSelectedItemNum + 1) % consumableItems.Length;

        Debug.Log("Consumable slot selected: " + consumableSelectedItemNum);

        InventorySlot slot =
            consumablesSlot.transform
                .GetChild(consumableSelectedItemNum)
                .GetComponent<InventorySlot>();

        EquipEquipment(ItemType.Consumable, slot?.myItem);
    }


    private int GetNextValidIndex(Item[] items, int currentIndex)
    {
        int length = items.Length;
        int nextIndex = currentIndex;

        for (int i = 0; i < length; i++)
        {
            nextIndex = (nextIndex + 1) % length;

            if (items[nextIndex] != null)
                return nextIndex;
        }

        // No valid item found
        return currentIndex;
    }



    private void EquipItemAtStart()
    {
        if (giveItemAtStart == null)
            return;

        // Find first right-hand slot
        InventorySlot targetSlot = null;

        foreach (Transform child in rightHand.transform)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null)
            {
                targetSlot = slot;
                break;
            }
        }

        if (targetSlot == null)
        {
            Debug.LogError("No right-hand InventorySlot found!");
            return;
        }

        // Create inventory item instance
        InventoryItem newItem = Instantiate(itemPrefab, targetSlot.transform);
        // Temporary 
        newItem.canRemove = false;
        newItem.Initialize(giveItemAtStart, targetSlot);


        // Force equip logic
        targetSlot.SetItem(newItem);

        rightSelectedItemNum = 0;

        // Optional: notify equip system
        EquipEquipment(ItemType.Weapon, newItem);
    }

    private void EquipLeftItemAtStart()
    {
        if (giveLeftItemAtStart == null)
            return;

        // Find first left-hand slot
        InventorySlot targetSlot = null;

        foreach (Transform child in leftHand.transform)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null)
            {
                targetSlot = slot;
                break;
            }
        }

        if (targetSlot == null)
        {
            Debug.LogError("No left-hand InventorySlot found!");
            return;
        }

        // Create inventory item instance
        InventoryItem newItem = Instantiate(itemPrefab, targetSlot.transform);

        newItem.Initialize(giveLeftItemAtStart, targetSlot);

        // Force equip
        targetSlot.SetItem(newItem);

        leftSelectedItemNum = 0;

        EquipEquipment(ItemType.Weapon, newItem);
        
    }
    private void UpdateEquippedItems()
    {
        UpdateSlots();
        InventoryItemShow inventoryItemShow = GetComponent<InventoryItemShow>();
        inventoryItemShow.HandleRightHand();
        inventoryItemShow.HandleLeftHand();
        inventoryItemShow.HandleArmorSlot();
        PlayerAnimationsController playerAnimationsController= GetComponent<PlayerAnimationsController>();
        playerAnimationsController.ChangeEquippedWeapon(GetItemOnRightHand());
        playerAnimationsController.ChangeEquippedShield(GetItemOnLeftHand());
    }


    private InventorySlot CreateInventorySlot()
    {
        GameObject slotGO = Instantiate(inventorySlot, inventorySlotsParent.transform);
        InventorySlot slot = slotGO.GetComponent<InventorySlot>();

        inventorySlots.Add(slot);
        return slot;
    }
    public void SpawnInventoryItem(Item item)
    {
        if (item == null) return;
        
        
        if (item.itemTypePublic == ItemType.Aura)
        {
            auraInventory.SpawnAuraItem(item);
            return;
        }

        // Normal inventory flow
        InventorySlot newSlot = CreateInventorySlot();

        Instantiate(itemPrefab, newSlot.transform)
            .Initialize(item, newSlot);
        UpdateEquippedItems();
    }


    // Update is called once per frame
    void Update()
    {
        if(carriedItem != null)
        {
            //carriedItem.transform.position = Input.mousePosition;
        }
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
                r+=1;
            }    
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
                r+=1;
            }            
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
                r+=1;
            }            
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
        //item.transform.SetParent(draggablesTransform);
    }

    public void EquipEquipment(ItemType type, InventoryItem item = null)
    {
        UpdateEquippedItems();
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
        }
        
    }
}
