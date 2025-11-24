using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Runtime.InteropServices;

public class CombatPlayableHandle
{
    private PlayableGraph graph;
    private AnimationPlayableOutput output;

    private AnimationClipPlayable singlePlayable;
    private AnimationMixerPlayable comboMixer;

    private CombatAnimations data;
    private bool isCombo => data != null && data.IsCombo;

    private int currentStep = 0;
    private bool isPlaying = false;

    public int CurrentStep => currentStep;
    public bool IsValid => graph.IsValid();

    // -----------------------------
    // BLENDING DATA
    // -----------------------------
    private bool blendActive = false;
    private int blendFromIndex = -1;
    private int blendToIndex = -1;
    private float blendTimer = 0f;
    private float blendDuration = 1f;

    // For animator-quality: eased blending
    private float Ease(float t) => t * t * (3 - 2 * t);

    // -----------------------------
    // FADE OUT DATA
    // -----------------------------
    private bool fadeOutActive = false;
    private float fadeOutTimer = 0f;
    private float fadeOutDuration = 0.50f;

    // START BLEND DATA
    private bool startBlendActive = false;
    private float startBlendTimer = 0f;
    private float startBlendDuration = 0.15f; // can be per-animation if needed

    private bool activateHitBox; // can be per-animation if needed

    public bool IsBlending => blendActive;
    public bool IsFadingOut => fadeOutActive;
    public bool IsFadingIn => startBlendActive;
    public bool ActivateHitBox => activateHitBox;

    public CombatPlayableHandle(Animator animator, CombatAnimations animData)
    {
        data = animData;

        graph = PlayableGraph.Create(animator.name + "_CombatGraph");
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        output = AnimationPlayableOutput.Create(graph, "CombatOutput", animator);

        if (isCombo)
            CreateComboMixer(animData);
        else
            CreateSinglePlayable(animData);

        graph.Play();
        graph.Stop();
    }

    private void CreateSinglePlayable(CombatAnimations animData)
    {
        singlePlayable = AnimationClipPlayable.Create(graph, animData.SingleClip);
        singlePlayable.SetApplyFootIK(true);
        singlePlayable.SetApplyPlayableIK(true);
        singlePlayable.SetDuration(animData.SingleClip.length);

        output.SetSourcePlayable(singlePlayable);
    }

    private void CreateComboMixer(CombatAnimations animData)
    {
        var steps = animData.Steps;
        comboMixer = AnimationMixerPlayable.Create(graph, steps.Length);

        for (int i = 0; i < steps.Length; i++)
        {
            var clipPlayable = AnimationClipPlayable.Create(graph, steps[i].Clip);
            clipPlayable.SetApplyFootIK(true);
            clipPlayable.SetApplyPlayableIK(true);
            comboMixer.ConnectInput(i, clipPlayable, 0);
            comboMixer.SetInputWeight(i, 0f);
        }

        output.SetSourcePlayable(comboMixer);
    }

    // -----------------------------
    // PLAY
    // -----------------------------
    public void Play(int stepIndex = 0)
    {
        if (!IsValid) return;

        isPlaying = true;
        currentStep = Mathf.Clamp(stepIndex, 0, isCombo ? data.Steps.Length - 1 : 0);

        if (isCombo)
        {
            for (int i = 0; i < data.Steps.Length; i++)
                comboMixer.SetInputWeight(i, 0f); // start at 0

            blendFromIndex = -1; // blending from animator
            blendToIndex = currentStep;
            startBlendTimer = 0f;
            startBlendActive = true;

            var playable = (AnimationClipPlayable)comboMixer.GetInput(currentStep);
            playable.SetTime(0);
            playable.SetSpeed(data.Steps[currentStep].ComboSpeed);
            playable.Play();
        }
        else
        {
            singlePlayable.SetTime(0);
            singlePlayable.SetSpeed(data.Speed);
            singlePlayable.Play();
            startBlendTimer = 0f;
            startBlendActive = true;
        }

        graph.Play();
    }



    // -----------------------------
    // FADE OUT WITH BLENDING
    // -----------------------------
    public void FadeOutAndStop()
    {
        fadeOutActive = true;
        fadeOutTimer = 0f;
        
    }

