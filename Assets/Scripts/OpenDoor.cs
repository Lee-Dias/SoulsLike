using UnityEngine;
using UnityEngine.InputSystem;

public class OpenDoor : MonoBehaviour
{
     [SerializeField] private Animator[] Animators;
     private bool playerInside = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    // Update is called once per frame
    public void Open(InputAction.CallbackContext ctx)
    {
        if (playerInside)
        {
            foreach (Animator animator in Animators)
            {
                animator.SetTrigger("Open");
            }
            Destroy(this);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    
    
}
