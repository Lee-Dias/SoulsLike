using UnityEngine;

public class LockOnSymbol : MonoBehaviour
{

    [SerializeField] private GameObject lockOnSymbol;
    

    public void ActivateSymbol()
    {
        if(lockOnSymbol == null) return; 
        
        if (!lockOnSymbol.activeInHierarchy)
        {
            lockOnSymbol.SetActive(true);
        }
        else
        {
            lockOnSymbol.SetActive(false);
        }


    }
}