    // -----------------------------
    // UPDATE (Blend + FadeOut)
    // -----------------------------
    public void Update(float deltaTime)
    {
        if (!isPlaying) return;

        graph.Evaluate(deltaTime);

        if (data.IsAttackAnimation&& !fadeOutActive && !blendActive && !startBlendActive)
        {
            if (data.IsCombo )
            {
                if(data.Steps[currentStep].HitStartTime < GetNormalizedTime() && data.Steps[currentStep].HitEndTime > GetNormalizedTime())
                {
                    activateHitBox = true;
                }
                else
                {
                    activateHitBox = false;
                }
            }
        }
        else
        {
            activateHitBox = false; 
        }
        // -----------------------
        // BLENDING BETWEEN STEPS
        // -----------------------
        if (blendActive)
        {
            blendTimer += deltaTime;
            float t = Mathf.Clamp01(blendTimer / blendDuration);
            float easedT = Ease(t);

            float fromW = 1f - easedT;
            float toW = easedT;

            comboMixer.SetInputWeight(blendFromIndex, fromW);
            comboMixer.SetInputWeight(blendToIndex, toW);

;
            var toPlayable = (AnimationClipPlayable)comboMixer.GetInput(blendToIndex);

            toPlayable.SetTime(0);

            if (t >= 1f)
            {
                blendActive = false;
                comboMixer.SetInputWeight(blendFromIndex, 0f);
                comboMixer.SetInputWeight(blendToIndex, 1f);
                toPlayable.Play();

            }
        }

        // -----------------------
        // FADE OUT SYSTEM
        // -----------------------
        if (fadeOutActive)
        {
            fadeOutTimer += deltaTime;
            float t = Mathf.Clamp01(fadeOutTimer / fadeOutDuration);
            float w = 1f - Ease(t);

            if (isCombo)
            {
                for (int i = 0; i < data.Steps.Length; i++)
                    comboMixer.SetInputWeight(i, (i == currentStep) ? w : 0f);
            }
            else
            {
                singlePlayable.SetSpeed(w);
            }
                output.SetWeight(w);  
            if (t >= 1f)
            {
                output.SetWeight(w);  
                fadeOutActive = false;
                isPlaying = false;
                // DO NOT immediately reset animation root
                graph.Stop();
                
            }
        }

        if (startBlendActive)
        {
            startBlendTimer += deltaTime;
            float t = Mathf.Clamp01(startBlendTimer / startBlendDuration);
            float easedT = Ease(t);

            if (isCombo)
            {
                comboMixer.SetInputWeight(blendToIndex, easedT);
            }
            else
            {
                output.SetWeight(easedT);
            }

            if (t >= 1f)
            {
                startBlendActive = false;
                if (!isCombo)
                    output.SetWeight(1f);
                else
                    comboMixer.SetInputWeight(blendToIndex, 1f);
            }
        }
    }

    // -----------------------------
    // NORMALIZED TIME
    // -----------------------------
    public float GetNormalizedTime()
    {
        if (!isPlaying) return 0f;

        if (isCombo)
        {
            AnimationClipPlayable playable;

            if (blendActive)
                return 0f;
                //playable = (AnimationClipPlayable)comboMixer.GetInput(blendFromIndex);
            else
                playable = (AnimationClipPlayable)comboMixer.GetInput(currentStep);

            double length = playable.GetAnimationClip().length;
            return (float)(playable.GetTime() / length);
        }
        else
        {
            var clip = singlePlayable.GetAnimationClip();
            return (float)(singlePlayable.GetTime() / clip.length);
        }
    }


    // -----------------------------
    // BLEND TO NEXT STEP (Animator-style)
    // -----------------------------
    public void BlendToStep(int nextIndex)
    {
        if (!isCombo || nextIndex >= data.Steps.Length) return;

        Debug.Log("blend called");
        blendFromIndex = currentStep;
        blendToIndex = nextIndex;
        currentStep = nextIndex;

        blendTimer = 0f;
        blendDuration = data.Steps[blendFromIndex].BlendTime;;
        blendActive = true;

        var nextPlayable = (AnimationClipPlayable)comboMixer.GetInput(nextIndex);
        nextPlayable.Pause();
        nextPlayable.SetSpeed(data.Steps[nextIndex].ComboSpeed);
        nextPlayable.SetTime(0);
    }

    public void ResetTime()
    {
        if (isCombo)
        {
            var playable = (AnimationClipPlayable)comboMixer.GetInput(currentStep);
            playable.SetTime(0);
        }
        else
        {
            singlePlayable.SetTime(0);
        }
    }

    public void Destroy()
    {
        if (graph.IsValid())
            graph.Destroy();
    }
}
