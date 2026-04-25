using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] AudioClip footStep;
    [SerializeField] Animator targetAnimator;
    [SerializeField] PlayerInputReader inputReader;
    [SerializeField] PlayerHP playerHP;

    [SerializeField] string moveXParameterName;
    [SerializeField] string moveYParameterName;
    [SerializeField] string isMoveParameterName;
    [SerializeField] string onHitParameterName;
    [SerializeField] string onDieParameterName;

    private int moveXHash;
    private int moveYHash;
    private int isMoveHash;
    private int onHitHash;
    private int onDieHash;

    float moveDeadZone = 0.01f;

    Vector2 currentMoveDir = Vector2.zero;
    bool isMove = false;
    private void Awake()
    {
        if(targetAnimator == null) targetAnimator = GetComponent<Animator>();
        if(inputReader == null) inputReader = GetComponentInParent<PlayerInputReader>();
        if(playerHP == null) playerHP = GetComponent<PlayerHP>();

        moveXHash = Animator.StringToHash(moveXParameterName);
        moveYHash = Animator.StringToHash(moveYParameterName);
        isMoveHash = Animator.StringToHash(isMoveParameterName);
        onHitHash = Animator.StringToHash(onHitParameterName);
        onDieHash = Animator.StringToHash(onDieParameterName);
    }
    private void OnEnable()
    {
        if(playerHP != null)
        {
            playerHP.OnHit += OnHitTrigger;
            playerHP.OnDied += OnDieTrigger;
        }
    }
    private void OnDisable()
    {
        if (playerHP != null)
        {
            playerHP.OnHit -= OnHitTrigger;
            playerHP.OnDied -= OnDieTrigger;
        }
    }
    private void Update()
    {
        if (targetAnimator == null) return;

        if(inputReader != null)
        {
            currentMoveDir = inputReader.MoveDir;
        }
        isMove = currentMoveDir.sqrMagnitude > moveDeadZone;

        SetParameter(currentMoveDir, isMove);
    }

    private void SetParameter(Vector2 dir, bool isMove)
    {
        targetAnimator.SetFloat(moveXHash, currentMoveDir.x);
        targetAnimator.SetFloat(moveYHash, currentMoveDir.y);
        targetAnimator.SetBool(isMoveHash, isMove);
    }
    private void OnDieTrigger()
    {
        targetAnimator.SetTrigger(onDieHash);
    }
    private void OnHitTrigger(float amount)
    {
        targetAnimator.SetTrigger(onHitHash);
    }
}
