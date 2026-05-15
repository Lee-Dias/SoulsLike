using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class BonfireMenuCamera : MonoBehaviour
{
    
    [SerializeField] private Transform mainCamera;
    private Camera selfCamera;
    private Transform bonfireTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selfCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeToBonfire(Vector3 bonfireTransform)
    {
        selfCamera.enabled = true;
        transform.position = mainCamera.position;
        mainCamera.gameObject.SetActive(false);
    }
    public void ChangeToPlayer(Vector3 bonfireTransform)
    {
      //StartCoroutine(WaitForSplineEnd());
    }

    /* IEnumerator WaitForSplineEnd()
    {
        
        gameObject.SetActive(false);
    } */
}
