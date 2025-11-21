using UnityEngine;

[CreateAssetMenu(fileName = "EnemyProfile", menuName = "Scriptable Objects/Enemy Profile")]
public class EnemyProfile : CharacterProfileBase
{
    public enum EnemyClass { GemiSword, GemiMace, GemiBow, Boss }

    [Header("Enemy Info")]
    [SerializeField] private EnemyClass enemyClass;
    [SerializeField] private int fenReward = 100;

    

    // --- Getters ---
    public EnemyClass Class => enemyClass;
    public int FenReward => fenReward;
}
