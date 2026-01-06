using System;
using System.Collections;
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
    public void PlayAudio(AudioClip audioClip, float delay = 0f)
    {
        if (audioClip == null || audioSource == null)
            return;

        StartCoroutine(PlayDelayed(() =>
        {
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
            audioSource.PlayOneShot(audioClip);
        }, delay));
    }
    private IEnumerator PlayDelayed(Action playAction, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        playAction?.Invoke();
    }
    public void PlayAudioWithRandomPitch(AudioClip audioClip, float volume = 1f ,float pitchmin= 0.9f, float pitchmax= 1.1f)
    {
        audioSource.pitch = 1f; 
        if (audioClip == null || audioSource == null)
            return;
        audioSource.volume = volume;
        audioSource.pitch = UnityEngine.Random.Range(pitchmin, pitchmax); // Adjust range as needed
        audioSource.PlayOneShot(audioClip);
        
    }
    public void PlayRandomFromListWithRandomPitch(
    AudioClip[] clips,
    float volume = 1f,
    float pitchMin = 0.9f,
    float pitchMax = 1.1f
    )
    {
        if (audioSource == null)
            return;

        if (clips == null || clips.Length == 0)
            return;

        AudioClip chosenClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        audioSource.volume = volume;
        audioSource.pitch = UnityEngine.Random.Range(pitchMin, pitchMax);
        audioSource.PlayOneShot(chosenClip);
    }
}
