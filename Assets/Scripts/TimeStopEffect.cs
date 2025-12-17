using UnityEngine;

public class TimeStopEffect : MonoBehaviour
{
    private float timer;
    private bool active;

    public void Activate(float duration)
    {
        timer = duration;
        active = true;

        // TODO: enable post-processing, vignette, chromatic aberration, shader, etc.
    }

    private void Update()
    {
        if (!active) return;

        timer -= Time.unscaledDeltaTime;

        if (timer <= 0)
        {
            active = false;

            // TODO: disable effects
        }
    }
}
