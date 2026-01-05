using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [Header("Portal")]
    [SerializeField] private GameObject portalVfx;
    [SerializeField] private GameObject objectToSpawn;

    [Header("Spawn Area")]
    [SerializeField] private bool spawnInArea;
    [SerializeField, ShowIf(nameof(spawnInArea))]
    private float spawningArea = 5f;

    [Header("Timing")]
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 3f;

    [Header("Portal Movement")]
    [SerializeField] private float portalDepth = 2f;
    [SerializeField] private float travelDuration = 4f; // seconds to fully pass portal


    [Header("Post Exit Movement")]
    [SerializeField] private bool continueForward = true;
    [SerializeField, ShowIf(nameof(continueForward))]
    private float postExitDistance = 2f;

    [Header("VFX & Lifetime")]
    [SerializeField] private float portalLifetime = 5f;

    [Header("Object Lifetime")]
    [SerializeField] private bool destroyObject = false;
    [SerializeField, ShowIf(nameof(destroyObject))]
    private float objectLifetime = 5f;

    private float nextSpawnTime = 0f;
    

    private void Start()
    {
       
    }

    private void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            Spawn();
            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
    }

    private void Spawn()
    {
        float prefabY = objectToSpawn.transform.position.y;

        Vector3 exitPoint = GetSpawnPoint();
        Vector3 spawnPoint = exitPoint - transform.forward * portalDepth;
        spawnPoint.y = prefabY;


        // Spawn portal VFX
        GameObject portalInstance = null;
        if (portalVfx)
        {
            portalInstance = Instantiate(portalVfx, exitPoint, transform.rotation);
            Destroy(portalInstance, portalLifetime);
        }

        // Spawn object
        GameObject spawned = Instantiate(objectToSpawn, spawnPoint, transform.rotation);

        PortalClip clip = spawned.AddComponent<PortalClip>();
        clip.portalTransform = transform; // the portal transform

        // Compute final target after portal
        Vector3 finalTarget = exitPoint;
        if (continueForward)
            finalTarget += transform.forward * postExitDistance;
        
        finalTarget.y = prefabY;

        // Move object through portal
        StartCoroutine(MoveThroughPortal(spawned.transform, finalTarget));

        // Optionally destroy object after separate lifetime
        if (destroyObject)
            Destroy(spawned, objectLifetime);

        if(spawned.GetComponent<BaseEnemyAI>() != null)
        {
            spawned.GetComponent<BaseEnemyAI>().CheckIfSpawned(travelDuration);
        }
        Destroy(spawned.GetComponent<PortalClip>(), travelDuration);
    }

    private Vector3 GetSpawnPoint()
    {
        if (!spawnInArea)
            return transform.position;

        Vector2 randomCircle = Random.insideUnitCircle * spawningArea;
        return transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
    }

    private IEnumerator MoveThroughPortal(Transform obj, Vector3 target)
    {
        Vector3 startPos = obj.position;

        // Preserve prefab Y (the real rule)
        float fixedY = startPos.y;

        float elapsed = Time.deltaTime; // avoid zero-frame stall

        while (elapsed < travelDuration)
        {
            float t = elapsed / travelDuration;

            Vector3 pos = Vector3.Lerp(startPos, target, t);
            pos.y = fixedY;

            obj.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.y = fixedY;
        obj.position = target;
    }




#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnInArea)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, spawningArea);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            transform.position,
            transform.position - transform.forward * portalDepth
        );
    }
#endif
}
