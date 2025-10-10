using UnityEngine;

[CreateAssetMenu(fileName = "New Wearable", menuName = "Item/Wearable")]
public class WearableScript : ScriptableObject
{
    public int value;
    public new string name;
    public string description;
    public int quantity;
    public int damage;
}
