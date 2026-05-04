using Unity.VisualScripting;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private AuraInventory auraInventory;

    public static EquipmentManager Singleton;   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Singleton = this;
    }

    // Update is called once per frame
    public void UpdateUpdateEquippedAura()
    {
        EquipAura(auraInventory.GetAuraEquipped());
    }

    private void EquipAura(Item item)
    {
        if (item == null ||item.AuraData == null)
        {
            UnequipAura();
            return;
        } 

        InventoryItemShow.Singleton.ShowHideAura(true);

        var modifiers = this.GetComponent<CombatModifiers>();

        modifiers.timeScaleMultiplier = item.AuraData.ParryTimeScaleMultiplier;
        modifiers.durationMultiplier = item.AuraData.ParryDurationMultiplier;
        modifiers.radiusMultiplier = item.AuraData.ParryRadiusMultiplier;
    }
    void UnequipAura()
    {
        InventoryItemShow.Singleton.ShowHideAura(false);

        var modifiers = this.GetComponent<CombatModifiers>();

        modifiers.timeScaleMultiplier = 1f;
        modifiers.durationMultiplier = 1f;
        modifiers.radiusMultiplier = 1f;
    }
    
}
