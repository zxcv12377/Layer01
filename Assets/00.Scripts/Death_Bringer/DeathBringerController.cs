using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Transform target;
    private Transform saveTarget;
    private Vector3 moveDirection;

    public bool isTeleport { get; private set; }
    public bool teleportRefill { get; private set; }
    public bool isAttack { get; private set; }

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
    [Space(15)]
    [Header("GuidedMissile")]
    [SerializeField] private GameObject GuidedMissile;
    [SerializeField] private Transform launcherPosition;

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

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).transform;
        saveTarget = target;

        Initialized();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        
        StartCoroutine(nameof(StateMachine));
        
    }

    void Update()
    {
        Debug.Log(state);
        if (target == null) return;
        SetDestination(target);
        CheckDirectionFace(moveDirection.x > 0);

        //StartCoroutine(nameof(Teloport));

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Spell();
        }
    }
    #region CHECK
    //public void CollisionCheck()
    //{
    //    if(Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, _layerMask))
    //    {
    //        MonsterFlip();
    //    }
    //}

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

    #region IDLE
    IEnumerator IDLE()
    {
        // ���� �������� Animator ���� ����
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // �ִϸ��̼� �̸��� Idle�� �ƴϸ� Play
        if (!curAnimStateInfo.IsName("Idle"))
        {
            anim.Play("Idle", 0, 0);
        }

        yield return new WaitForSeconds(5f);
        ChangeState(State.CHASE);

    }
    #endregion

    #region CHASE
    IEnumerator CHASE()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (target == null)
        {
            target = saveTarget;
        }

        if (!curAnimStateInfo.IsName("Walk"))
        {
            anim.Play("Walk", 0, 0);
            // SetDestination�� ���� �� �������� �ѱ������ �ڵ�
            yield return null;
        }
        // ��ǥ���� ���� �Ÿ��� ���ߴ� �������� �۰ų� ������ ����
        if (remainingDistance <= stoppingDistance)
        {
            ChangeState(State.ATTACK);
        }
        // ��ǥ���� �Ÿ��� �־��� ���
        else if (remainingDistance > lostDistance)
        {
            
            yield return null;
        }
        else
        {
            // �ִϸ��̼��� �� ����Ŭ ���� ���
            
            yield return new WaitForSeconds(curAnimStateInfo.length);
        }
    }
    #endregion

    #region TELEPORT
    IEnumerator Teloport()
    {
        if(target == null)
        {
            target = saveTarget;
        }
        if (!isTeleport)
        {
            yield return null;
        }
        else
        {
            var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            var wrPicker = new Rito.WeightedRandomPicker<string>();

            wrPicker.Add(
                ("Left", 50),
                ("Right", 50)
                );

            isTeleport = false;
            anim.Play("Teleport_before");
            yield return new WaitForSeconds(curAnimStateInfo.length);

            transform.position = new Vector3(-120, transform.position.y);
            string randIndex = wrPicker.GetRandomPick();
            Debug.Log("randIndex : " + randIndex);
            yield return new WaitForSeconds(curAnimStateInfo.length * 2f);

            transform.position = (randIndex != "Right") ? new Vector3(target.position.x + stoppingDistance, transform.position.y) : new Vector3(target.position.x - stoppingDistance, transform.position.y);
            enabled = true;
            anim.Play("Teleport_after");
            StartCoroutine(nameof(DelayOfTeleport));
        }
    }
    #endregion

    #region SPELL
    public void Spell()
    {
        anim.Play("Cast-NoEffect");
        var missile = ObjectPooling.GetObject(target);
        missile.transform.position = launcherPosition.position;
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
    #endregion

    #region ANIMATION STATE
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
            transform.position += new Vector3(moveDirection.x, 0, 0) * moveSpeed * Time.deltaTime;
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

    IEnumerator DelayOfTeleport()
    {
        yield return new WaitForSeconds(teleportDelay);
        isTeleport = true;
    }

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
    #endregion

    #region INITIALIZED
    private void Initialized()
    {
        isTeleport = true;
        isAttack = true;
        state = State.IDLE; // �⺻�� IDLE ���·� ����
    }
    #endregion
}
