using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Item;
using static Unity.VisualScripting.Metadata;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject dadSlot;
    private List<Item> allPocketSlots = new List<Item>();
    private Item currentItem { get; set; }

    Image itemIcon;


    private int inv = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
		int timesran = 0;
        
        foreach (Transform child in dadSlot.transform)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            allPocketSlots.Add(slot.myItem.myItem);
            timesran++;
        }
        Debug.Log("ran " + timesran); 
        */
        itemIcon = GetComponent<Image>();

        allPocketSlots.Add(null);
        allPocketSlots.Add(null);
        allPocketSlots.Add(null);
    }

    // Update is called once per frame
    void Update()
    {
        int helpingNumber = 0;
        foreach (Transform child in dadSlot.transform)
        {
            if(child.childCount > 0)
            {
                Transform grandChild = child.GetChild(0);
                InventoryItem slot = grandChild.GetComponent<InventoryItem>();
                allPocketSlots[helpingNumber] = slot.myItem;
            }
            helpingNumber++;
        }

        if (allPocketSlots[inv] != currentItem && allPocketSlots[inv] != null)
        {
            currentItem = allPocketSlots[inv];
            itemIcon.sprite = allPocketSlots[inv].Icon;
            print(allPocketSlots[inv].name);
        }
    }
}
