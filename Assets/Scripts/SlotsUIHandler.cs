using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotsUIHandler : MonoBehaviour
{
    [SerializeField]private Image weaponImage;
    [SerializeField]private Image shieldImage;
    [SerializeField]private Image consumableImage;
    [SerializeField]private Inventory inventory;
    [SerializeField]private TextMeshProUGUI auraText;

    private PlayerStats playerStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    void Update()
    {
        UpdateSlot(weaponImage, inventory.GetItemOnRightHand());
        UpdateSlot(shieldImage, inventory.GetItemOnLeftHand());
        UpdateSlot(consumableImage, inventory.GetItemOnConsumablesSlot());

        auraText.text = playerStats.CrystalShards.ToString();
    }

    private void UpdateSlot(Image image, Item item)
    {
        if (item != null)
        {
            image.sprite = item.ItemIcon;
            image.color = new Color(1f, 1f, 1f, 1f); // fully visible
        }
        else
        {
            image.sprite = null;
            image.color = new Color(1f, 1f, 1f, 0f); // alpha = 0 (invisible)
        }
    }
}
