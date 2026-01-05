using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private AuraInventory auraInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EquipAura(auraInventory.GetAuraEquipped());
    }

    void EquipAura(Item item)
    {
        if (item == null ||item.AuraData == null)
        {
            UnequipAura();
            return;
        } 

        var modifiers = this.GetComponent<CombatModifiers>();

        modifiers.timeScaleMultiplier = item.AuraData.ParryTimeScaleMultiplier;
        modifiers.durationMultiplier = item.AuraData.ParryDurationMultiplier;
        modifiers.radiusMultiplier = item.AuraData.ParryRadiusMultiplier;
    }
    void UnequipAura()
    {

        var modifiers = this.GetComponent<CombatModifiers>();

        modifiers.timeScaleMultiplier = 1f;
        modifiers.durationMultiplier = 1f;
        modifiers.radiusMultiplier = 1f;
    }
    
}
