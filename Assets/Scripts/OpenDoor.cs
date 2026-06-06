using UnityEngine;
using UnityEngine.InputSystem;

public class OpenDoor : MonoBehaviour
{
     [SerializeField] private Animator[] Animators;
     private bool playerInside = false;


    // Update is called once per frame
    public void Open()
    {
            foreach (Animator animator in Animators)
            {
                animator.SetTrigger("Open");
            }
            Destroy(this);
        

    }

    
    
}
