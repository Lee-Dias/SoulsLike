using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.InputSystem;
using TMPro; // Necessário para usar Coroutines

public class Npc : MonoBehaviour
{
    [SerializeField] private Item itemToGive;   
    [SerializeField] private GameObject dialogueBox;   
    [SerializeField] private string[] dialogueText; // Texto que o NPC vai falar (pode ser expandido para uma lista de falas, se necessário)
    private Inventory inventory;
    private PlayerState playerState;
    private bool playerInside = false;
    private bool isTalking = false; 
    private bool firstTalk = true; 
    private int currentText = 0;
    private int whereToStartText = 1;

    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        playerState = FindFirstObjectByType<PlayerState>();
        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }
    public void InteractWithNpc(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (playerInside || isTalking)
        {
            Talk();
        }

    }

    private void Talk()
    {
        if (playerState.IsBeingChased() || playerState.IsInSettings || playerState.IsOnInventory || !playerInside && !isTalking) return; // Impede de ganhar itens infinitos enquanto ele não some
        playerState.ChangeInteractionMessageState(false);
        isTalking = true;
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
            TextMeshProUGUI text = dialogueBox.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            
            if (currentText < dialogueText.Length)
            {
                text.text = dialogueText[currentText];
                currentText += 1;
            }
            else
            {
                currentText = whereToStartText;
                if (firstTalk)
                {
                    inventory.SpawnInventoryItem(itemToGive);
                    if (ItemPickedUp.Instance != null) {
                        ItemPickedUp.Instance.ShowItem(itemToGive);
                    }
                    firstTalk = false;
                }
                
                isTalking = false;
                dialogueBox.SetActive(false);
                if(playerInside)
                {
                    playerState.ChangeInteractionMessageState(true);
                }
            }
        }

    }

    private void OnTriggerEnter(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerInside = true;
            if (isTalking) return;  
            playerState.ChangeInteractionMessageState(true);
        }
    }

    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            if (firstTalk)
            {
                currentText = 0;

            }
            else
            {
                currentText = whereToStartText;
            }
            dialogueBox.SetActive(false);
            
            isTalking = false;
            playerInside = false;
            playerState.ChangeInteractionMessageState(false);
        }
    }
}