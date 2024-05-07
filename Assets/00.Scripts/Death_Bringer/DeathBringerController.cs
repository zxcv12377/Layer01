using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerController : MonoBehaviour
{
    [SerializeField] private MonsterData monsterData;

    #region COMPONENT
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D col;
    [SerializeField] private Transform target;
    private Transform saveTarget;
    private Vector3 moveDirection;
    #endregion

    #region PARAMETER
    public bool isTeleport { get; private set; }
    public bool isAttack { get; private set; }
    public bool isGuidedAttack { get; private set; }
    public bool isSpell { get; private set; }
    public bool isPattern1 { get; private set; }
    

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
    [SerializeField] private float chaseDelay; // �÷��̾ ����ٴҶ� ����� ������
    [SerializeField] private float pattern1Delay; // ���� ���� �ð� 
    [Space(5)]
    [Header("Attack State")]
    [SerializeField] private float teleportCooldown; // �÷��̾ ���� �ڷ���Ʈ�� ����ϱ� ���� �ð�
    [SerializeField] private float spellCooldown; // ���� �ð����� ���������� ���ϱ� ���� �ð�
    [SerializeField] private float guidedAttackCooldown; // ���� �ð����� ���������� ���ϱ� ���� �ð�
    [SerializeField] private float groggyTime; // �׷α� �ð�
    [Space(5)]
    [Header("State")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private bool monsterDirRight;
    [Space(5)]
    [Header("Layer")]
    [SerializeField] private LayerMask _layerMask;
    [Space(15)]
    [Header("GuidedMissile")]
    [SerializeField] private GameObject GuidedMissile;
    [SerializeField] private Transform launcherPosition;
    [Space(5)]
    [Header("Pattern1")]
    [SerializeField] private GameObject patternObj;

    #endregion

    List<Skill> skills;

    #region ENUM
    enum State
    {
        IDLE,
        CHASE,
        TELEPORT,
        ATTACK,
        SPELL,
        GUIDEDATTACK,
        PATTERN1,
        PATTERN2,
        PATTERN3,
        GROGGY
    }
    State state;
    #endregion

    private void Awake()
    {
        Initialized();

        skills = new List<Skill>
        {
            new Skill("��������", guidedAttackCooldown, 1),
            new Skill("��������", spellCooldown, 2),
            new Skill("�ڷ���Ʈ", teleportCooldown, 3)
        };
    }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).transform;
        saveTarget = target;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();

        StartCoroutine(nameof(StateMachine));

        //List<Skill> skills = new List<Skill> // AI�� �ൿ�� �켱������ ���� 
        //{
        //    new Skill("��������", guidedAttackCooldown, 1),
        //    new Skill("��������", spellCooldown, 2),
        //    new Skill("�ڷ���Ʈ", teleportCooldown, 3)
        //};

    }

    void Update()
    {
        //Debug.Log(state);
        if (target == null) return;
        if(!AnimCheck()) SetDestination(target);
        CheckDirectionFace(moveDirection.x > 0);
        //Debug.Log($"isAttack : {isAttack}");
        //Debug.Log($"isTeleport : {isTeleport}");
        //Debug.Log($"isGuidedAttack : {isGuidedAttack}");

        #region SKILL PRIORITY
        foreach (Skill skill in skills)
        {
            skill.UpdateCooldown(Time.deltaTime);
        }
        Skill nextSkill = null;
        int highestPriority = int.MaxValue;
        foreach (Skill skill in skills)
        {
            if (skill.IsReady() && skill.priority < highestPriority)
            {
                highestPriority = skill.priority;
                nextSkill = skill;
            }
        }

        if (nextSkill != null && AnimCheck() == false)
        {
            //ExecuteSkill(nextSkill);
            nextSkill.currentCooldown = nextSkill.cooldown;
        }
        #endregion

        #region TEST INPUT
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //StartCoroutine(nameof(TELEPORT));
            //StartCoroutine(nameof(GUIDED_ATTACK));
            //StartCoroutine(nameof(SPELL));
            StartCoroutine(nameof(PATTERN1));

        }
        #endregion
    }

    #region SKILL STORE
    private void ExecuteSkill(Skill skill)
    {
        switch (skill.name)
        {
            case "�ڷ���Ʈ":
                StartCoroutine(nameof(TELEPORT));
                //ChangeState(State.TELEPORT);
                break;
            case "��������":
                StartCoroutine(nameof(SPELL));
                //ChangeState(State.SPELL);
                break;
            case "��������":
                StartCoroutine(nameof(GUIDED_ATTACK));
                //ChangeState(State.GUIDEDATTACK);
                break;
            default:
                Debug.LogError($"Unknown skill : {skill.name}");
                break;
        }
        Debug.Log($"Executing skill : {skill.name}");
    }
    #endregion

    #region CHECK
    //public void CollisionCheck()
    //{
    //    if(Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, _layerMask))
    //    {
    //        MonsterFlip();
    //    }
    //}

    public bool AnimCheck()
    {
        if(isTeleport || isAttack || isGuidedAttack || isSpell || isPattern1)
        {
            return true;
        }
        else
        {
            return false;
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
        while(monsterData.currentHp > 0)
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
        if(target != null)
        {
            target = null;
        }

        // �ִϸ��̼� �̸��� Idle�� �ƴϸ� Play
        if (!curAnimStateInfo.IsName("Idle"))
        {
            anim.Play("Idle", 0, 0);
        }

        yield return new WaitForSeconds(chaseDelay);
        ChangeState(State.CHASE);

    }
    #endregion

    #region CHASE
    IEnumerator CHASE()
    {
        if (!AnimCheck())
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
    }
    #endregion

    #region ATTACK
    IEnumerator ATTACK()
    {
        if (!AnimCheck())
        {
            var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            isAttack = true;
            anim.Play("Attack", 0, 0);
            target = null;
            yield return new WaitForSeconds(curAnimStateInfo.length * 2);
            isAttack = false;
            ChangeState(State.IDLE);
        }
        //if (remainingDistance > stoppingDistance) // �Ÿ��� �־����� �ٽ� �߰�
        //{
        //    ChangeState(State.CHASE);
        //}
        //else
        //{
        //    // ��� �ð��� �̿��� ���� ������ ������ �� ����

        //}
    }
    #endregion

    #region TELEPORT
    IEnumerator TELEPORT()
    {
        if(target == null)
        {
            target = saveTarget;
        }
        if (!AnimCheck())
        {
            var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            var wrPicker = new Rito.WeightedRandomPicker<string>();

            wrPicker.Add(
                ("Left", 50),
                ("Right", 50)
                );

            isTeleport = true;
            Colenabled();
            anim.Play("Teleport_before");
            yield return new WaitForSeconds(curAnimStateInfo.length);

            transform.position = new Vector3(-120, transform.position.y);
            string randIndex = wrPicker.GetRandomPick();
            Debug.Log("randIndex : " + randIndex);
            yield return new WaitForSeconds(curAnimStateInfo.length * 2f);

            transform.position = (randIndex == "Right") ?
                new Vector3(target.position.x + stoppingDistance, transform.position.y) : new Vector3(target.position.x - stoppingDistance, transform.position.y);
            anim.Play("Teleport_after");
            target = saveTarget;
            //StartCoroutine(nameof(CooldownOfTeleport));
            yield return new WaitForSeconds(curAnimStateInfo.length);
            Colabled();
            isTeleport = false;
        }
    }
    #endregion

    #region SPELL
    IEnumerator SPELL()
    {
        if (!AnimCheck())
        {
            if (target == null) yield break;

            var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            isSpell = true;
            anim.Play("Cast");
            yield return new WaitForSeconds(curAnimStateInfo.length);

            var missile = ObjectPooling.GetObject();
            missile.transform.position = new Vector3(target.position.x, 2.73f, target.position.z);
            isSpell = false;
            ChangeState(State.IDLE);
        }
    }
    #endregion

    #region GUIDED ATTACK
    IEnumerator GUIDED_ATTACK()
    {
        if(!AnimCheck())
        {
            if (target == null) yield break;

            var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            isGuidedAttack = true;
            anim.Play("Cast-NoEffect");
            yield return new WaitForSeconds(curAnimStateInfo.length);

            var missile = Instantiate(GuidedMissile);
            missile.transform.position = launcherPosition.position;
            var gMissile = missile.GetComponent<GuidedMissile>();
            gMissile.target = saveTarget;
            missile.SetActive(true);
            isGuidedAttack = false;
            ChangeState(State.IDLE);
        }
    }
    #endregion

    #region PATTERN1
    IEnumerator PATTERN1()
    {
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);

        isPattern1 = true;
        anim.Play("PatternCastingFirst");
        yield return new WaitForSeconds(curAnimStateInfo.length * 20f);
        // ���� ���� ������ ���ϱ�
        if(pattern1Delay > 0) // ���� ����
        {// ���� ���ѽ� -> Groggy
            anim.SetBool("Pattern1", false);
        }
        else // ���� ����
        {// ���� ���� ���н� -> ���� �ߵ�
            anim.SetBool("Pattern1", true);
        }
    }
    #endregion

    #region PATTERN2
    public void PATTERN2()
    {
        
    }
    #endregion

    #region PATTERN3
    public void PATTERN3()
    {
        
    }
    #endregion

    #region GROGGY
    public void GROGGY()
    {
        
    }
    #endregion

    #region KILLED
    public void KILLED()
    {
        DeathBringerController ctrl;
        ctrl = GetComponent<DeathBringerController>();
        anim.Play("Death");
        target = null;
        ctrl.enabled = false;
    }
    #endregion

    #region HURT
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(monsterData.currentHp <= 0)
        {
            KILLED();
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

    #region OTHER METHODS
    private void Colenabled()
    {
        col.enabled = false;
        rb.gravityScale = 0f;
    }

    private void Colabled()
    {
        col.enabled = true;
        rb.gravityScale = 9.8f;
    }

    #endregion
    //IEnumerator CooldownOfTeleport()
    //{
    //    yield return new WaitForSeconds(teleportCooldown);
    //    isTeleport = false;
    //}

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
        isTeleport = false;
        isAttack = false;
        isGuidedAttack = false;
        isSpell = false;
        state = State.IDLE; // �⺻�� IDLE ���·� ����
    }
    #endregion
}

public class Skill
{
    public string name;
    public float cooldown;
    public float currentCooldown;
    public int priority;

    public Skill(string name, float cooldown, int priority)
    {
        this.name = name;
        this.cooldown = cooldown;
        this.priority = priority;
        this.currentCooldown = 0;
    }

    public void UpdateCooldown(float deltaTime)
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= deltaTime;
        }
    }

    public bool IsReady()
    {
        return currentCooldown <= 0;
    }
}