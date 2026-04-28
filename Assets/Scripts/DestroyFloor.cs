using System;
using UnityEngine;
using UnityEngine.AI;

public class DestroyFloor : MonoBehaviour
{
    [SerializeField] private GameObject destructableParent;
    [SerializeField] private NavMeshData navMeshSurface;
    
    public void DestroyFloorr()
    {
        foreach (Transform child in destructableParent.transform)
        {
            if (child.gameObject.GetComponent<Destructible>())
            {
                child.gameObject.GetComponent<Destructible>().DestroyObject();
            }
        }
        
    }
}
