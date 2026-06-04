using System.Collections;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    private PlayerStats playerStats;
    private float maxStamina;
    private float stamina;
    [SerializeField] private float staminaRegen = 0.2f;
    [SerializeField] private float timeToRegenStaminaAfterTake = 2f;
    private float amountOfStaminaToTake;

    // Controla se a stamina está sendo drenada por um golpe/ação rápida
    private float timeSinceLastTakeStamina = 0f;


    public float StaminaValue => stamina;
    public float MaxStamina => maxStamina;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        UpdateMaxStamin();
        stamina = maxStamina;
    }

    public void Update()
    {
        timeSinceLastTakeStamina += Time.deltaTime;
        // Só diminui se houver um custo contínuo (ex: correr)
        if (amountOfStaminaToTake > 0)
        {
            DecreaseStamina(amountOfStaminaToTake);
        }
        
        // Só regenera se não estiver gastando nada e não estiver no meio de um dreno suave
        if (amountOfStaminaToTake <= 0 && (timeSinceLastTakeStamina >= timeToRegenStaminaAfterTake) )
        {
            RegenStamina(staminaRegen);
        }
    }

    private void DecreaseStamina(float amount)
    {
        stamina = Mathf.Max(0, stamina - (amount * Time.deltaTime));
    }

    public void ChangeAmountOfStaminaToTake(float newAmount)
    {
        amountOfStaminaToTake = newAmount;
    }

    private void RegenStamina(float s)
    {
        if (stamina < maxStamina)
        {
            stamina = Mathf.Min(maxStamina, stamina + s * Time.deltaTime);
        }
    }

    public void UpdateMaxStamin()
    {
        maxStamina = 100 + (1.5f * playerStats.TotalEndurance);
    }

    // --- A MUDANÇA PRINCIPAL AQUI ---
    public void TakeStamina(float staminaTaken)
    {
        stamina -= staminaTaken;
        timeSinceLastTakeStamina = 0f; 
    }


}