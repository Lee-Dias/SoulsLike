using System;
using UnityEditor;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    private PlayerStats playerStats;
    private float maxStamina;
    private float stamina;
    [SerializeField]private float staminaRegen = 0.2f;
    private float amountOfStaminaToTake;

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
        DecreaseStamina(amountOfStaminaToTake);
        RegenStamina(staminaRegen);
    }

    private void DecreaseStamina(float amountOfStaminaToTake)
    {
        stamina -= amountOfStaminaToTake * Time.deltaTime;
    }
    public void ChangeAmountOfStaminaToTake(float newAmount)
    {
        amountOfStaminaToTake= newAmount;
    }
    private void RegenStamina(float s)
    {
        if(amountOfStaminaToTake > 0) return;

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
