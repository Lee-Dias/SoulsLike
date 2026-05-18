using UnityEngine;
using DG.Tweening;

public class SoulFollowPlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float durationUp = 2f;
    [Header("Audio Settings")]
    [SerializeField] string targetTag;
    [SerializeField] float durationToPlayer = 2f;
    [SerializeField] float durationToPlayerOffset = 0.01f;
    Sequence seq;

    void Start()
    {
        seq = DOTween.Sequence();

        Vector3 finalPosition = transform.position + targetPosition;
        seq.Append(transform.DOMove(finalPosition, durationUp));  // first move
        
        
    }

    void Update()
    {
        if (seq.IsPlaying() == false)
        {
            Transform playerTransform = GameObject.FindWithTag(targetTag).transform;
            seq.Append(transform.DOMove((playerTransform.position + new Vector3(0, 1, 0)), durationToPlayer));  // second move
        }
        Collider playerCollider = GameObject.FindWithTag(targetTag).GetComponent<Collider>();

        if (playerCollider != null && playerCollider.bounds.Contains(transform.position))
        {
            Destroy(gameObject);
        }

        durationToPlayer = durationToPlayer - durationToPlayerOffset; 
        print(durationToPlayer);
    }
}
