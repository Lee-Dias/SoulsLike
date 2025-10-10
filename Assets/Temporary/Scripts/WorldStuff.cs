using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleVisibility : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // The object you want to show/hide
    [SerializeField] private Key key = Key.I;

    void Update()
    {
        if (Keyboard.current[key].wasPressedThisFrame)
        {
            // Toggle visibility (active state)
            bool isActive = targetObject.activeSelf;
            targetObject.SetActive(!isActive);
        }
    }
}