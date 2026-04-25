using System;
using System.Collections;
using Unity.Multiplayer.PlayMode;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Chase,
    Attack,
    NuckBack,
    Stun,
    Dead,
    Return,
}
public class EnemyFSMController : MonoBehaviour
{
    [Header("Ref")]
    [SerializeField] private EnemyHP enemyHP;
    [SerializeField] EnemyAttack enemyAttack;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private Transform targetPos;

    [Header("FSM")]
    [SerializeField] private EnemyState currentState = EnemyState.Idle;
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float attackDistance = 1.5f;

    [SerializeField] LayerMask targetLayer;
    [SerializeField] private float attackAfterDelay;

    [Header("SFX")]
    [SerializeField] private float idleDistance = 10f;
    [SerializeField] AudioClip chaseSound;
    [SerializeField] AudioClip walkSound;
    [SerializeField] float walkSoundDelay = 0.27f;
    public event Action<EnemyState> OnEnemyStateChanged;

    public event Action OnAttack;
    public EnemyState CurrentState => currentState;
    public Vector2 CurrentDir {  get; private set; }
    public void SetCurrentState(EnemyState state) => TransitionTo(state);

    private Vector2 originPos;
    Coroutine attackCoroutine;

    [SerializeField] float stuckCheckTime = 0.2f;
    [SerializeField] float stuckMoveThreshold = 0.05f;

    private Vector2 lastPosition;
    private float stuckTimer;



    private void Awake()
    {
        if(enemyHP == null) enemyHP = GetComponentInChildren<EnemyHP>();
        if(rb2D == null) rb2D = GetComponentInChildren<Rigidbody2D>();
        if(enemyAttack == null) enemyAttack = GetComponent<EnemyAttack>();
    }
    private void Start()
    {
        StartCoroutine(CoIdleSoundSFXPlay());
        originPos = transform.position;
    }
    private void OnEnable()
    {
        if (enemyHP != null)
        {
            enemyHP.OnDied += HandleDead;
            enemyHP.OnHit += ApplyStun;
        }
    }
    private void OnDisable()
    {
        if (enemyHP != null)
        {
            enemyHP.OnDied -= HandleDead;
            enemyHP.OnHit -= ApplyStun;
        }
    }
    private void TransitionTo(EnemyState nextState)
    {
        if (currentState == nextState) return;

        currentState = nextState;

        if (currentState == EnemyState.Chase)
        {
            ResetStuckCheck();
        }

        OnEnemyStateChanged?.Invoke(currentState);
    }
    private void ResetStuckCheck()
    {
        lastPosition = transform.position;
        stuckTimer = 0f;
    }
    private void FixedUpdate()
    {
        rb2D.linearVelocity = Vector2.zero;
        rb2D.angularVelocity = 0f;

        if (currentState == EnemyState.Chase)
        {
            HandleChaseMovement();
            return;
        }
        if (currentState == EnemyState.Idle)
        {
            CurrentDir = Vector2.zero;
            return;
        }
        if (currentState == EnemyState.Attack)
        {
            HandleAttack(targetPos);
            return;
        }
        if (currentState == EnemyState.Return)
        {
            HandleReturnMovement();
            return;
        }
    }
    private bool TargetInIdleDistance()
    {
        if(targetPos == null) return false;
        float distanceToTarget = Vector2.Distance(transform.position, targetPos.position);
        if (distanceToTarget <= chaseDistance && chaseSound != null)
        {
            return true;
        }
        return false;
    }
    IEnumerator CoIdleSoundSFXPlay()
    {
        while (true)
        {
            yield return new WaitUntil(()=> TargetInIdleDistance());
            if (targetPos != null && currentState == EnemyState.Chase)
            {
                SoundManager.Instance.PlaySfxOneShot(chaseSound,0.4f);
            }
            yield return new WaitUntil(() => currentState == EnemyState.Idle);
        }
    }
    private void Update()
    {
        if (currentState == EnemyState.Dead) return;
        if(targetPos == null)
        {
            TryFindTarget();
            return;
        }
        if (currentState == EnemyState.Return) return;
        EvaluateStateTransition();
    }
    Coroutine stunCoroutine = null;
    private void ApplyStun(float amount)
    {
        if(stunCoroutine == null)
        {
            stunCoroutine = StartCoroutine(SetStun());
        }
        else
        {
            StopCoroutine(stunCoroutine);
            stunCoroutine = StartCoroutine(SetStun());
        }
    }
    private bool IsStuck()
    {
        float movedDistance = Vector2.Distance(transform.position, lastPosition);

        if (movedDistance < stuckMoveThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
            lastPosition = transform.position;
        }

        return stuckTimer >= stuckCheckTime;
    }
    IEnumerator SetStun()
    {
        TransitionTo(EnemyState.Stun);
        if(attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = null;
        CurrentDir = Vector2.zero;
        float distanceToTarget = Vector2.Distance(transform.position, targetPos.position);

        yield return new WaitForSeconds(0.4f);

        if (distanceToTarget <= attackDistance)
        {
            TransitionTo(EnemyState.Attack);
        }
        else if (distanceToTarget > chaseDistance * 1.2f)
        {
            TransitionTo(EnemyState.Return);
        }
        else
        {
            TransitionTo(EnemyState.Chase);
        }
    }

    private bool CanSeeTarget()
    {
        if(targetPos == null) return false;

        RaycastHit2D hit2D = Physics2D.Linecast(transform.position, targetPos.position, targetLayer);

        Debug.DrawLine(transform.position, hit2D.point, Color.red);
        if (hit2D.collider.gameObject.layer != targetPos.gameObject.layer)
        {
            return false;
        }
        return true;
    }
    private void EvaluateStateTransition()
    {
        if (targetPos == null) return;
        if (currentState == EnemyState.Dead) return;
        if (enemyHP.IsDead)
        {
            TransitionTo(EnemyState.Dead);
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, targetPos.position);
        if (!CanSeeTarget() && currentState == EnemyState.Chase)
        {
            TransitionTo(EnemyState.Return);
            return;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                if (!CanSeeTarget())
                {
                    return;
                }
                if (distanceToTarget <= chaseDistance)
                {
                    TransitionTo(EnemyState.Chase);
                }
                break;
            case EnemyState.Chase:
                if (distanceToTarget <= attackDistance)
                {
                    TransitionTo(EnemyState.Attack);
                }
                else if (distanceToTarget > chaseDistance * 1.2f)
                {
                    TransitionTo(EnemyState.Return);
                }
                break;
            case EnemyState.Attack:
                if (distanceToTarget > attackDistance && attackCoroutine == null)
                {
                    TransitionTo(EnemyState.Chase);
                }
                break;
        }
    }
    private void TryFindTarget()
    {
        if (targetPos != null) return;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            targetPos = playerObject.transform;
        }
    }

