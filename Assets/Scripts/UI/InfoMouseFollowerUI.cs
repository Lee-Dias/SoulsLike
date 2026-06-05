using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InfoMouseFollowerUI : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private RectTransform selfRect;
    private RectTransform childRect;
    private Canvas canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        selfRect = GetComponent<RectTransform>();
        childRect = transform.GetChild(0).GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {

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



        tmp.ForceMeshUpdate();

        int lineCount = tmp.textInfo.lineCount;

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
