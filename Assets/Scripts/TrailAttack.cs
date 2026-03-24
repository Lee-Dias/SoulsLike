using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TrailAttack : MonoBehaviour
{
    [SerializeField] string nameOfChild;
    [SerializeField] AnimationCurve widthCurve;
    [SerializeField] float timeOfTrail;
    [SerializeField] float minVertexDistanceOfTrail;
    [SerializeField] int cornerVerticesOfTrail;
    [SerializeField] Material materialOfTrail;
    [SerializeField] PlayerAnimationsController playerAnimationsControllerScript;
    List<GameObject> listOfChildren= new List<GameObject>();
    TrailRenderer trailRenderer;
    GameObject childWithTrail;
    bool activateTrail;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FindChild());
    }

    IEnumerator FindChild()
    {
        while(childWithTrail == null)
        {
            yield return null;
            for(int i = 0; i < listOfChildren.Count; i++)
            {
                if (listOfChildren[i].transform.Find(nameOfChild) != null)
                {
                    childWithTrail = listOfChildren[i].transform.Find(nameOfChild).gameObject;
                    break;
                }    
            }

            List<GameObject> tempListOfChildren = new List<GameObject>();
            foreach(GameObject child in listOfChildren)
            {
                if(child.transform.childCount > 0)
                {
                    tempListOfChildren.Add(child.transform.GetChild(0).gameObject);
                }
            }
            listOfChildren = tempListOfChildren;

            if(listOfChildren.Count == 0)
            {
                yield return null;
                listOfChildren = new List<GameObject>();
                while(true)
                {
                    yield return null;
                    if(transform.childCount == 0)
                    {
                        continue;
                    }
                    else
                    {
                        listOfChildren.Add(transform.GetChild(0).gameObject);
                        break;    
                    }
                    
                }
            }
            
        }

        childWithTrail.AddComponent<TrailRenderer>();
        trailRenderer = childWithTrail.GetComponent<TrailRenderer>();
        trailRenderer.time = timeOfTrail;
        trailRenderer.materials = new Material[1] { materialOfTrail };
        trailRenderer.widthCurve = widthCurve;
        trailRenderer.minVertexDistance = minVertexDistanceOfTrail;
        trailRenderer.numCornerVertices = cornerVerticesOfTrail;
    }

    // Update is called once per frame
    void Update()
    {
        if(trailRenderer == null)
        {
            return;
        }
        activateTrail = playerAnimationsControllerScript.ActivateTrail;
        if(activateTrail)
        {
            trailRenderer.emitting = true;
        }
        else
        {
            trailRenderer.emitting = false;
        }
    }
}
