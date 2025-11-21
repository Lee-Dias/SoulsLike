using LibGameAI.FSMs;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemyAI : MonoBehaviour
{
    [Header("Ranges")]
    [SerializeField] protected float minPreferredDistanceFromPlayer;
    [SerializeField] protected float maxPreferredDistanceFromPlayer;
    [SerializeField] protected float audioRange;
    [SerializeField] protected float viewRange;  // Max range of vision (distance from the enemy)
    [SerializeField] protected float viewAngle;  // Angle of the view cone (in degrees)

    [Header("Movement")]
    [SerializeField] protected float circleEnemySpeed; // used only when circling

    [Header("Timing & Stats")]
    [SerializeField] protected float minTimeCircling;
    [SerializeField] protected float maxTimeCircling;
    [SerializeField] protected float attacksCooldown;
    [SerializeField] protected EnemyProfile enemy;

    [SerializeField] protected Animator anim;

    protected NavMeshAgent agent;
    protected StateMachine stateMachine;
    protected State idleState;
    protected State chaseState;
    protected State circleState;
    protected State attackState;

    protected Transform player;
    protected float baseSpeed;
    protected float currentHealth;
    protected bool playerInViewRange;
    protected bool playerInAudioRange;
    protected bool takingDamage;
    protected bool isDead;
    protected float attackTimer;

    protected float circleTimer;
    protected bool canCircle = true;

    protected State stateAfterCircle;

    protected bool attacked = false;
    protected float circleDirection = 1f;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        baseSpeed = agent.speed;

        CreateStates();
        CreateTransitions();

        stateMachine = new StateMachine(idleState);

        // Manually trigger the entry actions of the starting state
        idleState.EntryActions?.Invoke();
    }

    protected virtual void Update()
    {
        Vector3 localVel = transform.InverseTransformDirection(agent.velocity);

        anim.SetFloat("x", localVel.x);  // strafe left/right
        anim.SetFloat("y", localVel.z);  // forward/backwards
        UpdatePerception();

        // Run whatever actions the FSM returns
        var actions = stateMachine.Update();
        actions?.Invoke();
    }

    protected virtual void CreateStates()
    {
        idleState = new State("Idle", OnEnterIdle, Idle, null);
        chaseState = new State("Chase", OnEnterChase, Chase, null);
        circleState = new State("Circle", OnEnterCircle, Circle, null);
        attackState = new State("Attack", OnEnterAttack, Attack, null);
    }

    protected virtual void CreateTransitions()
    {
        idleState.AddTransition(new Transition(() => !playerInViewRange || !playerInAudioRange, null, chaseState));
        chaseState.AddTransition(new Transition(() => IsInPreferredRange(), null, circleState));
        circleState.AddTransition(new Transition(() => stateAfterCircle == attackState, null, attackState));
        circleState.AddTransition(new Transition(() => stateAfterCircle == circleState, null, circleState));
        circleState.AddTransition(new Transition(() => !IsInPreferredRange() && playerInViewRange , null, chaseState));
        circleState.AddTransition(new Transition(() => !IsInPreferredRange() && !playerInViewRange && !playerInAudioRange, null, idleState));
        attackState.AddTransition(new Transition(() => attacked, null, circleState));
    }

    // -------------------- STATE LOGIC --------------------

    protected virtual void OnEnterIdle()
    {
        Debug.Log("Is Idle");
        agent.isStopped = true;
    }

    protected virtual void Idle()
    {
        
    }

    protected virtual void OnEnterChase()
    {
        Debug.Log("Is Chasing");

        // Smoothly rotate the enemy to face the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 5f * Time.deltaTime); // Adjust rotation speed

        agent.isStopped = false;
        agent.speed = baseSpeed;
    }

    protected virtual void Chase()
    {
        if (player == null) return;

        // Keep the enemy facing the player during the chase
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 5f * Time.deltaTime); // Smooth rotation

        // Move towards the player
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > minPreferredDistanceFromPlayer)
        {
            agent.SetDestination(player.position); // Keep moving towards the player
        }
        else
        {
            agent.isStopped = true; // Stop moving when in the preferred range
        }
    }

    protected virtual void OnEnterCircle()
    {
        stateAfterCircle = null;
        Debug.Log("Is Circling");
        agent.isStopped = false;
        agent.speed = circleEnemySpeed;

        // Reset the circle timer to a random value between min and max time
        ResetCircleTimer();

        // Randomly decide whether to circle clockwise or counterclockwise
        circleDirection = Random.value > 0.5f ? 1f : -1f;
    }

    protected virtual void Circle()
    {
        if (player == null) return;

        // Always face the player
        Vector3 toPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(toPlayer);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 360f * Time.deltaTime);

        // Strafe direction (sideways relative to the player)
        Vector3 strafeDir = Vector3.Cross(Vector3.up, toPlayer) * circleDirection;

        // Base target position: sideways from current position, keeping the player as the pivot
        Vector3 targetPos = transform.position + strafeDir * 2f; // 2 meters per side-step movement, feels natural

        // Keep the enemy near the preferred distance
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < minPreferredDistanceFromPlayer)
            targetPos -= toPlayer * 1f; // back off a bit
        else if (distance > maxPreferredDistanceFromPlayer)
            targetPos += toPlayer * 1f; // move slightly closer

        // Move there — let NavMeshAgent speed control actual rate
        agent.SetDestination(targetPos);    

        circleTimer -= Time.deltaTime;
        // If time is up, transition based on conditions
        if (circleTimer <= 0)
        {
            int rnd = Random.Range(1, 3);
            if (rnd == 1)
            {
                stateAfterCircle = circleState;
            }
            else
            {
                stateAfterCircle = attackState;
            }
            stateMachine.Update();
            ResetCircleTimer();
        }
    }

    protected abstract void OnEnterAttack();
    protected abstract void Attack(); // implemented by subclasses

    // -------------------- HELPERS --------------------

    protected virtual void UpdatePerception()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        IsPlayerInViewRange();

        playerInAudioRange = distance <= audioRange;
    }

    protected bool IsInPreferredRange()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance >= minPreferredDistanceFromPlayer && distance <= maxPreferredDistanceFromPlayer;
    }

    // Helper method to reset the circle timer to a random value between min and max
    private void ResetCircleTimer()
    {
        circleTimer = Random.Range(minTimeCircling, maxTimeCircling);
    }

    protected virtual bool IsPlayerInViewRange()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // Calculate the angle between the enemy's forward direction and the direction to the player
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check if the player is within the field of view (cone) and within the range
        if (angleToPlayer <= viewAngle / 2 && Vector3.Distance(transform.position, player.position) <= viewRange)
        {
            return true;
        }

        return false;
    }

    // -------------------- GIZMOS --------------------

    private void OnDrawGizmosSelected()
    {
        // Only draw gizmos when the player is assigned
        if (player == null) return;

        // Set Gizmo color for the view cone
        Gizmos.color = Color.green;

        // Calculate the boundaries of the view cone based on the enemy's current forward direction
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * viewRange;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward * viewRange;

        // Draw the left and right boundary lines
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);  // Left boundary of the cone
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary); // Right boundary of the cone

        // Draw the center line for reference
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * viewRange);

        // Draw the wire sphere to represent the total view range
        Gizmos.DrawWireSphere(transform.position, viewRange);

    }

}
