using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeathBringerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Transform target;
    private Vector2 moveDirection;

    private NavMeshAgent nmAgent;

    [Header("Navigator")]
    public float lostDistance; // 플레이어와 일정 거리가 멀어질때를 감지하기 위한 변수
    public float remainingDistance; // 플레이어까지 현재 남은 거리
    public float stoppingDistance; // 플레이어에게 도착하기 전, 어느정도의 거리에서 멈출지 알려주는 변수
    [Space(5)]
    [Header("Chase Delay")]
    public float chaseDelay; // 플레이어를 따라다닐때 생기는 딜레이
    [Space(5)]
    [Header("Attack State")]
    public float spellCooldown; // 일정 시간마다 범위공격을 가하기 위한 시간
    public float groggyTime; // 그로기 시간
    [Space(5)]
    [Header("State")]
    public float moveSpeed;
    public float HP = 0; // 체력



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

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        state = State.CHASE;
        StartCoroutine(StateMachine());
    }


    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        SetDestination(target);
    }

    IEnumerator StateMachine()
    {
        while(HP > 0)
        {
            yield return StartCoroutine(state.ToString());
        }
    }

    IEnumerator IDLE()
    {
        // 현재 진행중인 Animator 상태 정보
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 이름이 Idle이 아니면 Play
        if (!curAnimStateInfo.IsName("Idle"))
        {
            anim.Play("Idle", 0, 0);
        }
        yield return null;
    }

    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (!curAnimStateInfo.IsName("Walk"))
        {
            anim.Play("Walk", 0, 0);
            // SetDestination을 위해 한 프레임을 넘기기위한 코드
            yield return null;
        }

        if(remainingDistance <= stoppingDistance)
        {
            ChangeState(State.ATTACK);
        }
        else if(remainingDistance > lostDistance)
        {
            target = null;
            SetDestination(target);
            yield return null;
            ChangeState(State.IDLE);
        }
        else
        {
            yield return new WaitForSeconds(curAnimStateInfo.length);
        }
    }
    //naviMesh를 사용하지 않으니 직접 만들어야함
    IEnumerator ATTACK()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        anim.Play("Attack", 0, 0);
        if(remainingDistance > stoppingDistance)
        {
            ChangeState(State.CHASE);
        }
        else
        {
            yield return new WaitForSeconds(curAnimStateInfo.length * 2f);
        }
    }

    void ChangeState(State newState)
    {
        state = newState;
    }

    public void SetDestination(Transform target)
    {
        if (target)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            remainingDistance = direction.x;
            moveDirection = direction;
            rb.velocity = new Vector2(moveDirection.x, 0) * moveSpeed;
        }
    }
}
