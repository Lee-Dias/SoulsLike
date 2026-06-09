using System.Collections;
using UnityEngine;

public class BarrierDissolve : MonoBehaviour
{
    [SerializeField] private MeshRenderer form;
    [SerializeField] private MeshRenderer dissolve;
    private Shield shield;

    private Coroutine transitionCoroutine;
    private Color baseColor; // Guarda a cor original sem multiplicadores de intensidade
    private float duration = 0.7f; // Tempo da transição

    void Start()
    {
        shield = GetComponent<Shield>();
        
        // Guarda a cor inicial do material para usarmos como base limpa
        if (form != null && form.material.HasProperty("_ShieldColor"))
        {
            baseColor = form.material.GetColor("_ShieldColor");
        }
        
        StartShield();
    }
    public void StartShield()
    {
        Color targetColor;
        float targetErosion;

        targetColor = baseColor * 0.01f;
        targetErosion = 1.2f;
        transitionCoroutine = StartCoroutine(TransitionBarrier(targetColor, targetErosion, 0.1f));
    }

    public void ChangeBarrierValues()
    {
        if (shield == null) return;

        form.enabled = true;
        dissolve.enabled = true;

        // Definir os valores alvo dependendo da vida do escudo
        Color targetColor = baseColor;
        float targetErosion = 0f;

        targetColor = baseColor * 0.01f;
        targetErosion = 1.2f;

        if (shield.ShieldValue == shield.MaxShieldHealth)
        {
            targetColor = baseColor * 3f;
            targetErosion = 0.25f;
        }
        else if (shield.ShieldValue > 50)
        {
            targetColor = baseColor * 1f;
            targetErosion = 0.6f;
        }
        else if (shield.ShieldValue > 10)
        {
            targetColor = baseColor * 0.2f;
            targetErosion = 0.8f;
        }
        else
        {
            targetColor = baseColor * 0.001f;
            targetErosion = 1.2f;
        }

        // Se já houver uma transição a decorrer, para-a para não haver conflitos
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        // Inicia a nova transição suave
        transitionCoroutine = StartCoroutine(TransitionBarrier(targetColor, targetErosion, duration));
    }

    private IEnumerator TransitionBarrier(Color targetColor, float targetErosion, float duration )
    {
        // Ponto de partida atual (onde o material está NESTE exato momento)
        Color startColor = form.material.GetColor("_ShieldColor");
        float startErosion = dissolve.material.GetFloat("_Erosion");
        
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Vai de 0 a 1 ao longo de 0.3s

            // Interpolação suave entre o valor antigo e o novo
            Color newColor = Color.Lerp(startColor, targetColor, t);
            float newErosion = Mathf.Lerp(startErosion, targetErosion, t);

            // Aplica os valores intermédios
            form.material.SetColor("_ShieldColor", newColor);
            dissolve.material.SetFloat("_Erosion", newErosion);

            yield return null; // Espera pelo próximo frame
        }

        // Garante que no final os valores ficam exatamente os pretendidos
        form.material.SetColor("_ShieldColor", targetColor);
        dissolve.material.SetFloat("_Erosion", targetErosion);
    }
}