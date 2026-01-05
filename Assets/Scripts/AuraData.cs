using UnityEngine;

[CreateAssetMenu(fileName = "AuraData", menuName = "Scriptable Objects/AuraData")]
public class AuraData : ScriptableObject
{
    [SerializeField]private float parryTimeScaleMultiplier = 1f;
    [SerializeField]private float parryDurationMultiplier = 1f;
    [SerializeField]private float parryRadiusMultiplier = 1f;

    public float ParryTimeScaleMultiplier => parryTimeScaleMultiplier;
    public float ParryDurationMultiplier => parryDurationMultiplier;
    public float ParryRadiusMultiplier => parryRadiusMultiplier;
    
}
