using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private AudioSource audioSource;

    // Guarda o tempo (Time.time) em que cada clip foi tocado pela última vez
    private Dictionary<AudioClip, float> lastPlayedTime = new Dictionary<AudioClip, float>();
    private const float duplicateWindow = 0.3f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip audioClip = null, AudioClip[] clips = null, float delay = 0f, float volume = 1f, float pitchmin = 1f, float pitchmax = 1f, float spatialBlend = 0f, Transform audioSourceTransform = null, float maxDistance = 10f)
    {
        if (instance == null)
            return;

        AudioClip clipToPlay = null;

        if (clips != null && clips.Length > 0)
            clipToPlay = clips[UnityEngine.Random.Range(0, clips.Length)];
        else if (audioClip != null)
            clipToPlay = audioClip;
        else
            return;

        // Bloqueia se foi tocado nos últimos 0.3 segundos
        if (lastPlayedTime.TryGetValue(clipToPlay, out float lastTime) && Time.time - lastTime < duplicateWindow)
            return;

        StartCoroutine(PlayDelayed(() =>
        {
            if (clipToPlay == null)
                return;

            // Regista o tempo atual
            lastPlayedTime[clipToPlay] = Time.time;

            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = audioSourceTransform != null ? audioSourceTransform.position : transform.position;

            AudioSource tempSource = tempGO.AddComponent<AudioSource>();
            tempSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            tempSource.clip = clipToPlay;
            tempSource.volume = volume;
            tempSource.pitch = UnityEngine.Random.Range(pitchmin, pitchmax);
            tempSource.maxDistance = maxDistance;
            tempSource.rolloffMode = AudioRolloffMode.Linear;
            tempSource.spatialBlend = spatialBlend;
            tempSource.Play();

            float destroyDelay = clipToPlay.length / tempSource.pitch;
            Destroy(tempGO, destroyDelay);

        }, delay));
    }

    private IEnumerator PlayDelayed(Action playAction, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        playAction?.Invoke();
    }
}