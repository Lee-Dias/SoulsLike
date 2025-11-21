using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public interface ICombatActor
{
    Animator Animator { get; }
    PlayableGraph Graph { get; }
    AnimationMixerPlayable Mixer { get; }
    Transform Transform { get; }
    void ApplyDamage(float amount);
    void ConsumeStamina(float amount);
}