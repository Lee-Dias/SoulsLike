using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] skinnedMesh; //SkinnedMeshRenderer for characters or animated objects | MeshRenderer for not animated objects
    [SerializeField] private VisualEffect[] particlesVFX;
    [SerializeField] private float dissolveRate = 0.0125f;
    [SerializeField] private float refreshRate = 0.025f;

    private Material[] skinnedMaterials;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < skinnedMesh.Length; i++)
        {
            if (skinnedMesh[i] != null)
            {
                skinnedMaterials = skinnedMesh[i].materials;
            }
            particlesVFX[i]?.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(DissolveControl());
        }
    }

    IEnumerator DissolveControl()
    {
        for (int i = 0;i < particlesVFX.Length;i++)
            if(particlesVFX[i] != null)
                {
                    particlesVFX[i].Play();
                }

        if(skinnedMaterials.Length > 0)
        {
            float counter = 0f;
            
            while (counter < 1f)
            {
                //Decrease DissolveAmount
                counter += dissolveRate;
                for(int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }

            Destroy(gameObject);
        }
    }
}
