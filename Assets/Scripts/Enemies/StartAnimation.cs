using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StartAnimation : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private DratorsaAI dratorsaAI;
    
    [Header("Configurações de Levitação")]
    [SerializeField] private float targetHeight = 2.0f; // Altura final desejada
    [SerializeField] private float duration = 3.0f;    // Tempo da transição
    [SerializeField] private float startDelay = 2.0f;   // Tempo de espera antes de subir
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        dratorsaAI = GetComponent<DratorsaAI>();
    }

    // Update is called once per frame
    public void StartBoss()
    {
        animator.SetTrigger("Start");
        StartCoroutine(LevitateRoutine());
    }

    private IEnumerator LevitateRoutine()
    {
        yield return new WaitForSeconds(startDelay);
        float timeElapsed = 0;
        float startHeight = agent.baseOffset;

        while (timeElapsed < duration)
        {
            // Interpola o valor do Base Offset de 0 até a altura desejada
            agent.baseOffset = Mathf.Lerp(startHeight, targetHeight, timeElapsed / duration);
            
            timeElapsed += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        // Garante que termina exatamente no valor alvo
        agent.baseOffset = targetHeight;
        dratorsaAI.enabled = true; // Ativa o comportamento do inimigo após a levitação


        
    }
}
