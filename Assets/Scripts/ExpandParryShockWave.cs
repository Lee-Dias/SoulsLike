using System.Collections;
using UnityEngine;

public class ExpandParryShockWave : MonoBehaviour
{
    [SerializeField] private MeshRenderer material;
    [SerializeField] private float expandDuration = 1f;
    [SerializeField] private float collapseDuration = 0.5f;
    [SerializeField] private float maxInnerRadius = 0.55f;
    [SerializeField] private float opacityStart = 0.5f;
    [SerializeField] private float opacityMax = 1f;

    public void Play()
    {
        StartCoroutine(AnimateShockWave());
    }

    private IEnumerator AnimateShockWave()
    {
        // 0 → maxInnerRadius | opacityStart → opacityMax
        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / expandDuration);
            material.material.SetFloat("_Inner_Radius", Mathf.Lerp(0f, maxInnerRadius, t));
            material.material.SetFloat("_Opacity", Mathf.Lerp(opacityStart, opacityMax, t));
            yield return null;
        }
        material.material.SetFloat("_Inner_Radius", maxInnerRadius);
        material.material.SetFloat("_Opacity", opacityMax);

        // maxInnerRadius → 0 | opacityMax → opacityStart
        elapsed = 0f;
        while (elapsed < collapseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / collapseDuration);
            material.material.SetFloat("_Inner_Radius", Mathf.Lerp(maxInnerRadius, 0f, t));
            material.material.SetFloat("_Opacity", Mathf.Lerp(opacityMax, opacityStart, t));
            yield return null;
        }
        material.material.SetFloat("_Inner_Radius", 0f);
        material.material.SetFloat("_Opacity", opacityStart);
        this.gameObject.SetActive(false);
    }
}