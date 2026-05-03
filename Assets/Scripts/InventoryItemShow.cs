using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryItemShow : MonoBehaviour
{
    [Header("References")]
    private Inventory inventory;

    [Header("Holders")]
    [SerializeField] private Transform rightHandHolder;
    [SerializeField] private Transform leftHandHolder;

    private PlayerAnimationsController playerAnimationsController;

    private GameObject currentRightPrefab;
    private GameObject currentLeftPrefab;

    private GameObject instantiatedRight;
    private GameObject instantiatedLeft;

    private void Awake()
    {
        playerAnimationsController = GetComponent<PlayerAnimationsController>();
        inventory = FindFirstObjectByType<Inventory>();
    }
    public void HandleRightHand()
    {
        Item item = inventory.GetItemOnRightHand();

        if (item != null && item.Weapon != currentRightPrefab)
        {
            ClearHolder(rightHandHolder);

            instantiatedRight = Instantiate(item.Weapon, rightHandHolder);
            currentRightPrefab = item.Weapon;

            var attack = instantiatedRight.GetComponent<Attack>();
            if (attack != null)
                attack.SetCharacther(gameObject);
            
            playerAnimationsController.ChangeWeaponCollider(instantiatedRight.GetComponent<BoxCollider>());
            
        }
        else if (item == null && currentRightPrefab != null)
        {
            ClearHolder(rightHandHolder);
            currentRightPrefab = null;
        }
    }

    public void HandleLeftHand()
    {
        Item item = inventory.GetItemOnLeftHand();

        if (item != null && item.Weapon != currentLeftPrefab)
        {
            ClearHolder(leftHandHolder);

            instantiatedLeft = Instantiate(item.Weapon, leftHandHolder);
            currentLeftPrefab = item.Weapon;

            var attack = instantiatedLeft.GetComponent<Attack>();
            if (attack != null)
                attack.SetCharacther(gameObject);
            playerAnimationsController.ChangeShieldCollider(instantiatedLeft.GetComponent<BoxCollider>());
        }
        else if (item == null && currentLeftPrefab != null)
        {
            ClearHolder(leftHandHolder);
            currentLeftPrefab = null;
        }
    }

    private void ClearHolder(Transform holder)
    {
        foreach (Transform child in holder)
            Destroy(child.gameObject);
    }

    public void OnConsumable(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if(inventory.GetItemOnConsumablesSlot() != null) UseConsumable();
        
    }

    public void UseConsumable()
    {
        playerAnimationsController?.OnConsumable();
        Item item = inventory.GetItemOnConsumablesSlot();
        StartCoroutine(ExecuteAfterDelay(item));
    }

    private IEnumerator ExecuteAfterDelay(Item item)
    {
        yield return new WaitForSeconds(item.DelayToUse);
        DoAfterAnimation(item);
    }
    
    private void DoAfterAnimation(Item item)
    {
        if (item.IsHeal)
        {
            GetComponent<Health>().Heal(item.HealAmount);
        }
    }
}