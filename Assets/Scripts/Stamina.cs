using UnityEditor;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    private PlayerStats playerStats;
    private float maxStamina;
    private float stamina;
    private float staminaRegen = 0.05f;
    public float StaminaValue => stamina;
    public float MaxStamina => maxStamina;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        UpdateMaxStamin();
        stamina = maxStamina;
    }
    public void Update()
    {
        RegenStamina(staminaRegen);
        
    }
    private void RegenStamina(float s)
    {
        if (stamina < maxStamina)
        {
            if (staminaRegen > (maxStamina - stamina) )
            {
                stamina += maxStamina - stamina;
            }
            else
            {
                stamina += s;
            }           
        } 
    }
    public void UpdateMaxStamin()
    {
        maxStamina = 100 + (1.5f * playerStats.TotalEndurance);
    }

    // Update is called once per frame
    public void TakeStamina(float staminaTaken)
    {
        stamina -= staminaTaken; 
    }
}
