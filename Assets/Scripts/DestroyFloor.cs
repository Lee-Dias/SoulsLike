using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DestroyFloor : MonoBehaviour
{
    [SerializeField] private GameObject destructableParent;

    [SerializeField] private GameObject BlockActivate;
    public void DestroyFloorr()
    {
        BlockActivate.SetActive(true);
        foreach (Transform child in destructableParent.transform)
        {
            if (child.gameObject.GetComponent<Destructible>())
            {
                child.gameObject.GetComponent<Destructible>().DestroyObject();
            }
        }
        
    }
}
