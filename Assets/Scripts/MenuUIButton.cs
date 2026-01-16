using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuUIButton: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Color chosenColer = Color.white;

    bool isMouseOver = false;
    TMP_Text tmpText;
    Color prevText;


    void Start()
    {
        // For UI text (most common in canvases)
        tmpText = GetComponentInChildren<TextMeshProUGUI>();

        if (tmpText == null)
        {
            Debug.LogError("No TextMeshPro component found in children!");
        }
        prevText = tmpText.color;
        
    }

    private void Update()
    {
        if (isMouseOver)
        {
            tmpText.color = chosenColer; // Change color when hovered
        }
        else
        {
            tmpText.color = prevText; // Default color
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    void OnEnable()
    {
        isMouseOver = false;
    }
}