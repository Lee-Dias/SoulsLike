using LibGameAI.FSMs;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemyAI : MonoBehaviour
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

    [Header("Decision Chance")]
    [SerializeField] protected float chanceToCircle = 0.5f; // 50/50 default

    [SerializeField] protected Animator anim;

    [SerializeField] protected BoxCollider weaponCollider;

    protected NavMeshAgent agent;
    protected StateMachine stateMachine;
    protected Health health;

    protected State idleState;
    protected State decideState;
    protected State chaseState;
    protected State circleState;
    protected State attackState;

    protected Transform player;

    protected float baseSpeed;
    protected float currentHealth;
    protected bool playerInViewRange;
    protected bool playerInAudioRange;

    protected bool isDead;

    protected float circleTimer;
    protected float circleDirection = 1f;

    protected bool attackEnded = false;
    protected bool isInAttackAnimation = false;

    protected CombatAnimationManager animManager;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
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
                transform.position += (transform.forward * mov.y + transform.right * mov.x) * Time.deltaTime;
            }
                

        }
        if (!health.CanAttack())
        {
            if (animManager != null)
            {
                animManager?.UpdatePerFrame(Time.deltaTime);
            }
        }
        
        UpdatePerception();
        var actions = stateMachine.Update();
        actions?.Invoke();
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
    }

    // ------------------------------------------------------
    //  FSM TRANSITIONS
    // ------------------------------------------------------
    protected virtual void CreateTransitions()
    {
        idleState.AddTransition(new Transition(() => playerInViewRange || playerInAudioRange, null, decideState));

        decideState.AddTransition(new Transition(() => ShouldGoIdle(), null, idleState));
        decideState.AddTransition(new Transition(() => ShouldChase(), null, chaseState));
        decideState.AddTransition(new Transition(() => ShouldCircle(), null, circleState));
        decideState.AddTransition(new Transition(() => ShouldAttack() && health.CanAttack(), null, attackState));

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

        return Random.value < chanceToCircle;
    }

    private bool ShouldAttack()
    {
        if (!IsInPreferredRange()) return false;
        return Random.value >= chanceToCircle;
    }

    // ------------------------------------------------------
    //  STATE BEHAVIOR
    // ------------------------------------------------------

    protected virtual void OnEnterIdle()
    {
        agent.isStopped = true;
        Debug.Log("Idle");
    }

    protected virtual void Idle() { }

    protected virtual void OnEnterDecide()
    {
        agent.isStopped = false;
        agent.updateRotation = false;
        attackEnded = false;
        circleTimer = Random.Range(minTimeCircling, maxTimeCircling);
        Debug.Log("Deciding...");
    }

    protected virtual void Decide() { }

    protected virtual void OnEnterChase()
    {
        agent.isStopped = false;
        agent.speed = baseSpeed;
        Debug.Log("Chasing player");
    }

    protected virtual void Chase()
    {
        if (player == null) return;

        RotateTowardPlayer();
        agent.SetDestination(player.position);
    }

    protected virtual void OnEnterCircle()
    {
        Debug.Log("Circling player");
        agent.isStopped = false;
        agent.speed = circleEnemySpeed;

        circleTimer = Random.Range(minTimeCircling, maxTimeCircling);
        circleDirection = Random.value > 0.5f ? 1f : -1f;
    }

    protected virtual void Circle()
    {
        if (player == null) return;
        if (health.ShouldBlockMovement())
        {
            agent.isStopped=true;
        }
        else
        {
            agent.isStopped = false;
        }
        RotateTowardPlayer();

        Vector3 toPlayer = (player.position - transform.position).normalized;
        Vector3 strafe = Vector3.Cross(Vector3.up, toPlayer) * circleDirection;

        Vector3 target = transform.position + strafe * 2f;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < minPreferredDistanceFromPlayer) target -= toPlayer;
        else if (distance > maxPreferredDistanceFromPlayer) target += toPlayer;

        agent.SetDestination(target);

        circleTimer -= Time.deltaTime;
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

        playerInAudioRange = dist <= audioRange;
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

        return angle <= viewAngle / 2f && Vector3.Distance(transform.position, player.position) <= viewRange;
    }

    protected void RotateTowardPlayer()
    {
        if (player == null) return;
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 360f * Time.deltaTime);
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
            360f * Time.deltaTime
        );

        // Return true when rotation is close enough
        return Quaternion.Angle(transform.rotation, targetRot) < 1f;
    }
}
