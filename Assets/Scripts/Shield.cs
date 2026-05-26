using UnityEngine;

public class Shield : MonoBehaviour
{
    
    [SerializeField] private float maxShieldHealth = 100f; // Valor máximo da saúde do escudo
    private float shieldHealth = 100f; // Valor inicial da saúde do escudo~
    private BarrierDissolve barrierDissolve; // Referência ao componente de dissolução do escudo

    public float MaxShieldHealth => maxShieldHealth;
    public float ShieldValue => shieldHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shieldHealth = maxShieldHealth;
        barrierDissolve = GetComponent<BarrierDissolve>(); // Obtém a referência ao componente de dissolução
    }  

    // Update is called once per frame
    public void TakeShieldDamage(float damage)
    {
        if (shieldHealth <= 0)
        {
            this.GetComponent<Health>().GetHit(damage);
        }

        shieldHealth -= damage;

        if (shieldHealth <= 0)
        {
            shieldHealth = 0;
        }

        if (barrierDissolve != null)
        {
            barrierDissolve.ChangeBarrierValues();
        }
    }

    public bool IsShieldBroken()
    {
        return shieldHealth <= 0;
    }

    public void RestoreShield()
    {

        shieldHealth = maxShieldHealth;
        
    }
}
