using UnityEngine;

public class MusicZone : MonoBehaviour
{
    [SerializeField] private AudioClip areaMusic;
    [SerializeField] private float transitionTime = 2f;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.PlayMusic(areaMusic, transitionTime);
        }
    }
}