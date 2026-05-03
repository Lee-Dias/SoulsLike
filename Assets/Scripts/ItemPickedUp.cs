using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickedUp : MonoBehaviour
{
    // Singleton simples para facilitar o acesso do Inventory
    public static ItemPickedUp Instance;

    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject go;

    private Coroutine hideCoroutine;

    void Awake()
    {
        Instance = this;
        if (go != null) go.SetActive(false); // Começa escondido
    }

    public void ShowItem(Item item)
    {
        if (item == null) return;

        itemNameText.text = item.ItemName;
        itemIcon.sprite = item.ItemIcon;
        
        go.SetActive(true);

        // Se já houver um timer rodando, para ele e começa um novo
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideRoutine(5f));
    }

    IEnumerator HideRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        go.SetActive(false);
        hideCoroutine = null;
    }
}