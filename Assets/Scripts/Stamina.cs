using System.Collections;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    private PlayerStats playerStats;
    private float maxStamina;
    private float stamina;
    [SerializeField] private float staminaRegen = 0.2f;
    private float amountOfStaminaToTake;

    // Controla se a stamina está sendo drenada por um golpe/ação rápida
    private bool isDrainingSmoothly = false;

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
        // Só diminui se houver um custo contínuo (ex: correr)
        if (amountOfStaminaToTake > 0)
        {
            DecreaseStamina(amountOfStaminaToTake);
        }
        
        // Só regenera se não estiver gastando nada e não estiver no meio de um dreno suave
        if (amountOfStaminaToTake <= 0 && !isDrainingSmoothly)
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
        // Inicia a redução suave ao longo de 1 segundo
        StartCoroutine(SmoothTakeRoutine(staminaTaken, 1f));
    }

    private IEnumerator SmoothTakeRoutine(float amountToTake, float duration)
    {
        isDrainingSmoothly = true;
        float elapsed = 0f;
        float startStamina = stamina;
        float targetStamina = Mathf.Max(0, stamina - amountToTake);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Interpolação Linear (Lerp) para suavizar a descida
            stamina = Mathf.Lerp(startStamina, targetStamina, elapsed / duration);
            yield return null;
        }

        stamina = targetStamina;
        isDrainingSmoothly = false;
    }
}