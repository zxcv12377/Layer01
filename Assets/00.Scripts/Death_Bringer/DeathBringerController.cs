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
    [SerializeField] private float lostDistance; // 플레이어와 일정 거리가 멀어질때를 감지하기 위한 변수
    [SerializeField] private float remainingDistance; // 플레이어까지 현재 남은 거리
    [SerializeField] private float stoppingDistance; // 플레이어에게 도착하기 전, 어느정도의 거리에서 멈출지 알려주는 변수
    [Space(5)]
    [Header("Delay")]
    [SerializeField] private float chaseDelay; // 플레이어를 따라다닐때 생기는 딜레이
    [SerializeField] private float pattern1Delay; // 패턴 파훼 시간 
    [Space(5)]
    [Header("Attack State")]
    [SerializeField] private float teleportCooldown; // 플레이어를 향해 텔레포트를 사용하기 위한 시간
    [SerializeField] private float spellCooldown; // 일정 시간마다 범위공격을 가하기 위한 시간
    [SerializeField] private float guidedAttackCooldown; // 일정 시간마다 범위공격을 가하기 위한 시간
    [SerializeField] private float groggyTime; // 그로기 시간
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
            new Skill("유도공격", guidedAttackCooldown, 1),
            new Skill("마법공격", spellCooldown, 2),
            new Skill("텔레포트", teleportCooldown, 3)
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

        //List<Skill> skills = new List<Skill> // AI의 행동의 우선순위를 설정 
        //{
        //    new Skill("유도공격", guidedAttackCooldown, 1),
        //    new Skill("마법공격", spellCooldown, 2),
        //    new Skill("텔레포트", teleportCooldown, 3)
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
            case "텔레포트":
                StartCoroutine(nameof(TELEPORT));
                //ChangeState(State.TELEPORT);
                break;
            case "마법공격":
                StartCoroutine(nameof(SPELL));
                //ChangeState(State.SPELL);
                break;
            case "유도공격":
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
        // 현재 진행중인 Animator 상태 정보
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if(target != null)
        {
            target = null;
        }

        // 애니메이션 이름이 Idle이 아니면 Play
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
                // SetDestination을 위해 한 프레임을 넘기기위한 코드
                yield return null;
            }
            // 목표까지 남은 거리가 멈추는 지점보다 작거나 같으면 공격
            if (remainingDistance <= stoppingDistance)
            {
                ChangeState(State.ATTACK);
            }
            // 목표와의 거리가 멀어진 경우
            else if (remainingDistance > lostDistance)
            {

                yield return null;
            }
            else
            {
                // 애니메이션의 한 사이클 동안 대기

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
        //if (remainingDistance > stoppingDistance) // 거리가 멀어지면 다시 추격
        //{
        //    ChangeState(State.CHASE);
        //}
        //else
        //{
        //    // 대기 시간을 이용해 공격 간격을 조절할 수 있음

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
        // 패턴 파훼 조건을 정하기
        if(pattern1Delay > 0) // 파훼 성공
        {// 패턴 파훼시 -> Groggy
            anim.SetBool("Pattern1", false);
        }
        else // 파훼 실패
        {// 패턴 파훼 실패시 -> 공격 발동
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
        state = State.IDLE; // 기본을 IDLE 상태로 지정
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