using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;


public class SettingsManager : MonoBehaviour
{
    [SerializeField]private GameObject[] tabsToActivate;
    [SerializeField]private Button buttonToSelect;
    [SerializeField]private Slider gammaSlider;
    [SerializeField]private VolumeProfile volumeProfile;

    private PlayerState playerState;
    private LiftGammaGain liftGammaGain;
    private bool isOpen = false;
    

    private void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
        // Try to get the LiftGammaGain component from the profile
        if (volumeProfile.TryGet<LiftGammaGain>(out var tmp))
        {
            liftGammaGain = tmp;
            // Initialize slider value to match current profile value
            gammaSlider.value = liftGammaGain.gamma.value.w; 
        }
    }

    public void OnOpenTab(InputAction.CallbackContext context)
    {
        if(playerState.IsOnBonfire || playerState.IsOnInventory) return;
        if (!isOpen) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerState.ChangeIsInSettingsState(true);
            playerState.PlayerCanMoveState(false);
            
            foreach (var tab in tabsToActivate)
            {
                tab.SetActive(true);
            }
            isOpen = true;
            EventSystem.current.SetSelectedGameObject(null); // clear current selection
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
                tab.SetActive(false);
            }
            isOpen = false;
        }
    } 

    public void ChangeGammaValue()
    {

        if (liftGammaGain != null)
        {
            // In HDRP, Gamma is a Vector4 (X, Y, Z, W)
            // W is usually the 'Master' slider
            Vector4 newGamma = liftGammaGain.gamma.value;
            newGamma.w = gammaSlider.value;
            liftGammaGain.gamma.overrideState = true;
            liftGammaGain.gamma.value = newGamma;
        }

    }


    
}
