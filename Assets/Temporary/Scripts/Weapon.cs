using UnityEngine;

public class Weapon : Item
{
     public int damage;
    public Weapon(string name, int value, string description, int quantity, int damage) : base(name, value, description, quantity)
    {
        this.damage = damage;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
