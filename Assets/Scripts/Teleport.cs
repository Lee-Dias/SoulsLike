using System;
using System.Collections;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Animator animator;
    
    public void TeleportPlayer(Transform teleportLocation)
    {
        animator.SetTrigger("Change");
        StartCoroutine(TeleportAfterDelay(teleportLocation, 1f));
        
    }

    private IEnumerator TeleportAfterDelay(Transform teleportLocation, float delay)
    {
        yield return new WaitForSeconds(delay);
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        player.transform.SetPositionAndRotation(
            teleportLocation.position,
            teleportLocation.rotation
        );
        if (cc != null)
            cc.enabled = true;
    }
}
