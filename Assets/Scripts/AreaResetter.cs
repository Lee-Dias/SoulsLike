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
        public NavMeshAgent agent;
        public BaseEnemyAI enemyAI;
    }

    private List<ObjectState> childStates = new List<ObjectState>();

    void Awake() {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren) {
            if (child == transform) continue;

            Health h = child.GetComponent<Health>();
            Destructible d = child.GetComponent<Destructible>();

            if (h != null || d != null) {
                childStates.Add(new ObjectState {
                    obj = child.gameObject,
                    initialPos = child.localPosition,
                    initialRot = child.localRotation,
                    healthComp = h,
                    destComp = d,
                    agent = child.GetComponent<NavMeshAgent>(),      // ← was never assigned
                    enemyAI = child.GetComponent<BaseEnemyAI>()      // ← cache it too
                });
            }
        }
    }

    public void ResetArea() {
        foreach (var state in childStates) {
            // 1. Reativa o objeto
            state.obj.SetActive(true);

            // 2. Desativa o NavMeshAgent antes de mover, senão ele resiste ao teleport
            if (state.agent != null) {
                state.agent.enabled = false;
            }

            // 3. Reseta posição e rotação
            state.obj.transform.localPosition = state.initialPos;
            state.obj.transform.localRotation = state.initialRot;

            // 4. Reativa o NavMeshAgent depois de mover
            if (state.agent != null) {
                state.agent.enabled = true;
                state.agent.ResetPath();
            }

            // 5. Reseta health
            if (state.healthComp != null) {
                state.healthComp.ResetHealth();
            }

            // 6. Reseta destrutível
            if (state.destComp != null) {
                
            }

            // 7. Reseta o AI por último (já tem posição e vida corretas)
            if (state.enemyAI != null) {
                state.enemyAI.ResetEnemy();
            }
        }
    }
}