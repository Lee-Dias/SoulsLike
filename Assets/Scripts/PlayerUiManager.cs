using UnityEngine;
using UnityEngine.UI;

public class PlayerUiManager : MonoBehaviour
{
    [SerializeField]
    private Image staminaBar;
    [SerializeField]
    private Stamina stamina;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        staminaBar.fillAmount = stamina.StaminaValue / stamina.MaxStamina;
    }
}
