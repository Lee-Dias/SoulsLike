using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Item : ScriptableObject
{
    public enum ItemType { Weapon, Equipment, Consumable, None}
    private enum BodyPlace { Head, Chest, Pants, Ring}
    private enum Buff { Health, Damage, Armor}

    [SerializeField] private ItemType itemType;
    [SerializeField] private new string name;
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;
    [SerializeField] private int value;
    //[SerializeField] private SlotTag itemTag;

    public ItemType itemTypePublic => itemType;

    public Sprite Icon => icon;

    // Weapon ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Weapon)] private int damage;



    // Equipment ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Equipment)] private int armorQuantity;
    [SerializeField, ShowIf("itemType", ItemType.Equipment)] private BodyPlace bodyPlace;


    //Consumable ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Consumable)] private int duration;


    // Buff ------------------------------------------------------------------------------
    [SerializeField] private bool buff;

    [SerializeField, ShowIf("buff")] private Buff buffType;
    [SerializeField, ShowIf("buff")] private int buffQuantity;

    private bool isConsumable => itemType == ItemType.Consumable;
    [SerializeField, ShowIf(EConditionOperator.And, "buff", "isConsumable")] private int buffCooldown;





    public int ArmorQuantity => armorQuantity;

}
