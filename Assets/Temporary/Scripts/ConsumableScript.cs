using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Item/Consumable")]
public class ConsumableScript : ScriptableObject
{
    public int value;
    public new string name;
    public string description;
    public int quantity;
    public int damage;
}
