using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class BonfireMenuCamera : MonoBehaviour
{
    
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Transform player;
    [SerializeField] private float duration = 1f;
    private Camera selfCamera;
    private Transform bonfireTarget;
    Sequence seq;
    Camera mainCameraComponent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selfCamera = GetComponent<Camera>();
        seq = DOTween.Sequence();
        mainCameraComponent = mainCamera.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
    }

    public void ChangeToBonfire(Vector3 bonfireTransform)
    {
        seq.Kill();
        seq = DOTween.Sequence(); 
        selfCamera.enabled = true;
        transform.position = mainCamera.transform.position;
        mainCameraComponent.enabled = false;
        seq.Append(transform.DOMove(bonfireTransform, duration));
    }
    public void ChangeToPlayer(Vector3 bonfireTransform)
    {
        seq.Kill();
        seq = DOTween.Sequence(); 
        seq.Append(transform.DOMove(mainCamera.transform.position, duration)).OnComplete(() => {
            print("Arrived");
            selfCamera.enabled = false;
            mainCameraComponent.enabled = true;
        });
        //StartCoroutine(WaitForMovementEnd());
    }

    /* IEnumerator WaitForMovementEnd()
    {
        yield return null;
        yield return new WaitUntil(() => !seq.IsPlaying());

        selfCamera.enabled = false;
        mainCameraComponent.enabled = true;
        print("|Camera position: " + transform.position + "\n|Main Camera position: " + mainCamera.transform.position);
    } */
}
