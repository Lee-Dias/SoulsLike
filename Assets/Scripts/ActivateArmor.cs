using UnityEngine;

public class ActivateArmor : MonoBehaviour
{
    [SerializeField] private GameObject armor;
    [SerializeField] private GameObject mainBody;
    private PlayerState playerState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerState = FindFirstObjectByType<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState != null)        {
            if (playerState.HasArmorEquipped)
            {
                armor.SetActive(true);
                mainBody.SetActive(false);
            }
            else
            {
                armor.SetActive(false);
                mainBody.SetActive(true);
            }
        }
    }
}
