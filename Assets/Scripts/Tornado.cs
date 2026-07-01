using UnityEngine;

public class Tornado : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float waitTime = 1f;
    private float time = 0f;

    public void SetDirectionSpeed(Vector3 dir, float s)
    {
        direction = dir.normalized;
        speed = s;
        Destroy(gameObject, 10f); 
    }   
    private void Update()
    {
        time += Time.deltaTime;
        if (time < waitTime)
        {
            return; 
        }
        transform.position += direction * speed * Time.deltaTime;
    }
}