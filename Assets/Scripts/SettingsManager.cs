using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    GameObject background;
    GameObject closeButton;
    GameObject settingsMenu;
    GameObject settingsOptions;
    
    
    PlayerController playerController;
    bool on = false;
    bool inOptions = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        background = transform.GetChild(0).gameObject;
        closeButton = transform.GetChild(1).gameObject;
        settingsMenu = transform.GetChild(2).gameObject;
        settingsOptions = transform.GetChild(3).gameObject;
        playerController = FindFirstObjectByType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (on)
        {
            background.SetActive(true);
            closeButton.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerController.PlayerCanMoveState(false);

            if (inOptions)
            {
                settingsOptions.SetActive(true);
                settingsMenu.SetActive(false);
            }
            else
            {
                settingsOptions.SetActive(false);
                settingsMenu.SetActive(true);
            }
        }
        else
        {
            TurnOff();
        }
    }

    public void OpenCloseStettingsInputPlayer(InputAction.CallbackContext ctx)
    {
        if(inOptions) 
        {
            inOptions = false;
            on = true;
        }
        else if(!on)
        {
            on = true;
        }
        else
        {
            on = false;
        }
            
    }
    public void OpenCloseStettings()
    {
        if (inOptions)
        {
            inOptions = false;
        }
        else
        {
            on = false;
        }
    }

    public void OpenOptions()
    {
        inOptions = true;
    }

    void TurnOff()
    {
        settingsOptions.SetActive(false);
        settingsMenu.SetActive(false);
        background.SetActive(false);
        closeButton.SetActive(false);
    }
}
