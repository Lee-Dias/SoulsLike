using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuUIButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseOver;
    private TMP_Text tmpText;
    private Color prevText;


    void Start()
    {
        // For UI text (most common in canvases)
        tmpText = GetComponentInChildren<TextMeshProUGUI>();

        // Or for 3D world-space text
        // tmpText = GetComponentInChildren<TextMeshPro>();

        // Or using the base class (works for either)
        // tmpText = GetComponentInChildren<TMP_Text>();

        if (tmpText == null)
        {
            Debug.LogError("No TextMeshPro component found in children!");
        }
        prevText = tmpText.color;
    }

    private void Update()
    {
        if(isMouseOver)
        {
            tmpText.color = Color.white; // Change color when hovered
        }
        else
        {
            tmpText.color = prevText; // Default color
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        Debug.Log($"{name} hovered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        Debug.Log($"{name} unhovered");
    }
}