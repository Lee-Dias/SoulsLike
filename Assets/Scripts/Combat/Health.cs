using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float cooldownPerGetHit = 1f;
    private Animator animator;
    private float timePassedSinceLastHit;

    void Start()
    {
        animator = GetComponent<Animator>();
        timePassedSinceLastHit = cooldownPerGetHit;
    }
    private void FixedUpdate()
    {
        timePassedSinceLastHit+= Time.deltaTime;
    }

    public void GetHit()
    {
        if (cooldownPerGetHit <= timePassedSinceLastHit )
        {
            animator.SetTrigger("gethit");
            timePassedSinceLastHit = 0;
        }

    }
}
