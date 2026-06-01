using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AmbientSound : MonoBehaviour
{
    [SerializeField] private Collider area;
    [SerializeField] private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 closestPoint = area.ClosestPoint(player.transform.position);
        transform.position = closestPoint;
    }
}
