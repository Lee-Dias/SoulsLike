using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "CombatAnimations", menuName = "Scriptable Objects/CombatAnimations")]
public class CombatAnimations : ScriptableObject
{
    public enum WeaponType { Sword, Axe, Spear, Shield }
    public enum AttackType { Light,Heavy ,Special }
    [SerializeField] private bool isCombo;
    [HideIf(nameof(isCombo))] [SerializeField] private AudioClip soundEffect;
    [SerializeField] private AttackType attackType;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private bool stopControlledMovement;
    [SerializeField] private bool walkDuringAnimation;
    [SerializeField] private bool AttackAnimation;
    [HideIf(nameof(isCombo))] [SerializeField, Range(0.1f, 3f)] private float speed = 1f;

    [HideIf(nameof(isCombo))] [SerializeField] private AnimationClip singleClip;
    [ShowIf(nameof(ShouldShowWalk))] [SerializeField] private MovementInterval[] movementCurve;
    [ShowIf(nameof(isCombo))] [SerializeField] private ComboStep[] comboSteps;

    [System.Serializable]
    public struct MovementInterval
    {
        public float startTime;   // normalized start
        public float endTime;     // normalized end
        public Vector2 speed;     // custom movement X/Y
    }
    [System.Serializable]
    public struct ComboStep
    {
        [SerializeField] private string name;
        [SerializeField] private AnimationClip clip;
        [SerializeField] private AudioClip soundEffectCombo;
        [SerializeField] private float transitionWindowStart;
        [SerializeField] private float transitionWindowEnd;
        [SerializeField] private MovementInterval[] movementCurveOnCombo;
        [SerializeField, Range(0.1f, 1f)] private float nextAnimationTriggerPoint; // when to start next combo part
        [SerializeField, Range(0.1f, 3f)] private float comboSpeed;
        [SerializeField, Range(0.01f, 3f)] private float blendTime;
        [SerializeField, Range(0f, 1f)] private float hitStartTime;
        [SerializeField, Range(0f, 1f)] private float hitEndTime;

        public float HitStartTime => hitStartTime;
        public float HitEndTime => hitEndTime;
        public string Name => name;
        public AnimationClip Clip => clip;
        public float BlendTime =>blendTime;
        public AudioClip SoundEffectCombo => soundEffectCombo;
        public float TransitionWindowStart => transitionWindowStart;
        public float TransitionWindowEnd => transitionWindowEnd;
        public MovementInterval[] MovementCurveOnCombo => movementCurveOnCombo;
        public float NextAnimationTriggerPoint => nextAnimationTriggerPoint;
        public float ComboSpeed => comboSpeed;

    }
    private bool ShouldShowWalk()
    {
        return walkDuringAnimation && !isCombo;
    }

    public bool IsAttackAnimation => AttackAnimation;
    public bool IsCombo => isCombo;
    public bool StopControlledMovement => stopControlledMovement;
    public bool WalkDuringAnimation => walkDuringAnimation;
    public AudioClip SoundEffect => soundEffect;
    public AnimationClip SingleClip => singleClip;
    public ComboStep[] Steps => comboSteps;
    public MovementInterval[] MovementCurve => movementCurve;

    public WeaponType TypeWeapon => weaponType;
    public AttackType TypeAttack => attackType;
    public float Speed => speed;


}
