// CharacterProfileBase.cs
using UnityEngine;

public abstract class CharacterProfileBase : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    [Header("Base Stats")]
    [SerializeField, Range(1, 99)] private int baseVitlaity = 10;
    [SerializeField, Range(1, 99)] private int baseEndurance = 10; 
    [SerializeField, Range(1, 99)] private int baseMana = 10;
    [SerializeField, Range(1, 99)] private int baseDefense = 10;
    [SerializeField, Range(1, 99)] private int baseStrength = 10;
    [SerializeField, Range(1, 99)] private int baseDexterity = 10;
    

    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public int BaseVitlaity => baseVitlaity;
    public int BaseEndurance => baseEndurance;
    public int BaseDefense => baseDefense;
    public int BaseStrength => baseStrength;
    public int BaseDexterity => baseDexterity;
    public int BaseMana => baseMana;

}