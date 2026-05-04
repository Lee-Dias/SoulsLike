using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Splines; 
using System.Collections.Generic;
using System.Collections;

public class BonfireMenuCamera : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachineSplineDolly splineDolly;
    [SerializeField] private CinemachineRotationComposer rotationComposer;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform mainCamera;
    private Transform bonfireTarget;
    private Spline spline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spline = splineContainer.Splines[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeToBonfire(Vector3 bonfireTransform)
    {

        List<BezierKnot> knots = new List<BezierKnot>
        {
            new BezierKnot(mainCamera.position),
            new BezierKnot(bonfireTransform),
        };
        spline.Knots = knots;

        print("Added knots to spline: " + mainCamera.position + " and " + bonfireTransform);
        splineDolly.AutomaticDolly.Enabled = true;
    }
    public void ChangeToPlayer(Vector3 bonfireTransform)
    {
        List<BezierKnot> knots = new List<BezierKnot>
        {
            new BezierKnot(bonfireTransform),
            new BezierKnot(mainCamera.position),
        };
        spline.Knots = knots;

        print("Added knots to spline: " + mainCamera.position + " and " + bonfireTransform);
        
        splineDolly.AutomaticDolly.Enabled = true;
      StartCoroutine(WaitForSplineEnd());
    }

    IEnumerator WaitForSplineEnd()
    {
        yield return new WaitUntil(() => splineDolly.CameraPosition >= spline.Count - 1);

        Debug.Log("Reached the last knot!");
        splineDolly.AutomaticDolly.Enabled = false;
        gameObject.SetActive(false);
    }
}
