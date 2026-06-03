using UnityEngine;

public class PlayerState : MonoBehaviour
{

    [SerializeField] private float enemyCheckRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask wallLayer;

    private Inventory inventory;


    private bool playerCanMove = true;

    private bool isDefending = false;  
    private bool isOnInventory;
    private bool isOnBonfire;
    private bool isInSettings;
    private bool enemyAround;
    private bool hasArmorEquipped = false;
    
    public bool EnemyAround => enemyAround;
    public bool PlayerCanMove => playerCanMove;
    public bool IsOnInventory => isOnInventory;
    public bool IsInSettings => isInSettings;
    public bool IsOnBonfire => isOnBonfire;
    public bool HasArmorEquipped => hasArmorEquipped;
    
    [SerializeField] private GameObject interactionMessage;
    [HideInInspector] public bool playerIsInBonfire;
    [HideInInspector]public LiminalWorldChanger playerBonfire;
    [HideInInspector] public Vector3 bonfireLocation;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    public void ChangeHasArmorEquipped(bool state)
    {
        hasArmorEquipped = state;
    }

    public void PlayerCanMoveState(bool state)
    {
        playerCanMove = state;
    }
    public void ChangeIsInInventoryState(bool state)
    {
        isOnInventory = state;  
    }
    public void ChangeIsInSettingsState(bool state)
    {
        isInSettings = state;  
    }
    public void ChangeIsInBonfireState(bool state)
    {
        isOnBonfire = state;  
        CheckInteractionMessageState();
    }

    public void CheckInteractionMessageState()
    {
        if (playerIsInBonfire && !isOnBonfire)
        {
            ChangeInteractionMessageState(true);
        }
        else
        {
            ChangeInteractionMessageState(false);
        }
    }
    public void ChangeInteractionMessageState(bool state)
    {
        interactionMessage.SetActive(state);
    }
    private void Update()
    {
        enemyAround = IsEnemyNearby();
    }
    public bool IsEnemyNearby()
    {
        // 1. Encontra todos os colliders na camada de inimigos dentro do raio
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, enemyCheckRadius, enemyLayer);

        foreach (Collider enemyCollider in enemiesInRange)
        {
            // 2. Calcula a direção e distância para o inimigo
            Vector3 directionToEnemy = (enemyCollider.transform.position - transform.position).normalized;
            float distanceToEnemy = Vector3.Distance(transform.position, enemyCollider.transform.position);

            // 3. Lança um raio para ver se bate numa parede antes de chegar ao inimigo
            // Usamos transform.position + Vector3.up para o raio não sair "do chão"
            if (!Physics.Raycast(transform.position + Vector3.up, directionToEnemy, distanceToEnemy, wallLayer))
            {
                // Se o raio NÃO bateu em nenhuma parede, significa que o inimigo está visível
                return true; 
            }
        }

        return false;
    }
    private void OnDrawGizmos()
    {
        if (enemyAround)
            Gizmos.color = Color.red;   // enemy detected
        else
            Gizmos.color = Color.green; // no enemies

        Gizmos.DrawWireSphere(transform.position, enemyCheckRadius);
    }

    public float getAmountToDefend()
    {
        if(isDefending)
        {
            float a;
            if(inventory.GetItemOnArmourSlot() != null)
            {
                a = inventory.GetItemOnLeftHand().Damage + inventory.GetItemOnArmourSlot().ArmorQuantity;
            }
            else
            {
                a = inventory.GetItemOnLeftHand().Damage;
            }

           
           return a;
        }
         else
        {
            if(inventory.GetItemOnArmourSlot() == null) return 0;
            float a = inventory.GetItemOnArmourSlot().ArmorQuantity;
            return a;
        }
    }

    public void HandleDefense(bool def)
    {
        isDefending = def;
    }
    public bool IsDefending()
    {
        return isDefending;
    }
}
