using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tabsToActivate;
    [SerializeField] private Button buttonToSelect;
    [SerializeField] private Slider gammaSlider;
    [SerializeField] private VolumeProfile[] volumeProfile;

    private PlayerState playerState;
    // Alterado para array para suportar múltiplos perfis
    private LiftGammaGain[] liftGammaGain;
    private bool isOpen = false;

    private void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();

        // Inicializa o array com o mesmo tamanho dos perfis disponíveis
        liftGammaGain = new LiftGammaGain[volumeProfile.Length];

        for (int i = 0; i < volumeProfile.Length; i++)
        {
            if (volumeProfile[i] != null && volumeProfile[i].TryGet<LiftGammaGain>(out var tmp))
            {
                liftGammaGain[i] = tmp;

                // Usa o valor do primeiro perfil válido para inicializar o slider na UI
                if (gammaSlider != null)
                {
                    gammaSlider.value = liftGammaGain[i].gamma.value.w;
                }
            }
        }
    }

    public void OnOpenTab(InputAction.CallbackContext context)
    {
        // Certifica-se de ler o input apenas quando o botão for pressionado (started)
        if (!context.started) return;

        if (playerState.IsOnBonfire || playerState.IsOnInventory) return;

        if (!isOpen) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerState.ChangeIsInSettingsState(true);
            playerState.PlayerCanMoveState(false);
            
            foreach (var tab in tabsToActivate)
            {
                if (tab != null) tab.SetActive(true);
            }
            isOpen = true;
            EventSystem.current.SetSelectedGameObject(null); 
            EventSystem.current.SetSelectedGameObject(buttonToSelect.gameObject);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerState.ChangeIsInSettingsState(false);
            playerState.PlayerCanMoveState(true);
            
            foreach (var tab in tabsToActivate)
            {
                if (tab != null) tab.SetActive(false);
            }
            isOpen = false;
        }
    } 

    public void ChangeGammaValue()
    {
        if (liftGammaGain == null || gammaSlider == null) return;

        // Percorre todos os componentes guardados no array e atualiza o Gamma de cada um
        foreach (var lgg in liftGammaGain)
        {
            if (lgg != null)
            {
                // No URP/HDRP, Gamma é um Vector4 (X, Y, Z, W) onde W é o Master
                Vector4 newGamma = lgg.gamma.value;
                newGamma.w = gammaSlider.value;
                
                lgg.gamma.overrideState = true;
                lgg.gamma.value = newGamma;
            }
        }
    }
}