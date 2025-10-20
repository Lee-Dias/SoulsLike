using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldStuff : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // The object you want to show/hide
    [SerializeField] private Key key = Key.I;
    [SerializeField]Dictionary<int, Items> inventory = new Dictionary<int, Items>();

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