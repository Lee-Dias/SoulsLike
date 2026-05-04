using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AreaResetter : MonoBehaviour 
{
    private struct ObjectState {
        public GameObject obj;
        public Vector3 initialPos;
        public Quaternion initialRot;
        public Health healthComp;
        public Destructible destComp;
        internal NavMeshAgent agent;
    }

    private List<ObjectState> childStates = new List<ObjectState>();

    void Awake() {
        // Pega todos os Transforms filhos, netos, etc. (o 'true' inclui objetos desativados)
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren) {
            // Ignora o próprio objeto onde o script está (o Pai de todos)
            if (child == transform) continue;

            Health h = child.GetComponent<Health>();
            Destructible d = child.GetComponent<Destructible>();

            // Só adiciona à lista se tiver pelo menos um dos dois componentes
            if (h != null || d != null) {
                childStates.Add(new ObjectState {
                    obj = child.gameObject,
                    initialPos = child.localPosition,
                    initialRot = child.localRotation,
                    healthComp = h,
                    destComp = d
                });
            }
        }
    }

    public void ResetArea() {
        foreach (var state in childStates) {
            // 1. Reativa o objeto (caso tenha sido feito SetActive(false))
            state.obj.SetActive(true); 

            // 2. Reseta posição e rotação


            // 3. Reseta a vida se tiver o componente Health


            // 4. Reseta o estado de destruído se tiver o componente Destructible
            if (state.destComp != null) {
            }




            if (state.healthComp != null) {
                state.healthComp.ResetHealth();
            }
            if (state.obj.GetComponent<BaseEnemyAI>() != null) {
                state.obj.GetComponent<BaseEnemyAI>().ResetEnemy();
            }
            state.obj.transform.localPosition = state.initialPos;
            state.obj.transform.localRotation = state.initialRot;



           
        }

    }
}