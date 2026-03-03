using UnityEngine;

public class PlayerState : MonoBehaviour
{

    private bool playerCanMove = true;

    private bool isOnInventory;
    private bool isOnBonfire;

    public bool PlayerCanMove => playerCanMove;
    public bool IsOnInventory => isOnInventory;
    public bool IsOnBonfire => isOnBonfire;
    [SerializeField] private GameObject interactionMessage;
    [HideInInspector]public bool playerIsInBonfire;
    public void PlayerCanMoveState(bool state)
    {
        playerCanMove = state;
    }
    public void ChangeIsInInventoryState(bool state)
    {
        isOnInventory = state;  
    }
    public void ChangeIsInBonfireState(bool state)
    {
        isOnBonfire = state;  
        CheckInteractionMessageState();
    }

    public void CheckInteractionMessageState()
    {
        if (playerIsInBonfire && !isOnBonfire)
        {
            ChangeInteractionMessageState(true);
        }
        else
        {
            ChangeInteractionMessageState(false);
        }
    }
    public void ChangeInteractionMessageState(bool state)
    {
        interactionMessage.SetActive(state);
    }
}
