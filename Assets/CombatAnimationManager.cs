using UnityEngine;
using UnityEngine.XR;

public class CombatAnimationManager
{
    private Animator animator;
    private CombatPlayableHandle handle;
    private CombatAnimations currentAnim;

    private bool queuedNext;
    private bool isPlaying;
    
    public bool IsPlaying => isPlaying;
    public CombatAnimations CurrentAnimation => currentAnim;
    public bool QueuedNext => queuedNext;

    public CombatPlayableHandle Handle => handle;


    public CombatAnimationManager(Animator anim)
    {
        animator = anim;
    }

    // -------------------------
    // PLAY
    // -------------------------
    public void Play(CombatAnimations animData)
    {
        Stop();

        if (animData == null) return;

        currentAnim = animData;
        handle = new CombatPlayableHandle(animator, animData);
        handle.Play(0);

        queuedNext = false;
        isPlaying = true;
    }

    // -------------------------
    // UPDATE
    // -------------------------
    public void UpdatePerFrame(float deltaTime)
    {
        if (handle == null)
            return;
        
        handle.Update(deltaTime);

        if (!isPlaying)
            return;

        if (handle.IsBlending)
            return; 
        if (handle.IsFadingOut)
            return; 
        if (handle.IsFadingIn)
            return; 



        float normalizedTime = handle.GetNormalizedTime();
        var steps = currentAnim.Steps;
        int step = handle.CurrentStep;

        // --- SINGLE animations ---
        if (!currentAnim.IsCombo)
        {
            if (normalizedTime >= 1f)
                Stop();
            return;
        }
        if (queuedNext && normalizedTime >= 1f)
        {
            queuedNext = false;

            int nextStep = step + 1;

            if (nextStep < steps.Length)
            {
                handle.BlendToStep(nextStep);
            }
            else
            {
                // At final animation → restart combo
                handle.BlendToStep(0);
                //handle.ResetTime(); 
            }
        }
    }

    public bool IsOnLastStep()
    {
        if (currentAnim == null || !currentAnim.IsCombo) return false;
        return handle.CurrentStep == currentAnim.Steps.Length - 1;
    }

    // -------------------------
    // COMBO QUEUE
    // -------------------------
    public void TryQueueNextStep()
    {
        if (currentAnim == null || !currentAnim.IsCombo || queuedNext)
            return;

        int step = handle.CurrentStep;
        var steps = currentAnim.Steps;

        float t = handle.GetNormalizedTime();
        float minWindow = Mathf.Max(steps[step].TransitionWindowStart, 0.25f); // force at least 25% played
        if (t >= minWindow && t <= steps[step].TransitionWindowEnd)
        {
            queuedNext = true;
        }
    }

    // -------------------------
    // GET MOVEMENT FROM CURVE
    // -------------------------
    public Vector2 GetMovementFromCurrentAnimation()
    {
        if (currentAnim == null) 
            return Vector2.zero;

        float t = handle.GetNormalizedTime();

        // Single animation
        if (!currentAnim.IsCombo)
        {
            return GetMovementFromCurve(currentAnim.MovementCurve, t);
        }

        // Combo animation
        var step = currentAnim.Steps[handle.CurrentStep];
        return GetMovementFromCurve(step.MovementCurveOnCombo, t);
    }

    private Vector2 GetMovementFromCurve(CombatAnimations.MovementInterval[] intervals, float t)
    {
        if (intervals == null)
            return Vector2.zero;

        foreach (var interval in intervals)
        {
            if (t >= interval.startTime && t <= interval.endTime)
                return interval.speed;
        }

        return Vector2.zero;
    }

    // -------------------------
    // STOP
    // -------------------------
    public void Stop()
    {
        if (handle != null)
            handle.FadeOutAndStop();

        currentAnim = null;
        queuedNext = false;
        isPlaying = false;
    }
}
