using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    GameObject background;
    GameObject closeButton;
    GameObject settingsMenu;
    GameObject settingsOptions;
    Slider volumeSlider;
    
    
    PlayerController playerController;
    bool on = false;
    bool inOptions = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //------------UI Management------------//
        background = transform.GetChild(0).gameObject;
        closeButton = transform.GetChild(1).gameObject;
        settingsMenu = transform.GetChild(2).gameObject;
        settingsOptions = transform.GetChild(3).gameObject;
        playerController = FindFirstObjectByType<PlayerController>();

        for (int i = 0; i < settingsOptions.transform.childCount; i++)
        {
            if (settingsOptions.transform.GetChild(i).name == "VolumeSlider")
            {
                volumeSlider = settingsOptions.transform.GetChild(i).GetComponent<Slider>();
            }
        }
        //------------End of UI Management------------//

        //------------Volume Management------------//
        if(!PlayerPrefs.HasKey("MainVolume"))
        {
            PlayerPrefs.SetFloat("MainVolume", 1f);
            Load();
        }
        else
        {
            Load();
        }
        //------------End of Volume Management------------//
    }

    // Update is called once per frame
    void Update()
    {
        //------------UI Management------------//
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
        //------------End of UI Management------------//
    }

    //------------UI Management Method------------//
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerController.PlayerCanMoveState(true);
    }
    //------------End of UI Management Method------------//

    //------------Volume Management Method------------//
    public void MainVolumeControl(System.Single volume)
    {
        AudioListener.volume = volume;
        Save(volume);
    }

    void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("MainVolume");
        AudioListener.volume = PlayerPrefs.GetFloat("MainVolume");
    }

    void Save(System.Single volume)
    {
        PlayerPrefs.SetFloat("MainVolume", volume);
    }
    //------------End of Volume Management Method------------//


}
