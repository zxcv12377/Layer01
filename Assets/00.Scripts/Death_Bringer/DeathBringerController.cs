using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Transform target;
    private Vector3 moveDirection;

    [Header("Check")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize;
    [Space(5)]
    [Header("Navigator")]
    [SerializeField] private float lostDistance; // 플레이어와 일정 거리가 멀어질때를 감지하기 위한 변수
    [SerializeField] private float remainingDistance; // 플레이어까지 현재 남은 거리
    [SerializeField] private float stoppingDistance; // 플레이어에게 도착하기 전, 어느정도의 거리에서 멈출지 알려주는 변수
    [Space(5)]
    [Header("Delay")]
    [SerializeField] private float teleportDelay; // 플레이어를 따라다닐때 생기는 딜레이
    [Space(5)]
    [Header("Attack State")]
    [SerializeField] private float spellCooldown; // 일정 시간마다 범위공격을 가하기 위한 시간
    [SerializeField] private float groggyTime; // 그로기 시간
    [Space(5)]
    [Header("State")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float HP = 0; // 체력
    [SerializeField] private bool monsterDirRight;
    [Space(5)]
    [Header("Layer")]
    [SerializeField] private LayerMask _layerMask;

    #region ENUM
    enum State
    {
        IDLE,
        KILLED,
        HURT,
        CHASE,
        ATTACK,
        SPELL
    }
    State state;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        WeightRandomPicker();
    }


    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        SetDestination(target);
        CheckDirectionFace(moveDirection.x > 0);
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StartCoroutine(Teloport());
        }
    }
    #region CHECK
    public void CollisionCheck()
    {
        if(Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, _layerMask))
        {
            MonsterFlip();
        }
    }

    public void CheckDirectionFace(bool isMovingRight)
    {
        if (isMovingRight != monsterDirRight)
        {
            MonsterFlip();
        }
    }
    #endregion

    #region STATEMACHINE
    IEnumerator StateMachine()
    {
        while(HP > 0)
        {
            yield return StartCoroutine(state.ToString());
        }
    }
    #endregion

    //#region IDLE
    //IEnumerator IDLE()
    //{
    //    // 현재 진행중인 Animator 상태 정보
    //    var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

    //    // 애니메이션 이름이 Idle이 아니면 Play
    //    if (!curAnimStateInfo.IsName("Idle"))
    //    {
    //        anim.Play("Idle", 0, 0);
    //    }
    //    yield return null;
    //}
    //#endregion

    #region CHASE
    //IEnumerator CHASE()
    //{
    //    var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

    //    transform.position += moveDirection * moveSpeed * Time.deltaTime;

    //    if (!curAnimStateInfo.IsName("Walk"))
    //    {
    //        anim.Play("Walk", 0, 0);
    //        // SetDestination을 위해 한 프레임을 넘기기위한 코드
    //        yield return null;
    //    }
    //    // 목표까지 남은 거리가 멈추는 지점보다 작거나 같으면 공격
    //    if(remainingDistance <= stoppingDistance)
    //    {
    //        ChangeState(State.ATTACK);
    //    }
    //    // 목표와의 거리가 멀어진 경우
    //    else if(remainingDistance > lostDistance)
    //    {
    //        yield return null;
    //    }
    //    else
    //    {
    //        // 애니메이션의 한 사이클 동안 대기
    //        ChangeState(State.IDLE);
    //        yield return new WaitForSeconds(chaseDelay);
    //    }
    //}
    #endregion

    #region TELEPORT
    IEnumerator Teloport()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        var wrPicker = new Rito.WeightedRandomPicker<string>();

        wrPicker.Add(
            ("Left", 50),
            ("Right", 50)
            );


        anim.Play("Teleport_before");
        yield return new WaitForSeconds(curAnimStateInfo.length);

        transform.position = new Vector3(-120, transform.position.y);
        string randIndex = wrPicker.GetRandomPick();
        Debug.Log("randIndex : " + randIndex);
        yield return new WaitForSeconds(curAnimStateInfo.length * 2f);

        transform.position = (randIndex != "Right") ? new Vector3(target.position.x + stoppingDistance, transform.position.y) : new Vector3(target.position.x - stoppingDistance, transform.position.y);
        enabled = true;
        anim.Play("Teleport_after");
    }
    #endregion

    #region SPELL
    public void Spell()
    {
        
    }
    #endregion

    #region GUIDED ATTACK
    public void Guided_Attack()
    {
        
    }
    #endregion

    #region PATTERN1
    public void Pattern1()
    {
        
    }
    #endregion

    #region PATTERN2
    public void Pattern2()
    {
        
    }
    #endregion

    #region PATTERN3
    public void Pattern3()
    {
        
    }
    #endregion

    #region GROGGY
    public void Groggy()
    {
        
    }
    #endregion

    #region KILLED
    public void  Killed()
    {
        
    }
    #endregion

    #region HURT
    public void Hit()
    {
        
    }
    #endregion

    #region ATTACK
    IEnumerator ATTACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.Play("Attack", 0, 0);
        if(remainingDistance > stoppingDistance) // 거리가 멀어지면 다시 추격
        {
            ChangeState(State.CHASE);
        }
        else
        {
            // 대기 시간을 이용해 공격 간격을 조절할 수 있음
            yield return new WaitForSeconds(curAnimStateInfo.length * 3f);
        }
    }

    void ChangeState(State newState)
    {
        state = newState;
    }
    #endregion

    #region REMAINING DISTANCE
    public void SetDestination(Transform target)
    {
        if (target)
        {
            Vector3 direction = target.position - transform.position;
            remainingDistance = Mathf.Abs(direction.x);
            moveDirection = direction.normalized;
        }
    }
    #endregion


    #region FLIP
    public void MonsterFlip()
    {
        monsterDirRight = !monsterDirRight;

        Vector3 scale = transform.localScale;
        if (monsterDirRight)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
        }
        transform.localScale = scale;
        rb.velocity = Vector2.zero;
    }
    #endregion

    #region WEIGHTRANDOMPICK METHOD
    private void WeightRandomPicker()
    {
        
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
    #endregion
}
