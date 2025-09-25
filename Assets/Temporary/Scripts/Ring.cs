using UnityEngine;

public class Ring : Item
{
    public int buff;
    public Ring(string name, int value, string description, int quantity, int buff) : base(name, value, description, quantity)
    {
        this.buff = buff;
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
