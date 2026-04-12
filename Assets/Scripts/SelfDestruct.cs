using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float selfDestructIn = 5f;
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private float fadeDuration = 1f;

    private float timer;
    private bool isDestructing = false;
    private Renderer[] allRenderers;

    void Start()
    {
        allRenderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (isDestructing) return;

        timer += Time.deltaTime;

        if (timer >= selfDestructIn)
        {
            if (fadeOut && allRenderers.Length > 0)
            {
                StartCoroutine(FadeAndDestroy());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        isDestructing = true;
        
        // Configura os materiais para permitirem transparência
        foreach (Renderer rend in allRenderers)
        {
            foreach (Material mat in rend.materials)
            {
                SetupMaterialToFade(mat);
            }
        }

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            foreach (Renderer rend in allRenderers)
            {
                if (rend == null) continue;
                foreach (Material mat in rend.materials)
                {
                    Color c = mat.color;
                    mat.color = new Color(c.r, c.g, c.b, alpha);
                }
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    // Essa função "destrava" o material para aceitar transparência em tempo de execução
    private void SetupMaterialToFade(Material mat)
    {
        // Se estiver usando Standard Shader (Built-in)
        mat.SetFloat("_Mode", 2); // 2 é o modo Fade
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        
        // Se estiver usando URP, a propriedade geralmente é essa:
        if(mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1); 
    }
}