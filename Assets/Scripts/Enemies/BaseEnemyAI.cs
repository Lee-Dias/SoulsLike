using LibGameAI.FSMs;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemyAI : MonoBehaviour, IEnemyTimeAffectable
{
    [Header("Ranges")]
    [SerializeField] protected float minPreferredDistanceFromPlayer;
    [SerializeField] protected float maxPreferredDistanceFromPlayer;
    [SerializeField] protected float audioRange;
    [SerializeField] protected float viewRange;
    [SerializeField] protected float viewAngle;

    [Header("Movement")]
    [SerializeField] protected float circleEnemySpeed;

    [Header("Timing & Stats")]
    [SerializeField] protected float minTimeCircling;
    [SerializeField] protected float maxTimeCircling;
    [SerializeField] protected float attacksCooldown;
    [SerializeField] protected EnemyProfile enemy;

    [Header("Dodge Settings")]
    [SerializeField] protected bool canDodge;
    [SerializeField] protected float dodgeDistance = 5f;
    [SerializeField] protected float dodgeDuration = 0.1f;
    [SerializeField] protected float chanceToDodge = 25f;
    [SerializeField] protected float dodgeCoolDown = 3f;

    [Header("Perception Layers")]
    [SerializeField] protected LayerMask occlusionLayers;

    private float dodgeCoolDownTimer;
    private float totalCircleTime;
    private int dodgeChecksDone = 0; // Para garantir que só checa 3 vezes

    [Header("Decision Chance")]
    [SerializeField] protected float chanceToCircle = 0.5f; // 50/50 default

    [Header("Important")]
    protected Animator anim;
    [SerializeField] protected BoxCollider weaponCollider;
    [SerializeField] protected Item item;
    [SerializeField] protected GameObject canvas;
    [SerializeField] protected int auraValue = 100;
    [Header("Attack Settings")]
    [SerializeField] protected bool hasPredifinedFirstAttack = false;
    [SerializeField] protected float firstAttackDistanceToActivate = 0f;
    [SerializeField] protected float firstAttackViewDistanceToActivate = 0f;

    
    private float spawnTimer;
    private bool isSpawnDelayed = false;

    public bool HasNoShield = false;

    protected NavMeshAgent agent;
    protected StateMachine stateMachine;
    protected Health health;

    protected State idleState;
    protected State decideState;
    protected State chaseState;
    protected State circleState;
    protected State attackState;
    protected State firstAttackState;
    protected State downState;

    protected Transform player;

    protected float baseSpeed;
    protected float currentHealth;
    protected bool playerInViewRange;
    protected bool playerInAudioRange;

    protected bool isDead;

    protected float circleTimer;
    protected float circleDirection = 1f;

    protected bool attackEnded = false;
    protected bool canAttack = false;
    protected bool isInAttackAnimation = false;

    protected float timeScale = 1f;

    protected CombatAnimationManager animManager;
    protected AudioManager audioManager;

    protected bool doneFirstAttack = true;
    protected float originalViewRange;
    protected float originalAudioRange;

    private float lastDecisionRoll;

    protected int timesCircledSinceLastAttack = 0;

    public bool IsInAttackAnimation => isInAttackAnimation;
    public int AuraValue => auraValue;

    private Vector3 enemyTargetPosition;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();
        audioManager = FindFirstObjectByType<AudioManager>();
        animManager = new CombatAnimationManager(anim);
        if(hasPredifinedFirstAttack)
        {
            doneFirstAttack = false;
            originalViewRange = viewRange;
            originalAudioRange = audioRange;
            viewRange = firstAttackViewDistanceToActivate;
            audioRange = firstAttackDistanceToActivate;
        }


    }
    public virtual void ResetEnemy()
    {
        animManager.Stop();
    }
    public float DamageToDeal()
    {
        return item.Damage + (enemy.BaseDexterity / 5 ) + (enemy.BaseStrength / 10);
    }

    public void CheckIfSpawned(float delay)
    {
        spawnTimer = delay;
        isSpawnDelayed = true;
        agent.isStopped = true; // freeze movement
    }
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animManager = new CombatAnimationManager(anim);
        baseSpeed = agent.speed;

        CreateStates();
        CreateTransitions();

        stateMachine = new StateMachine(idleState);
        idleState.EntryActions?.Invoke();

    }


    protected virtual void Update()
    {
        if (isSpawnDelayed)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                isSpawnDelayed = false;
                canvas.SetActive(true);
                agent.isStopped = false; // allow movement again
            }
            else
            {
                Vector3 localVel = transform.InverseTransformDirection(agent.velocity);
                anim.SetFloat("x", 0);
                anim.SetFloat("y", 1);
                anim.SetBool("IsIdle", false);
                return; 
            }
        }

        // === rest of your existing Update() ===
        if (!isInAttackAnimation)
        {
            Vector3 localVel = transform.InverseTransformDirection(agent.velocity);
            
            anim.SetFloat("x", localVel.x);
            anim.SetFloat("y", localVel.z);

        }
        else
        {
            anim.SetFloat("x", 0);
            anim.SetFloat("y", 0);
            if (!IsTouchingPlayer())
            {
                Vector2 mov = animManager.GetMovementFromCurrentAnimation();
                transform.position += (transform.forward * mov.y + transform.right * mov.x) * (Time.deltaTime * timeScale);
            }
        }


        animManager?.UpdatePerFrame(Time.deltaTime * timeScale);
        UpdatePerception();
        var actions = stateMachine.Update();
        actions?.Invoke();
    }


    public void SetTimeScale(float scale)   
    {
        timeScale = scale;
        agent.speed = baseSpeed * scale;
        anim.speed = scale; 
    }

    // ------------------------------------------------------
    //  STATE DEFINITIONS
    // ------------------------------------------------------
    protected virtual void CreateStates()
    {
        idleState = new State("Idle", OnEnterIdle, Idle, null);
        decideState = new State("Decide", OnEnterDecide, Decide, null);
        chaseState = new State("Chase", OnEnterChase, Chase, null);
        circleState = new State("Circle", OnEnterCircle, Circle, null);
        attackState = new State("Attack", OnEnterAttack, Attack, null);
        firstAttackState = new State("FirstAttack", OnEnterFirstAttack, FirstAttack, null);        
        downState = new State("down", OnEnterDown, Down, null);
    }

    // ------------------------------------------------------
    //  FSM TRANSITIONS
    // ------------------------------------------------------
    protected virtual void CreateTransitions()
    {
        idleState.AddTransition(new Transition(() => (playerInViewRange || playerInAudioRange) && doneFirstAttack, null, decideState));

        idleState.AddTransition(new Transition(() => !doneFirstAttack && (playerInViewRange || playerInAudioRange), null, firstAttackState));

        firstAttackState.AddTransition(new Transition(() => doneFirstAttack, null, decideState));

        decideState.AddTransition(new Transition(() => ShouldGoIdle() && !HasNoShield, null, idleState));
        decideState.AddTransition(new Transition(() => ShouldChase() && !HasNoShield, null, chaseState));
        decideState.AddTransition(new Transition(() => ShouldCircle() && !HasNoShield , null, circleState));
        decideState.AddTransition(new Transition(() => ShouldAttack() && health.CanAttack() && !HasNoShield, null, attackState));

        idleState.AddTransition(new Transition(() => HasNoShield, null, downState));
        chaseState.AddTransition(new Transition(() => HasNoShield, null, downState));
        circleState.AddTransition(new Transition(() => HasNoShield, null, downState));
        attackState.AddTransition(new Transition(() => HasNoShield, null, downState));
        firstAttackState.AddTransition(new Transition(() => HasNoShield, null, downState));

        downState.AddTransition(new Transition(() => !HasNoShield, null, decideState));

        chaseState.AddTransition(new Transition(() => IsInPreferredRange(), null, decideState));

        circleState.AddTransition(new Transition(() => circleTimer <= 0, null, decideState));

        attackState.AddTransition(new Transition(() => attackEnded, null, decideState));
    }

    // ------------------------------------------------------
    //  DECISION LOGIC
    // ------------------------------------------------------
    private bool ShouldGoIdle()
    {
        return !playerInAudioRange && !playerInViewRange;
    }

    private bool ShouldChase()
    {
        return !IsInPreferredRange() && (playerInAudioRange || playerInViewRange);
    }

    private bool ShouldCircle()
    {
        if (!IsInPreferredRange()) return false;
        if(!canAttack) return true;
        return lastDecisionRoll < chanceToCircle;
    }

    private bool ShouldAttack()
    {
        if (!IsInPreferredRange() || canAttack == false) return false;
        if(timesCircledSinceLastAttack > 1 && IsInPreferredRange()) return true; // Garante que o inimigo ataca depois de 2 círculos, mesmo que o RNG não colabore
        return lastDecisionRoll >= chanceToCircle;
    }

    // ------------------------------------------------------
    //  STATE BEHAVIOR
    // ------------------------------------------------------

    protected virtual void OnEnterIdle()
    {
        agent.isStopped = true;
        anim.SetBool("IsIdle", true);
        canAttack = true;
        
    }

    protected virtual void Idle() { }

    protected abstract void OnEnterFirstAttack();
    protected abstract void FirstAttack();

    protected abstract void OnEnterDown();
    protected abstract void Down();

    protected virtual void OnEnterDecide()
    {
        agent.isStopped = false;
        agent.updateRotation = false;
        attackEnded = false;
        lastDecisionRoll = Random.value;
        circleTimer = Random.Range(minTimeCircling, maxTimeCircling);
        Debug.Log("Deciding...");
    }

    protected virtual void Decide()
    {
        
        
    }

    protected virtual void OnEnterChase()
    {
        anim.SetBool("IsIdle", false);
        agent.isStopped = false;
        agent.updateRotation = true;
        agent.speed = baseSpeed;
        canAttack = true;
        Debug.Log("Chasing player");
        // 1. Descobre a direção: do jogador apontando para o inimigo
        Vector3 directionFromPlayer = (transform.position - player.position).normalized;

        // 2. Escolhe a distância ideal (o meio termo entre o mínimo e o máximo)
        float targetDistance = (minPreferredDistanceFromPlayer + maxPreferredDistanceFromPlayer) / 2f;

        // 3. Calcula o ponto exato no mundo que fica nessa distância do jogador
        enemyTargetPosition = player.position + (directionFromPlayer * targetDistance);
    }

    protected virtual void Chase()
    {
        if (player == null) return;

        timesCircledSinceLastAttack += 1;   
        
        agent.SetDestination(enemyTargetPosition);
        if(this.transform.position.x - enemyTargetPosition.x < 0.5f || this.transform.position.x - enemyTargetPosition.x > 0.5f)
        {
            Vector3 directionFromPlayer = (transform.position - player.position).normalized;

            // 2. Escolhe a distância ideal (o meio termo entre o mínimo e o máximo)
            float targetDistance = (minPreferredDistanceFromPlayer + maxPreferredDistanceFromPlayer) / 2f;

            // 3. Calcula o ponto exato no mundo que fica nessa distância do jogador
            enemyTargetPosition = player.position + (directionFromPlayer * targetDistance);
        }
    }

    protected virtual void OnEnterCircle()
    {
        anim.SetBool("IsIdle", false);
        Debug.Log("Circling player");
        agent.isStopped = false;
        agent.updateRotation = false;
        agent.speed = circleEnemySpeed;
        canAttack = true;
        dodgeCoolDownTimer = dodgeCoolDown;
        circleTimer = Random.Range(minTimeCircling, maxTimeCircling);
        totalCircleTime = circleTimer; 
        dodgeChecksDone = 0;   
        circleDirection = Random.value > 0.5f ? 1f : -1f;
    }

    protected virtual void Circle()
    {
        if (player == null || health.ShouldBlockMovement()) 
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        RotateTowardPlayer();


        // --- Lógica de Movimento de Círculo ---
        Vector3 toPlayer = (player.position - transform.position).normalized;
        Vector3 strafe = Vector3.Cross(Vector3.up, toPlayer) * circleDirection;
        Vector3 target = transform.position + strafe * 2f;
        
        // Ajuste de distância preferida
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < minPreferredDistanceFromPlayer) target -= toPlayer;
        else if (distance > maxPreferredDistanceFromPlayer) target += toPlayer;

        agent.SetDestination(target);

        // --- Lógica dos 3 Checkpoints de Dodge ---
        float progress = 1f - (circleTimer / totalCircleTime); // 0 a 1
        if (canDodge)
        {
            if (dodgeChecksDone == 0 && progress >= 0.25f) CheckForDodge();
            if (dodgeChecksDone == 1 && progress >= 0.50f) CheckForDodge();
            if (dodgeChecksDone == 2 && progress >= 0.75f) CheckForDodge();
        }


        circleTimer -= Time.deltaTime * timeScale;
        dodgeCoolDownTimer += Time.deltaTime * timeScale;
    }

    private void CheckForDodge()
    {
        dodgeChecksDone++;
        if (Random.value < (chanceToDodge / 100f) && dodgeCoolDownTimer >= dodgeCoolDown)
        {
            Dodge();
            dodgeCoolDownTimer = 0f; // reset cooldown after dodging
        }
    }

    private void Dodge()
    {
        // Toca a animação
        anim.SetTrigger("Dodge");
        
        // Calcula a direção lateral baseada no círculo
        Vector3 dodgeDirection = transform.right * circleDirection;

        // Inicia o movimento suave em vez de um TP instantâneo
        StartCoroutine(SmoothDodge(dodgeDirection));

        Debug.Log("Inimigo realizou um dodge fluido!");
    }

    private System.Collections.IEnumerator SmoothDodge(Vector3 direction)
    {
        
        float elapsed = 0f;
        
        // Opcional: Podes desativar a atualização do NavMesh temporariamente para não haver conflito
        // agent.updatePosition = false; 

        while (elapsed < dodgeDuration)
        {
            // Calcula quanto mover este frame
            float speed = dodgeDistance / dodgeDuration;
            agent.Move(direction * speed * Time.deltaTime * timeScale);
            
            elapsed += Time.deltaTime * timeScale;
            yield return null; // Espera pelo próximo frame
        }

        // agent.updatePosition = true;
    }

    protected abstract void OnEnterAttack();
    protected abstract void Attack();

    // ------------------------------------------------------
    //  HELPERS
    // ------------------------------------------------------
    protected virtual void UpdatePerception()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        playerInAudioRange = dist <= audioRange && HasLineOfSight();
        
        playerInViewRange = IsPlayerInViewRange();
    }
    protected bool IsTouchingPlayer()
    {
        if (player == null) return false;

        float radius = 1.0f; // tweak based on enemy size
        return Physics.CheckSphere(transform.position, radius, LayerMask.GetMask("Player"));
    }

    protected bool IsInPreferredRange()
    {
        float d = Vector3.Distance(transform.position, player.position);
        return d >= minPreferredDistanceFromPlayer && d <= maxPreferredDistanceFromPlayer;
    }

    protected bool IsPlayerInViewRange()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        float distance = Vector3.Distance(transform.position, player.position);

        // Verifica: Ângulo -> Distância -> Paredes
        return angle <= viewAngle / 2f && distance <= viewRange && HasLineOfSight();
    }

    protected bool HasLineOfSight()
    {
        if (player == null) return false;

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        // Lança um raio do inimigo até ao jogador
        // Se o raio bater em algo que pertença à 'occlusionLayers' antes de chegar ao jogador, retorna false
        if (Physics.Raycast(transform.position + Vector3.up, direction, out RaycastHit hit, distance, occlusionLayers))
        {
            // Se bateu em algo (parede), não consegue ver/ouvir
            return false;
        }

        // Se o caminho estiver limpo
        return true;
    }

    protected void RotateTowardPlayer()
    {
        if (player == null) return;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 360f * (Time.deltaTime * timeScale));
    }

    public bool RotateTowardPlayerEnded()
    {
        if (player == null) return true;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        if (dir.sqrMagnitude < 0.01f)
            return true;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            360f * (Time.deltaTime * timeScale)
        );

        // Return true when rotation is close enough
        return Quaternion.Angle(transform.rotation, targetRot) < 1f;
    }
    private void OnDrawGizmosSelected()
    {
        // AUDIO RANGE
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, audioRange);

        // VIEW RANGE
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewRange);

        // VIEW ANGLE (Cone lines)
        Gizmos.color = Color.cyan;
        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + left * viewRange);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRange);
    }

}
