using System.Collections;
using UnityEngine;

public class ExpandParryShockWave : MonoBehaviour
{
    [SerializeField] private MeshRenderer material;
    [SerializeField] private float expandDuration = 1f;
    [SerializeField] private float collapseDuration = 0.5f;
    [SerializeField] private float maxInnerRadius = 0.55f;

    public void Play()
    {
        StartCoroutine(AnimateInnerRadius());
    }

    private IEnumerator AnimateInnerRadius()
    {
        // 0 → maxInnerRadius
        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(0f, maxInnerRadius, Mathf.Clamp01(elapsed / expandDuration));
            material.material.SetFloat("_Inner_Radius", value);
            yield return null;
        }
        material.material.SetFloat("_Inner_Radius", maxInnerRadius);

        // maxInnerRadius → 0
        elapsed = 0f;
        while (elapsed < collapseDuration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(maxInnerRadius, 0f, Mathf.Clamp01(elapsed / collapseDuration));
            material.material.SetFloat("_Inner_Radius", value);
            yield return null;
        }
        material.material.SetFloat("_Inner_Radius", 0f);
        this.gameObject.SetActive(false);
    }
}