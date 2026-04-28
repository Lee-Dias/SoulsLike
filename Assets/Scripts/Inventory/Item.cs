using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Item : ScriptableObject
{
    public enum ItemType { Weapon, Shield ,Equipment, Consumable, Aura,None}
    private enum Buff { Health, Damage, Armor}
    [System.Serializable]
    public struct StatToBuff
    {
        [SerializeField]private PlayerStats.Stats stats;
        [SerializeField]private int amountToBuff;
    }

    [SerializeField] private ItemType itemType;
    [SerializeField] private new string name;
    [SerializeField] private Sprite icon;
    [SerializeField] private string description;
    public ItemType itemTypePublic => itemType;

    public Sprite Icon => icon;

    // Weapon ------------------------------------------------------------------------------
    [SerializeField, ShowIf(nameof(ShowAnimations))] private float damage;
    [SerializeField, ShowIf(nameof(ShowAnimations))] private WeaponAnimationsData animationsData;
    [SerializeField, ShowIf(nameof(ShowAnimations))] private GameObject weapon;




    // Equipment ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Equipment)] private int armorQuantity;


    //Consumable ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Consumable)] private int duration;
    [SerializeField, ShowIf("itemType", ItemType.Consumable)] private bool isHeal;
    [SerializeField, ShowIf("isHeal")] private int healAmount;
    [SerializeField, ShowIf("itemType", ItemType.Consumable)] private bool isDamageBuff;
    [SerializeField, ShowIf("isDamageBuff")] private int damageBuffAmount;
    [SerializeField, ShowIf("itemType", ItemType.Consumable)] private CombatAnimations animation;

    //Aura ------------------------------------------------------------------------------
    [SerializeField, ShowIf("itemType", ItemType.Aura)]
    private AuraData auraData;

    


    // Buff ------------------------------------------------------------------------------
    [SerializeField] private bool buff;
    [SerializeField, ShowIf("buff")] private StatToBuff[] statToBuff;
    [SerializeField, ShowIf("buff")] private int buffQuantity;

    [SerializeField] private bool destroyOnUse;
    [SerializeField] private float delayToUse;

    private bool isConsumable => itemType == ItemType.Consumable;

    private bool ShowAnimations()
    {
        return itemType == ItemType.Weapon || itemType == ItemType.Shield;
    }



    public string ItemName => name;
    public AuraData AuraData => auraData;
    public int ArmorQuantity => armorQuantity;
    public float Damage => damage;
    public WeaponAnimationsData AnimationsData => animationsData;
    public CombatAnimations Animation => animation;
    public Sprite ItemIcon => icon;
    public GameObject Weapon => weapon;
    public StatToBuff[] StatsToBuff => statToBuff;
    public bool IsHeal => isHeal;
    public bool IsDamageBuff => isDamageBuff;
    public int HealAmount => healAmount;
    public int DamageBuffAmount => damageBuffAmount;
    public bool DestroyOnUse => destroyOnUse;
    public float DelayToUse => delayToUse;

}
