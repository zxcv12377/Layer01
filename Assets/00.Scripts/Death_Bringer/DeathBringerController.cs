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
    public float lostDistance; // �÷��̾�� ���� �Ÿ��� �־������� �����ϱ� ���� ����
    public float remainingDistance; // �÷��̾���� ���� ���� �Ÿ�
    public float stoppingDistance; // �÷��̾�� �����ϱ� ��, ��������� �Ÿ����� ������ �˷��ִ� ����
    [Space(5)]
    [Header("Chase Delay")]
    public float chaseDelay; // �÷��̾ ����ٴҶ� ����� ������
    [Space(5)]
    [Header("Attack State")]
    public float spellCooldown; // ���� �ð����� ���������� ���ϱ� ���� �ð�
    public float groggyTime; // �׷α� �ð�
    [Space(5)]
    [Header("State")]
    public float moveSpeed;
    public float HP = 0; // ü��



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
        // ���� �������� Animator ���� ����
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼� �̸��� Idle�� �ƴϸ� Play
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
            // SetDestination�� ���� �� �������� �ѱ������ �ڵ�
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
    //naviMesh�� ������� ������ ���� ��������
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
