using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class BonfireMenuCamera : MonoBehaviour
{
    
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Transform player;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float distanceXZ = 3f;
    [SerializeField] private float distanceY = 1.5f;
    private Camera selfCamera;
    private Transform bonfireTarget;
    private Sequence seq;
    private Camera mainCameraComponent;

    private bool isMovingToPlayer = false;
    private Transform tempCamera;
    
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

        if(isMovingToPlayer)
        {

            if (tempCamera != mainCamera.transform)
            {
                float timeLeft = seq.Duration() - seq.Elapsed();
                seq.Kill();
                seq = DOTween.Sequence(); 
                seq.Append(transform.DOMove(mainCamera.transform.position, duration))
                .Join(transform.DORotateQuaternion(mainCamera.transform.rotation, duration))
                .OnComplete(() => {
                    print("Arrived");
                    selfCamera.enabled = false;
                    mainCameraComponent.enabled = true;
                });
                tempCamera = mainCamera.transform;
            }
        }
        else
        {
            transform.LookAt(player);
        }
    }

    public void ChangeToBonfire(Vector3 bonfireTransform, Vector3 playerTransform)
    {
        seq.Kill();
        seq = DOTween.Sequence(); 
        selfCamera.enabled = true;
        transform.position = mainCamera.transform.position;
        mainCameraComponent.enabled = false;

        // Get the mirrored direction
        Vector3 direction = (bonfireTransform - playerTransform).normalized;


        Vector3 targetPosition = new Vector3(
            bonfireTransform.x + direction.x * distanceXZ,
            bonfireTransform.y + distanceY,
            bonfireTransform.z + direction.z * distanceXZ);
        seq.Append(transform.DOMove(targetPosition, duration));
    }
    public void ChangeToPlayer()
    {
        isMovingToPlayer = true;

        tempCamera = mainCamera.transform;
        seq.Kill();
        seq = DOTween.Sequence(); 
        seq.Append(transform.DOMove(mainCamera.transform.position, duration))
        .Join(transform.DORotateQuaternion(mainCamera.transform.rotation, duration))
        .OnComplete(() => {
            print("Arrived");
            selfCamera.enabled = false;
            mainCameraComponent.enabled = true;
        });
        //StartCoroutine(WaitForMovementEnd());

        isMovingToPlayer = false;
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
