using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Profile Reference")]
    [SerializeField] private PlayerProfile playerProfile;

    [Header("Points Invested")]
    private int endurancePoints;
    private int strengthPoints;
    private int dexterityPoints;
    private int manaPoints;
    private int vitalityPoints;

    // --- Totals (Base + Invested) ---
    public int TotalVitality => playerProfile.BaseVitlaity + vitalityPoints;
    public int TotalEndurance => playerProfile.BaseEndurance + endurancePoints;
    public int TotalMana => playerProfile.BaseMana + manaPoints;
    public int TotalDefense => playerProfile.BaseDefense + endurancePoints;
    public int TotalStrength => playerProfile.BaseStrength + strengthPoints;
    public int TotalDexterity => playerProfile.BaseDexterity + dexterityPoints;

    // --- Example: Derived Values ---
    public float MaxHealth => TotalVitality * 10f;
    public float MaxStamina => TotalEndurance * 8f;
    public float MaxMana => TotalMana * 5f;


}
