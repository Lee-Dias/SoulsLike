using UnityEngine;
using UnityEngine.UI;

public class EnemyUiManager : MonoBehaviour
{
    [SerializeField]private Health health;
    [SerializeField]private Image healthBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = health.HealthValue / health.MaxHealth;
    }
}
