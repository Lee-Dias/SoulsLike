using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InfoMouseFollowerUI : MonoBehaviour
{
    private TextMeshProUGUI text;
    private RectTransform selfRect;
    private RectTransform childRect;
    private Canvas canvas;

    private Image selfImage;
    private TextMeshProUGUI mouseText;
    public bool isHoveringItem = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        selfRect = GetComponent<RectTransform>();
        childRect = transform.GetChild(0).GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        selfImage = GetComponent<Image>();
        mouseText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHoveringItem)
        {
            selfImage.enabled = false;
            mouseText.text = "";
            return;
        }
        else
        {
            selfImage.enabled = true;
        }

        /* Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.GetComponent<RectTransform>(),
        Mouse.current.position.ReadValue(),
        canvas.worldCamera,
        out mousePos
        );

        selfRect.anchoredPosition = mousePos; */


        Vector2 mousePos = Mouse.current.position.ReadValue();
    
        // Convert screen position to canvas position
        Vector2 canvasPos = mousePos / canvas.scaleFactor;
        
        selfRect.anchoredPosition = new Vector2(canvasPos.x, canvasPos.y - (Screen.height / canvas.scaleFactor)) + new Vector2(120,-30);



        text.ForceMeshUpdate();

        int lineCount = text.textInfo.lineCount;

        if (lineCount == 1)
        {
            childRect.anchoredPosition = new Vector2(childRect.anchoredPosition.x, 3);
            selfRect.sizeDelta = new Vector2(selfRect.sizeDelta.x, 30);
        }
        else
        {
            childRect.anchoredPosition = new Vector2(childRect.anchoredPosition.x, 14);
            selfRect.sizeDelta = new Vector2(selfRect.sizeDelta.x, 55);
        }
    }
}
