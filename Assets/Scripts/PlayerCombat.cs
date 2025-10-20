using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float comboResetTime = 1.2f;
    [SerializeField] private float inputBufferTime = 0.5f; // How long input buffer lasts
    [SerializeField] private float comboTriggerPoint = 0.7f; // When next attack can trigger (normalizedTime)

    private Animator anim;
    private int comboStep = 0;
    private bool canQueueNext = false;
    private bool attackQueued = false;
    private float lastAttackTime;
    private float bufferTimer = 0f;

    public bool isattacking;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        // Reset combo if too long
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
            attackQueued = false;
            canQueueNext = false;

            isattacking = false;
        }

        // Open buffer window mid-animation
        if (state.normalizedTime >= comboTriggerPoint && comboStep > 0)
            canQueueNext = true;

        // If buffer timer expires, clear queued attack
        if (attackQueued)
        {
            bufferTimer -= Time.deltaTime;
            if (bufferTimer <= 0f)
                attackQueued = false;
        }

        // Trigger next attack when animation ends (or nearly ends)
        if (canQueueNext && attackQueued && state.normalizedTime >= 0.6f)
        {
            attackQueued = false;
            canQueueNext = false;
            PlayNextAttack();
        }
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        lastAttackTime = Time.time;
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        // If idle or first attack
        if (comboStep == 0 || state.IsName("Walk") || state.IsName("Walk locked"))
        {
            isattacking = true;
            comboStep = 1;
            PlayAttack("hit1");
            return;
        }

        // If already attacking, queue next if allowed
        if (!attackQueued && comboStep < 3)
        {
            attackQueued = true;
            bufferTimer = inputBufferTime; // keep buffer alive briefly
        }
    }

    private void PlayNextAttack()
    {
        comboStep++;
        if (comboStep > 3)
        {
            comboStep = 0;
            isattacking = true;
            return;
        }

        PlayAttack($"hit{comboStep}");
    }

    private void PlayAttack(string trigger)
    {
        anim.ResetTrigger("hit1");
        anim.ResetTrigger("hit2");
        anim.ResetTrigger("hit3");
        anim.SetTrigger(trigger);

        // Reset timing flags for new animation
        canQueueNext = false;
        attackQueued = false;
    }
}