    float walktimer = 0f;
    private void HandleChaseMovement()
    {
        if (targetPos == null || rb2D == null) return;
        if (currentState != EnemyState.Chase) return;


        CurrentDir = (Vector2)(targetPos.position - transform.position).normalized;

        CurrentDir = ToTarget(CurrentDir);

        rb2D.MovePosition(rb2D.position + CurrentDir * moveSpeed * Time.fixedDeltaTime);
        walktimer += Time.fixedDeltaTime;
        if(walktimer >= walkSoundDelay && SoundManager.Instance)
        {
            SoundManager.Instance.PlaySfxOneShot(walkSound);
            walktimer = 0f;
        }
    }

    private Vector2 ToTarget(Vector2 currentDir)
    {
        if (IsStuck())
        {
            if(Mathf.Abs(currentDir.x) > Mathf.Abs(currentDir.y))
            {
                currentDir.x = 0;

            }
            else if(Mathf.Abs(currentDir.x) < Mathf.Abs(currentDir.y))
            {
                currentDir.y = 0;
            }
        }
        return currentDir;
    }

    private float lastAttackTime;
    private void HandleAttack(Transform targetTansform)
    {
        if (lastAttackTime > Time.time) return;
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(Attack(targetTansform));
        }

    }
    IEnumerator Attack(Transform targetTansform)
    {
        CurrentDir = Vector2.zero;

        if (enemyHP.IsDead)
        {
            attackCoroutine = null;
            yield break;
        }

        yield return new WaitForSeconds(0.2f);
        OnAttack?.Invoke();
        enemyAttack.BeginAttack(targetTansform, out HitBox hitBox);

        yield return new WaitForSeconds(0.1f);
        enemyAttack.EndAttack(hitBox);

        yield return new WaitForSeconds(attackAfterDelay);
        attackCoroutine = null;
        lastAttackTime = Time.time + enemyAttack.AttackCoolDown;
    }


    private void HandleDead()
    {
        if(attackCoroutine != null) StopCoroutine (attackCoroutine);
        TransitionTo(EnemyState.Dead);
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
        }

    }
    private void HandleReturnMovement()
    {
        if (CurrentState != EnemyState.Return) return;
        CurrentDir = (originPos - (Vector2)transform.position).normalized;

        CurrentDir = ToTarget(CurrentDir);

        rb2D.MovePosition(rb2D.position + CurrentDir * moveSpeed * Time.fixedDeltaTime);

        float distance = Vector2.Distance(transform.position, originPos);
        if (distance <= 0.1f)
        {
            enemyHP.Heal((int)enemyHP.MaxHP);
            TransitionTo(EnemyState.Idle);
            return;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance * 1.2f);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position,idleDistance);
        //if (targetPos != null) Gizmos.DrawLine(transform.position, targetPos.position);
    }
}
