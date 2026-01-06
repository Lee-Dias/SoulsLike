using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates
        }
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void PlayAudio(AudioClip audioClip)
    {
        audioSource.volume = 1f;
        audioSource.pitch = 1f; 
        audioSource.PlayOneShot(audioClip);
    }
    public void PlayAudioWithRandomPitch(AudioClip audioClip, float pitchmin, float pitchmax, float volume)
    {
        audioSource.pitch = 1f; 
        if (audioClip == null || audioSource == null)
            return;
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(pitchmin, pitchmax); // Adjust range as needed
        audioSource.PlayOneShot(audioClip);
        
    }
}
