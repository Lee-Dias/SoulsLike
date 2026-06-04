using UnityEngine;
using UnityEngine.UI;

public class PlayerUiManager : MonoBehaviour
{
    [SerializeField]
    private Image staminaBar;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image staminaHiddenBar;
    [SerializeField]
    private Image healthHiddenBar;
    [SerializeField]
    private Stamina stamina;
    [SerializeField]
    private Health health;
    [SerializeField]
    private PlayerStats playerStats;

    [Header("Configurações de Suavizado")]
    [SerializeField]
    private float timeTake = 1f; // O tempo que leva para esvaziar de 100% a 0%

    void Start()
    {
    }

    void Update()
    {
        // 1. Valores Alvo (Frações de 0.0 a 1.0)
        float targetHealth = health.HealthValue / health.MaxHealth;
        float targetStamina = stamina.StaminaValue / stamina.MaxStamina;

        // 2. Atualização Instantânea (Barras Normais)
        healthBar.fillAmount = targetHealth;
        staminaBar.fillAmount = targetStamina;

        // 3. Velocidade de transição para o fillAmount (de 0 a 1 em 0.1 segundos)
        // Como o fillAmount vai de 0.0 a 1.0, a velocidade total é 1.0 / tempoDeQueda
        float velocidadePreenchimento = 1.0f / timeTake;

        // 4. Atualização Gradual (Barras Hidden)
        healthHiddenBar.fillAmount = Mathf.MoveTowards(
            healthHiddenBar.fillAmount, 
            targetHealth, 
            velocidadePreenchimento * Time.deltaTime
        );

        staminaHiddenBar.fillAmount = Mathf.MoveTowards(
            staminaHiddenBar.fillAmount, 
            targetStamina, 
            velocidadePreenchimento * Time.deltaTime
        );
    }
}