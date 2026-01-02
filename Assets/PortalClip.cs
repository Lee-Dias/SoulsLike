using UnityEngine;
using System.Collections.Generic;

public class PortalClip : MonoBehaviour
{
    public Transform portalTransform; // assign the portal

    private List<Material> materials = new List<Material>();

    void Start()
    {
        // Get all renderers in this object and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            // Create new material instances to avoid modifying read-only shared materials
            Material[] mats = r.materials; // this automatically instantiates
            r.materials = mats; // assign back the instantiated materials

            foreach (Material mat in mats)
            {
                materials.Add(mat);
            }
        }
    }

    void Update()
    {
        if (portalTransform == null) return;

        Vector3 normal = portalTransform.forward; 
        Vector3 point = portalTransform.position;

        Vector4 plane = new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(normal, point));

        foreach (Material mat in materials)
        {
            mat.SetVector("_ClipPlane", plane);
        }
    }
}
