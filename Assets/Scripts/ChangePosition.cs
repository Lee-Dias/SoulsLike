using UnityEngine;

public class ChangePosition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdatePostion()
    {
        this.transform.position = new Vector3(0f ,-1f , 1.57f);
    }
}
