using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public enum Stats {Vitality, Endurance, Mana, Defense, Strength, Dexterity};

    [Header("Profile Reference")]
    [SerializeField] private PlayerProfile playerProfile;

    private int playerLevel = 1 ;

    [Header("Points Invested")]
    private int vitalityPoints = 0 ;
    private int endurancePoints = 0;
    private int manaPoints = 0;
    private int defensePoints = 0;
    private int strengthPoints = 0;
    private int dexterityPoints = 0;




    private int bonusDefense = 0;
    private int bonusStrength = 0 ;
    private int bonusDexterity = 0 ;

    
    private int crystalShards = 0;


    // --- Totals (Base + Invested) ---
    public int TotalVitality => playerProfile.BaseVitlaity + vitalityPoints;
    public int TotalEndurance => playerProfile.BaseEndurance + endurancePoints;
    public int TotalMana => playerProfile.BaseMana + manaPoints;
    public int TotalDefense => playerProfile.BaseDefense + defensePoints;
    public int TotalStrength => playerProfile.BaseStrength + strengthPoints;
    public int TotalDexterity => playerProfile.BaseDexterity + dexterityPoints;

    public int CrystalShards => crystalShards;

    public int PlayerLevel => playerLevel;


    // --- Example: Derived Values ---
    public float MaxHealth => TotalVitality * 10f;
    public float MaxStamina => TotalEndurance * 8f;
    public float MaxMana => TotalMana * 5f;

    public void OnAddShard(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        GiveCrystalShards(10000);
        
    }

    public void GiveCrystalShards(int crystalShardsToAdd)
    {
        crystalShards += crystalShardsToAdd;
    }

    public int PriceToUpgrade(int levelsPreUpgraded = 0)
    {
        return (int)(1000 + ((playerLevel + levelsPreUpgraded) * 150f));
    }
    public void ApplyUpgrade(Stats stat, int amount)
    {
        switch (stat)
        {
            case Stats.Vitality:
                vitalityPoints += amount;
                break;
            case Stats.Endurance:
                endurancePoints += amount;
                break;
            case Stats.Mana:
                manaPoints += amount;
                break;
            case Stats.Defense:
                defensePoints += amount;
                break;
            case Stats.Strength:
                strengthPoints += amount;
                break;
            case Stats.Dexterity:
                dexterityPoints += amount;
                break;
        }

    }
    public void levelUp(int amount = 1)
    {
        playerLevel += amount;
    }



}
