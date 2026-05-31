using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource sourceA;
    [SerializeField] private AudioSource sourceB;

    private AudioSource currentSource;
    private AudioSource nextSource;

    private Dictionary<AudioClip, float> clipStartTimes =
        new Dictionary<AudioClip, float>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentSource = sourceA;
        nextSource = sourceB;
    }

    public void PlayMusic(AudioClip newClip, float fadeTime = 2f)
    {
        if (newClip == null)
            return;

        // Avoids restarting the same music
        if (currentSource.clip == newClip && currentSource.isPlaying)
            return;

        // Registers when this music started existing
        if (!clipStartTimes.ContainsKey(newClip))
        {
            clipStartTimes[newClip] = Time.time;
        }

        StartCoroutine(CrossFade(newClip, fadeTime));
    }

    private IEnumerator CrossFade(AudioClip newClip, float fadeTime)
    {
        nextSource.clip = newClip;

        // Calculates where the music is now
        float elapsed = Time.time - clipStartTimes[newClip];

        if (newClip.length > 0)
        {
            nextSource.time = elapsed % newClip.length;
        }

        nextSource.volume = 0f;
        nextSource.Play();

        float timer = 0f;
        float startVolume = currentSource.volume;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            float t = timer / fadeTime;

            currentSource.volume =
                Mathf.Lerp(startVolume, 0f, t);

            nextSource.volume =
                Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        currentSource.Stop();
        currentSource.volume = 1f;

        AudioSource temp = currentSource;
        currentSource = nextSource;
        nextSource = temp;
    }
}