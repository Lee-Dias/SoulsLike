using UnityEngine;

public class PlayerState : MonoBehaviour
{

    [SerializeField] private float enemyCheckRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;

    private Inventory inventory;


    private bool playerCanMove = true;

    private bool isDefending = false;  
    private bool isOnInventory;
    private bool isOnBonfire;
    private bool isInSettings;
    private bool enemyAround;
    
    public bool EnemyAround => enemyAround;
    public bool PlayerCanMove => playerCanMove;
    public bool IsOnInventory => isOnInventory;
    public bool IsInSettings => isInSettings;
    public bool IsOnBonfire => isOnBonfire;
    
    [SerializeField] private GameObject interactionMessage;
    [HideInInspector] public bool playerIsInBonfire;
    [HideInInspector]public LiminalWorldChanger playerBonfire;
    [HideInInspector] public Vector3 bonfireLocation;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
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
        return Physics.CheckSphere(transform.position, enemyCheckRadius, enemyLayer);
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
           float a = inventory.GetItemOnLeftHand().Damage;
           return a;
        }
         else
        {
        }
        return 0f;
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
