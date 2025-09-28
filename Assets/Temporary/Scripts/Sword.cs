using UnityEngine;

public class Sword : Weapon
{
    public float Damage { get; private set; }
    public Sword(string name, int value, string description, int quantity, int damage) : base(name, value, description, quantity, damage)
    {
        Damage = damage;
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
