using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]private Transform transformToFollow;
    [SerializeField]private Vector3 position;
    [SerializeField]private Vector3 rotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = transformToFollow.position + position;
        this.transform.rotation = transformToFollow.rotation * Quaternion.Euler(rotation);
    }
}
