using UnityEngine;
using System.Collections;
using NUnit.Framework;
using UnityEngine.InputSystem; // Necessário para usar Coroutines

public class Npc : MonoBehaviour
{
    [SerializeField] private Item itemToGive;   
    [SerializeField] private GameObject DialogueBox;   
    private Inventory inventory;
    private PlayerState playerState;
    private bool playerInside = false;
    private bool hasTalked = false; // Garante que só falas uma vez se quiseres que ele desapareça

    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        playerState = FindFirstObjectByType<PlayerState>();
        if (DialogueBox != null)
            DialogueBox.SetActive(false);
    }
    public void InteractWithNpc(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (playerInside)
        {
            Talk();
        }

    }

    private void Talk()
    {
        if (hasTalked || playerState.EnemyAround || playerState.IsInSettings || playerState.IsOnInventory || !playerInside) return; // Impede de ganhar itens infinitos enquanto ele não some
        playerState.ChangeInteractionMessageState(false);

        if (DialogueBox != null)
        {
            DialogueBox.SetActive(true);
        }

        hasTalked = true;
        
        // Inicia a contagem decrescente para desaparecer
        StartCoroutine(HandleDisappear());
    }

    private IEnumerator HandleDisappear()
    {
        // Espera por 3 segundos reais
        yield return new WaitForSeconds(5f);

        // Desativa a caixa de diálogo (opcional, já que o NPC vai sumir)
        if (DialogueBox != null)
        {
            DialogueBox.SetActive(false);
        }

        if (inventory != null && itemToGive != null)
        {
            inventory.SpawnInventoryItem(itemToGive);
            if (ItemPickedUp.Instance != null) {
                ItemPickedUp.Instance.ShowItem(itemToGive);
            }
        }

        //gameObject.SetActive(false);
        
    }

    private void OnTriggerEnter(Collider tag)
    {
        if(hasTalked) return;
        if (tag.CompareTag("Player"))
        {
            playerInside = true;
            playerState.ChangeInteractionMessageState(true);
        }
    }

    private void OnTriggerExit(Collider tag)
    {
        if (tag.CompareTag("Player"))
        {
            playerInside = false;
            playerState.ChangeInteractionMessageState(false);
        }
    }
}