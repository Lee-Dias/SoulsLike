using UnityEngine;
using UnityEngine.UI;
using static Item;

public class Inventory : MonoBehaviour
{
    public static Inventory Singleton;
    public static InventoryItem carriedItem;

    [SerializeField] private InventorySlot[] inventorySlots;

    [SerializeField] private Transform draggablesTransform;
    [SerializeField] private InventoryItem itemPrefab;

    [Header("Item List")]
    [SerializeField] private Item[] items;

    [Header("Debug")]
    [SerializeField] private Button giveItemButton;
    
    void Awake()
    {
        Singleton = this;
        giveItemButton.onClick.AddListener( delegate { SpawnInventoryItem(); });
    }

    public void SpawnInventoryItem(Item item = null)
    {
        Item _item = item;
        if(_item == null)
        {
            int random = Random.Range(0, items.Length);
            _item = items[random];
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].myItem == null)
            {
                Instantiate(itemPrefab, inventorySlots[i].transform).Initialize(_item, inventorySlots[i]);
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(carriedItem != null)
        {
            carriedItem.transform.position = Input.mousePosition;
        }
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
