using UnityEngine;
using UnityEngine.Events;

public class CheckRadius : MonoBehaviour
{
    [SerializeField] private LayerMask layerToDetect;  

    public UnityEvent onTargetEnter;

  

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layerToDetect) != 0)
        {
            onTargetEnter?.Invoke();
            Destroy(gameObject);
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
