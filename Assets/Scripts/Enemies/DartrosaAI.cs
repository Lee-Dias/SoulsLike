using System.Collections;
using System.Collections.Generic;
using LibGameAI.FSMs;
using UnityEngine;
using UnityEngine.AI;

public class DratorsaAI : BaseEnemyAI
{

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject tornadoPrefab;
    [SerializeField] private AudioClip[] soundsToPlayOnAttackProjectile;
    [SerializeField] private AudioClip[] soundsToPlayOnAttackTornado;
    [SerializeField] private float Volume;
    [SerializeField] private float ChanceToSpawnTornado = 0.25f;
    [SerializeField] private float delayToInstantiateProjectile = 2f;
    [SerializeField] private float delayToInstantiateTornado = 2f;

    
    [SerializeField] private int tornadoCount = 3;
    [SerializeField] private float spawnRadius = 10f; 
    [SerializeField] private float spawnDistance = 3f;   
    [SerializeField] private float minDistance = 3f;    
    [SerializeField] private float tornadoSpeed = 3f;     

    [SerializeField] private float timeDownlimit = 5f;
    

    private float timeElapsed = 0;
    private float startHeight;

    private float timeDown; 

    private bool attacked;

    private bool wentDown = false;
    private bool isGettingUp = false; // <--- NOVA VARIÁVEL

    

    protected override void Start()
    {
        base.Start();
        timeDown = timeDownlimit;
        startHeight = agent.baseOffset;
    }
    protected override void Update()
    {
        HasNoShield = this.GetComponentInChildren<Shield>().IsShieldBroken();
        base.Update();
        

    }
    
    protected override void OnEnterDown()
    {
        anim.SetTrigger("Down");
        StopAllCoroutines();
        
        // Reseta as flags e o tempo para garantir uma queda limpa
        wentDown = false;
        isGettingUp = false;
        timeElapsed = 0;
    }

    protected override void Down()
    {
        // Se ainda está com tempo positivo e sem escudo
        if (timeDown > 0 && HasNoShield)
        {
            timeDown -= Time.deltaTime;

            if (timeElapsed < (timeDownlimit / 4) && !wentDown)
            {
                agent.baseOffset = Mathf.Lerp(startHeight, 0, timeElapsed / (timeDownlimit / 4));
                timeElapsed += Time.deltaTime; 
            }
            else
            {
                wentDown = true;
                agent.baseOffset = 0;
            }
        }
        else // O tempo acabou ou o escudo voltou
        {

            if (!isGettingUp)
            {
                anim.SetTrigger("Up");
                isGettingUp = true; // Impede que o trigger seja chamado no próximo frame
                timeElapsed = 0;    // Zera o tempo para começar o Lerp de subida
            }
            
            // 2. Continua rodando o Lerp por (timeDownlimit / 4) segundos
            if (timeElapsed < (timeDownlimit / 4))
            {
                agent.baseOffset = Mathf.Lerp(0, startHeight, timeElapsed / (timeDownlimit / 4));
                timeElapsed += Time.deltaTime; 
            }
            // 3. Terminou de levantar
            else
            {
                agent.baseOffset = startHeight;
                timeDown = timeDownlimit;
                timeElapsed = 0; 
                
                wentDown = false;
                isGettingUp = false; // Reseta para a próxima vez que o escudo quebrar
                
                // Restaura o escudo (isto também tira o boss do downState através da FSM)
                HasNoShield = false;
                
                GetComponent<Shield>().RestoreShield();
                GetComponent<BarrierDissolve>().ChangeBarrierValues();
            }
        }
    }
    
    
    protected override void OnEnterAttack()
    {
        attacked = false;
        canAttack = false;
        float rnd = Random.value;
        if (rnd <= ChanceToSpawnTornado)
        {
            StartCoroutine(SpawnTornado());
        }
        else
        {
            StartCoroutine(SpawnProjectile());
        }
        timesCircledSinceLastAttack = 0;
        
    }

    protected override void Attack()
    {
        
        
    }

    private IEnumerator SpawnProjectile()
    {
        yield return new WaitForSeconds(delayToInstantiateProjectile);
        if(attacked) yield break; 
        anim.SetTrigger("Attack");
        Vector3 tr = this.transform.position;
        tr.y += 2f; 
        audioManager.PlayAudio(null,soundsToPlayOnAttackProjectile,0,Volume);
        GameObject projectile = Instantiate(projectilePrefab, tr, this.transform.rotation);
        projectile.GetComponent<OrbProjectile>().SetTarget(player.transform);
        attackEnded = true;
        attacked = true;
    }
    private IEnumerator SpawnTornado()
    {
        yield return new WaitForSeconds(delayToInstantiateTornado);
        if(attacked) yield break; 

        anim.SetTrigger("Attack");

        Vector3 tr = transform.position;
        tr.y -= 6f;

        audioManager.PlayAudio(null, soundsToPlayOnAttackTornado, 0, Volume);


        List<Vector3> chosenPositions = new List<Vector3>();

        for (int i = 0; i < tornadoCount; i++)
        {
            Vector3 pos;
            int attempts = 0;

            do
            {
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                pos = player.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

                attempts++;
                if (attempts > 20) break;
            }
            while (!IsFarEnough(pos, chosenPositions, minDistance));

            chosenPositions.Add(pos);

            Vector3 dir = pos - tr;
            dir.y = -1f;       
            dir = dir.normalized;

            Vector3 spawnPos = tr + dir * spawnDistance;
            GameObject tornado = Instantiate(tornadoPrefab, spawnPos, transform.rotation);

            tornado.GetComponent<Tornado>().SetDirectionSpeed(dir, tornadoSpeed);
        }

        attackEnded = true;
        attacked = true;
    }
    private bool IsFarEnough(Vector3 newPos, List<Vector3> existingPositions, float minDist)
    {
        foreach (var pos in existingPositions)
        {
            if (Vector3.Distance(newPos, pos) < minDist)
                return false;
        }
        return true;
    }
    protected override void OnEnterFirstAttack()
    {
    }

    protected override void FirstAttack()
    {
        
    }

}
