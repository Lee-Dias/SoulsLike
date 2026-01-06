using UnityEngine;
using System;

public class SpawnedLifetimeTracker : MonoBehaviour
{
    public Action OnDestroyed;

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
