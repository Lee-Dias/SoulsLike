using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]private Transform transformToFollow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = transformToFollow.position;
        this.transform.rotation = transformToFollow.rotation;
    }
}
