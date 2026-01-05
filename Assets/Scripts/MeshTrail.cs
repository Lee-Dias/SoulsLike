using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{

    private float meshRefreshRate = 0.1f;

    private bool isTrailActive;
    
    private SkinnedMeshRenderer[] skinnedMeshRenderers;

    [SerializeField] private Transform positionToSpawn;
    [SerializeField] private Material mat;
    [SerializeField] private string shaderVarRef;
    [SerializeField] private float shaderVarRate= 0.1f;
    [SerializeField] private float shaderVarRefreshRate = 0.01f;
    [SerializeField] private float meshDestroyDelay = 3;

    private void Start()
    {
        if (skinnedMeshRenderers == null)
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Trail(float activeTime)
    {
        if (!isTrailActive)
        {
            isTrailActive = true;   
            StartCoroutine(ActivateTrail(activeTime));            
        }
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0;  i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;
                
                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gObj, meshDestroyDelay);
            }
            
            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
