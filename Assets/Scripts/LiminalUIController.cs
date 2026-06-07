using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LiminalUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private TextMeshProUGUI priceForUpgradeText;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI playerCrystalsText;  // quanto o jogador tem
    [SerializeField] private TextMeshProUGUI spendingNowText;     // gasto pendente global
    [SerializeField] private TextMeshProUGUI nextUpgradeText;     // preço do próximo upgrade (apenas informativo)
    
    [SerializeField] private TextMeshProUGUI vitalityText;
    [SerializeField] private TextMeshProUGUI enduranceText;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI dexterityText;

    [SerializeField] private TextMeshProUGUI playerLevel;

    [SerializeField] private GameObject baseMenu;
    [SerializeField] private GameObject upgradeMenu;
    [SerializeField] private GameObject auraMenu;

    // Guarda os upgrades pendentes por stat
    private Dictionary<PlayerStats.Stats, int> pendingUpgrades;

    // Soma global dos upgrades pendentes (quanto vai gastar)
    private int totalPendingPrice = 0;

    private int levelsToUpgrade;
    private PlayerController playerController;
    private PlayerState playerState;

    private bool menuIsActive = false;
    private Animator animator;
    private PlayerAnimationsController playerAnimationsController;
    [SerializeField]private BonfireMenuCamera cameraSettings;

    void Start()
    {
        pendingUpgrades = new Dictionary<PlayerStats.Stats, int>();
        
        playerState = FindFirstObjectByType<PlayerState>();
        playerController = FindFirstObjectByType<PlayerController>();
        playerAnimationsController = FindFirstObjectByType<PlayerAnimationsController>();
        animator = playerController.GetComponent<Animator>();

        foreach (PlayerStats.Stats stat in System.Enum.GetValues(typeof(PlayerStats.Stats)))
        {
            pendingUpgrades.Add(stat, 0);
        }

        UpdateUI();
    }

    public void BonfireMenu(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || (!playerState.IsOnBonfire && !playerState.PlayerCanMove) || playerAnimationsController.IsAttacking || playerState.IsInSettings || playerState.IsBeingChased() || playerState.IsOnInventory) return;
        if (playerState.playerIsInBonfire)
        {
            if (!menuIsActive)
            {
                TurnOn();
            }
            else
            {
                TurnOff();
            }

        }else
        {
            TurnOff();
        }
    }

    public void TurnOn()
    {        
        playerState.gameObject.GetComponent<Health>().Heal(1000);
        animator.SetTrigger("Down");
        animator.ResetTrigger("Up");
        animator.SetBool("Sitting", true);
        baseMenu.SetActive(true);
        playerState.PlayerCanMoveState(false);
        playerState.ChangeIsInBonfireState(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        menuIsActive = true;
        cameraSettings.ChangeToBonfire(new Vector3(
            playerState.bonfireLocation.x,
            playerState.bonfireLocation.y,
            playerState.bonfireLocation.z),
            new Vector3(
                playerState.transform.position.x,
                playerState.transform.position.y,
                playerState.transform.position.z)
            );
        
        PauseAllSpawners();
        foreach (AreaResetter areaResetter in FindObjectsByType<AreaResetter>(FindObjectsSortMode.None))
        {
            areaResetter.ResetArea();
        }
    }
    public void TurnOff()
    {
        animator.SetTrigger("Up");
        baseMenu.SetActive(false);
        upgradeMenu.SetActive(false);
        auraMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        menuIsActive = false;
        cameraSettings.ChangeToPlayer();
        ResetUpgrades();
        ResumeAllSpawners();
        
        if(playerState.IsOnBonfire) StartCoroutine(ResetRoutine());
        
    }

    private IEnumerator ResetRoutine() 
    {
        // Espera 5 segundos
        yield return new WaitForSeconds(1f);
        
        foreach (AreaResetter areaResetter in FindObjectsByType<AreaResetter>(FindObjectsSortMode.None))
        {
            areaResetter.ResetArea();
        }
    }


    private void PauseAllSpawners()
    {
        foreach (Spawner spawner in FindObjectsByType<Spawner>(FindObjectsSortMode.None))
            spawner.PauseSpawner();
    }

    private void ResumeAllSpawners()
    {
        foreach (Spawner spawner in FindObjectsByType<Spawner>(FindObjectsSortMode.None))
            spawner.ResumeSpawner();
    }

    public void DisableSittingAfterDelay()
    {
        playerState.PlayerCanMoveState(true);
        playerState.ChangeIsInBonfireState(false);
        animator.SetBool("Sitting", false);
    }

    // Métodos de wrapper para os botões
    public void UpgradeVitality() { UpgradeStat(PlayerStats.Stats.Vitality); }
    public void UpgradeEndurance() { UpgradeStat(PlayerStats.Stats.Endurance); }
    public void UpgradeMana() { UpgradeStat(PlayerStats.Stats.Mana); }
    public void UpgradeDefense() { UpgradeStat(PlayerStats.Stats.Defense); }
    public void UpgradeStrength() { UpgradeStat(PlayerStats.Stats.Strength); }
    public void UpgradeDexterity() { UpgradeStat(PlayerStats.Stats.Dexterity); }

    // Upgrade global
    public void UpgradeStat(PlayerStats.Stats stat)
    {
        int price = playerStats.PriceToUpgrade(levelsToUpgrade);

        // Só permite se tiver Crystal Shards suficientes (gasto global)
        if (playerStats.CrystalShards >= totalPendingPrice + price)
        {
            pendingUpgrades[stat]++;
            totalPendingPrice += price;

            levelsToUpgrade += 1;
            UpdateUI(price);
        }
    }

    // Confirma upgrades
    public void ConfirmUpgrade()
    {
        foreach (var entry in pendingUpgrades)
        {
            if (entry.Value > 0)
                playerStats.ApplyUpgrade(entry.Key, entry.Value);
        }
        playerStats.levelUp(levelsToUpgrade);
        playerStats.GiveCrystalShards(-totalPendingPrice);
        ResetUpgrades();
    }

    // Cancela upgrades pendentes
    public void ResetUpgrades()
    {
        foreach (PlayerStats.Stats stat in new List<PlayerStats.Stats>(pendingUpgrades.Keys))
        {
            pendingUpgrades[stat] = 0;
        }
        levelsToUpgrade = 0;
        totalPendingPrice = 0;
        UpdateUI();
        UpdateStatsUI();
    }

    // Atualiza UI de tudo
    private void UpdateUI(int nextPrice = -1)
    {
        playerCrystalsText.text = "Crystal Shards Total: " +  playerStats.CrystalShards;

        spendingNowText.text = "Crystal Shards Spending: " +  totalPendingPrice;

        nextUpgradeText.text = "Next Upgrade: " + playerStats.PriceToUpgrade(levelsToUpgrade);
        playerLevel.text = "Player Level: " + playerStats.PlayerLevel;
        


        // Mantém o restante do teu UpdateStatsUI original
        UpdateStatsUI();
    }

    // Mantém teu UpdateStatsUI original (opcional)
    public void UpdateStatsUI()
    {
        vitalityText.text = $"Vitality: {playerStats.TotalVitality}  -  ({pendingUpgrades[PlayerStats.Stats.Vitality]})";
        enduranceText.text = $"Endurance: {playerStats.TotalEndurance} - ({pendingUpgrades[PlayerStats.Stats.Endurance]})";
        manaText.text = $"Mana: {playerStats.TotalMana} - ({pendingUpgrades[PlayerStats.Stats.Mana]})";
        defenseText.text = $"Defense: {playerStats.TotalDefense} - ({pendingUpgrades[PlayerStats.Stats.Defense]})";
        strengthText.text = $"Strength: {playerStats.TotalStrength} - ({pendingUpgrades[PlayerStats.Stats.Strength]})";
        dexterityText.text = $"Dexterity: {playerStats.TotalDexterity} - ({pendingUpgrades[PlayerStats.Stats.Dexterity]})";
    }
}
