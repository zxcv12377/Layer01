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
    [SerializeField] private float lostDistance; // �÷��̾�� ���� �Ÿ��� �־������� �����ϱ� ���� ����
    [SerializeField] private float remainingDistance; // �÷��̾���� ���� ���� �Ÿ�
    [SerializeField] private float stoppingDistance; // �÷��̾�� �����ϱ� ��, ��������� �Ÿ����� ������ �˷��ִ� ����
    [Space(5)]
    [Header("Delay")]
    [SerializeField] private float teleportDelay; // �÷��̾ ����ٴҶ� ����� ������
    [Space(5)]
    [Header("Attack State")]
    [SerializeField] private float spellCooldown; // ���� �ð����� ���������� ���ϱ� ���� �ð�
    [SerializeField] private float groggyTime; // �׷α� �ð�
    [Space(5)]
    [Header("State")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float HP = 0; // ü��
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
    //    // ���� �������� Animator ���� ����
    //    var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

    //    // �ִϸ��̼� �̸��� Idle�� �ƴϸ� Play
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
    //        // SetDestination�� ���� �� �������� �ѱ������ �ڵ�
    //        yield return null;
    //    }
    //    // ��ǥ���� ���� �Ÿ��� ���ߴ� �������� �۰ų� ������ ����
    //    if(remainingDistance <= stoppingDistance)
    //    {
    //        ChangeState(State.ATTACK);
    //    }
    //    // ��ǥ���� �Ÿ��� �־��� ���
    //    else if(remainingDistance > lostDistance)
    //    {
    //        yield return null;
    //    }
    //    else
    //    {
    //        // �ִϸ��̼��� �� ����Ŭ ���� ���
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
        if(remainingDistance > stoppingDistance) // �Ÿ��� �־����� �ٽ� �߰�
        {
            ChangeState(State.CHASE);
        }
        else
        {
            // ��� �ð��� �̿��� ���� ������ ������ �� ����
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
