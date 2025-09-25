using UnityEngine;

public class Item
{

    protected int value;
    protected string name;
    protected string description;
    protected int quantity;

    public Item(string name, int value, string description, int quantity)
    {
        this.value = value;
        this.name = name;
        this.description = description;
        this.quantity = quantity;
    }
}
