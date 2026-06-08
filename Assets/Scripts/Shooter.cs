using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform target;
    [SerializeField] private float timeBetweenShots = 5f;
    [SerializeField] private MeshRenderer form;
    [SerializeField] private MeshRenderer dissolve;
    [SerializeField] private GameObject selfTarget;
    [SerializeField] private OpenDoor door;
    private float shotTimer = 0f;
    private float lifes = 2;



    private Color baseColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (form != null && form.material.HasProperty("_ShieldColor"))
        {
            baseColor = form.material.GetColor("_ShieldColor");
        }
        ChangeBarrier();

    }

    // Update is called once per frame
    void Update()
    {
        shotTimer += Time.deltaTime;
        if (shotTimer >= timeBetweenShots)
        {
            shotTimer = 0f;
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        projectile.GetComponent<OrbProjectile>().SetTarget(target, false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Orb"))
        {
            if(other.GetComponent<OrbProjectile>().GetTarget() != selfTarget) return;
            Destroy(other.gameObject);
            lifes--;
            ChangeBarrier();
        }
    }
    private void ChangeBarrier()
    {
        Color targetColor = baseColor;
        float targetErosion = 0f;

        if(lifes == 2)
        {
            targetColor = baseColor * 1f;
            targetErosion = 0.6f;
        }
        else if(lifes == 1)
        {
            targetColor = baseColor * 0.2f;
            targetErosion = 0.8f;
        }
        else if(lifes <= 0)
        {
            targetColor = baseColor * 0.01f;
            targetErosion = 1.2f;
            StartCoroutine(TransitionBarrier(targetColor, targetErosion, 0.5f, true));
            door.Open();
            
        }

        StartCoroutine(TransitionBarrier(targetColor, targetErosion, 0.5f));
    }

    private IEnumerator TransitionBarrier(Color targetColor, float targetErosion, float duration, bool destroyAfter = false)
    {
        // Ponto de partida atual (onde o material está NESTE exato momento)
        Color startColor = form.material.GetColor("_ShieldColor");
        float startErosion = dissolve.material.GetFloat("_Erosion");
        
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // Vai de 0 a 1 ao longo de 0.3s

            // Interpolação suave entre o valor antigo e o novo
            Color newColor = Color.Lerp(startColor, targetColor, t);
            float newErosion = Mathf.Lerp(startErosion, targetErosion, t);

            // Aplica os valores intermédios
            form.material.SetColor("_ShieldColor", newColor);
            dissolve.material.SetFloat("_Erosion", newErosion);

            yield return null; // Espera pelo próximo frame
        }

        

        // Garante que no final os valores ficam exatamente os pretendidos
        form.material.SetColor("_ShieldColor", targetColor);
        dissolve.material.SetFloat("_Erosion", targetErosion);
        if(destroyAfter)
        {
            Destroy(this);
            Destroy(selfTarget);
        }
    }
}
