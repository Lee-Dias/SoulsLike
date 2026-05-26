using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DestroyFloor : MonoBehaviour
{
    [SerializeField] private GameObject destructableParent;
    [SerializeField] private NavMeshData navMeshSurface;
    
    public void DestroyFloorr()
    {
        foreach (Transform child in destructableParent.transform)
        {
            if (child.gameObject.GetComponent<Destructible>())
            {
                child.gameObject.GetComponent<Destructible>().DestroyObject();
            }
        }
    }

    public void RebuildNavMesh()
    {
        // Inicia a contagem decrescente em segundo plano
        StartCoroutine(RebuildNavMeshRoutine());
    }

    // Esta é a rotina real que faz a espera dos 5 segundos
    private IEnumerator RebuildNavMeshRoutine()
    {
        // Espera 5 segundos reais no jogo
        yield return new WaitForSeconds(5f);

        // Limpa e reconstrói o NavMesh após a espera
        NavMesh.RemoveAllNavMeshData();
        NavMesh.AddNavMeshData(navMeshSurface); 
        
        // NOTA: Se estiveres a usar o componente "NavMeshSurface" oficial, 
        // o comando ideal costuma ser: navMeshSurface.BuildNavMesh();
    }
}
