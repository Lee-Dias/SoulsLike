using UnityEngine;
using UnityEngine.UI;

public class SlotsUIHandler : MonoBehaviour
{
    [SerializeField]private Image weaponImage;
    [SerializeField]private Image ShieldImage;
    [SerializeField]private Image consumableImage;
    [SerializeField]private Inventory inventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        weaponImage.sprite = inventory.GetItemOnRightHand() != null ? inventory.GetItemOnRightHand().ItemIcon : null;
        ShieldImage.sprite = inventory.GetItemOnLeftHand() != null ? inventory.GetItemOnLeftHand().ItemIcon : null;
        consumableImage.sprite = inventory.GetItemOnConsumablesSlot() != null ? inventory.GetItemOnConsumablesSlot().ItemIcon: null;
    }
}
