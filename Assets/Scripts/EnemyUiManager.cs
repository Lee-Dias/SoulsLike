using UnityEngine;
using UnityEngine.UI;

public class EnemyUiManager : MonoBehaviour
{
    [SerializeField]private Health health;
    [SerializeField]private Image healthBar;
    [SerializeField]private Shield shield;
    [SerializeField]private Image shieldBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health != null && healthBar != null)
            healthBar.fillAmount = health.HealthValue / health.MaxHealth;
        if(shield != null && shieldBar != null)
            shieldBar.fillAmount = shield.ShieldValue / shield.MaxShieldHealth;
    }
}
