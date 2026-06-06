using UnityEngine;

public class RandomTalk : MonoBehaviour
{
    [SerializeField] private AudioClip[] talkList;
    [SerializeField] private float minTalkInterval = 5f;
    [SerializeField] private float maxTalkInterval = 15f;
    private float talkTimer = 0f;
    private float nextTalkTime = 0f;

    private AudioManager audioManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextTalkTime = Random.Range(minTalkInterval, maxTalkInterval);
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        talkTimer += Time.deltaTime;
        if (talkTimer >= nextTalkTime)
        {
            nextTalkTime = Random.Range(minTalkInterval, maxTalkInterval);
            talkTimer = 0f;
            audioManager.PlayAudio(null, talkList, 0f, 0.4f, 0.95f, 1.05f, 1f, this.transform);
        }
    }
}
