using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject parentObject; // Assign in Inspector
    private Dictionary<GameObject, int> childrenDict = new Dictionary<GameObject, int>();

    void Start()
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent object not assigned!");
            return;
        }
        byte i = 0;
        // Loop through all direct children
        foreach (Transform child in parentObject.transform)
        {
            // Get all Buttons under the parent
            Button[] childButtons = child.GetComponentsInChildren<Button>();
            
            foreach (Button btn in childButtons)
            {
                childrenDict[btn.gameObject] = i;
                // Add a listener for each button
                btn.onClick.AddListener(() => OnChildClicked(btn));
                i++;
            }
        }

        
    }

    private void OnChildClicked(Button clickedButton)
    {
        Debug.Log($"Button clicked: {clickedButton.name} (Number: {childrenDict[clickedButton.gameObject]})");
    }

    // This function can be called from a Button's OnClick event
    public void OnButtonClicked(GameObject clickedButton)
    {

        // Get the button's parent Transform
        Transform parent = clickedButton.transform.parent;

        if (parent != null)
        {
            Debug.Log("Parent name: " + parent.name);
        }
        else
        {
            Debug.Log("This button has no parent!");
        }

        string parentName = parent.name;

        // Remove all non-digit characters
        string numbersOnly = Regex.Replace(parentName, @"\D", "");

        if (int.TryParse(numbersOnly, out int number))
        {
            Debug.Log($"Parent name: {parentName} → Number: {number}");
        }
        else
        {
            Debug.Log($"Parent name: {parentName} → No numbers found or not valid");
        }
    }
}
