using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Item/Weapon")]
public class WeaponScript : ScriptableObject
{
    public int value;
    public new string name;
    public string description;
    public int quantity;
    public int damage;
}
