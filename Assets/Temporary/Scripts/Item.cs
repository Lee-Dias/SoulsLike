using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Item : ScriptableObject
{
    public enum ItemType { Weapon, Shield ,Equipment, Consumable, Aura,None}
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
    [SerializeField, ShowIf("itemType", ItemType.Weapon)] private float damage;
    [SerializeField, ShowIf("itemType", ItemType.Weapon)] private WeaponAnimationsData animationsData;
    [SerializeField, ShowIf("itemType", ItemType.Weapon)] private GameObject weapon;



    // Equipment ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Equipment)] private int armorQuantity;


    //Consumable ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Consumable)] private int duration;
    [SerializeField, ShowIf("itemType", ItemType.Consumable)] private CombatAnimations animation;

    //Aura ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Aura)]
    private AuraData auraData;

    


    // Buff ------------------------------------------------------------------------------
    [SerializeField] private bool buff;

    [SerializeField, ShowIf("buff")] private Buff buffType;
    [SerializeField, ShowIf("buff")] private int buffQuantity;

    private bool isConsumable => itemType == ItemType.Consumable;
    [SerializeField, ShowIf(EConditionOperator.And, "buff", "isConsumable")] private int buffCooldown;




    public AuraData AuraData => auraData;
    public int ArmorQuantity => armorQuantity;
    public float Damage => damage;
    public WeaponAnimationsData AnimationsData => animationsData;
    public CombatAnimations Animation => animation;
    public Sprite ItemIcon => icon;
    public GameObject Weapon => weapon;

}
