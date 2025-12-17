using UnityEngine;

[CreateAssetMenu(fileName = "WeaponAnimationsData", menuName = "Scriptable Objects/Weapon Animations Data")]
public class WeaponAnimationsData : ScriptableObject
{
    public enum WeaponType { Sword, Axe, Spear, Shield }

    [SerializeField] private WeaponType weaponType;
    [SerializeField] private bool twoHanded;
    [SerializeField] private CombatAnimations lightAttack;
    [SerializeField] private CombatAnimations heavyAttack;
    [SerializeField] private CombatAnimations specialAttack;
    [SerializeField] private CombatAnimations parry;

    public WeaponType Type => weaponType;
    public bool TwoHanded => twoHanded;
    public CombatAnimations LightAttack => lightAttack;
    public CombatAnimations HeavyAttack => heavyAttack;
    public CombatAnimations SpecialAttack => specialAttack;
    public CombatAnimations Parry => parry;
}