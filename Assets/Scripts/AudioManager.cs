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
    public void PlayAudio(AudioClip audioClip = null, AudioClip[] clips = null, float delay = 0f, float volume = 1f,float pitchmin = 1f, float pitchmax = 1f)
    {
        if (instance == null)
            return;

        AudioClip clipToPlay = null;

        // Pick random from list if available
        if (clips != null && clips.Length > 0)
        {
            clipToPlay = clips[UnityEngine.Random.Range(0, clips.Length)];
        }
        else if (audioClip != null)
        {
            clipToPlay = audioClip;
        }
        else
        {
            return;
        }

        StartCoroutine(PlayDelayed(() =>
        {
            if (clipToPlay == null)
                return;
            // Create temporary GameObject
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = transform.position;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.clip = clipToPlay;
            tempSource.volume = volume;
            tempSource.pitch = UnityEngine.Random.Range(pitchmin, pitchmax);
            tempSource.Play();

            // Destroy after finished playing
            Destroy(tempGO, clipToPlay.length / tempSource.pitch);

        }, delay));
    }

    private IEnumerator PlayDelayed(Action playAction, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        playAction?.Invoke();
    }
}
