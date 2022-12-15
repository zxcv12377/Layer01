using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. 다음 만들것 체력, 죽는 것, 히트모션 (해결)
// 2. 본격적으로 벽을 만들어야함 (해결)
// 3. 플랫폼도 수정
// 4. 카메라 움직임 (해결)
// 5. 스카이박스 움직임 (해결)
// (추가)
// 6. 대쉬 움직임 수정하기 (해결) 
// 7. 인터페이스
// 8. 벽의 히트모션 만들기
// 9. 더욱 다양한 벽들 양산하기
public class CharacterController : MonoBehaviour
{
    //Component
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private CharacterController cc;

    //private
    //      Move
    private float Hmove;
    private float MoveSpeed = 5f;
    [SerializeField] private float jumpPower = 25f;
    //      Dash
    private bool canDash = true;
    private float DashPower = 10f;
    private float DashTime = 0.2f;
    private float DashCooldown = 0.5f;
    //      Jump
    private bool isGrounded = true;
    private bool isJump = false;
    private bool DoubleJump = false;
    //      Attack
    public bool isAttack = false;


    //public
    //      State
    public float maxHP;
    public float currentHP;
    public float Damage;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (anim.GetBool("isDash"))
        {
            return;
        }
        Hmove = Input.GetAxisRaw("Horizontal");
        PlayerMove();

    }
    // Update is called once per frame
    void Update()
    {
        if (anim.GetBool("isDash"))
        {
            return;
        }
        animtionUpdate();

        if(rb.velocity.y < 0)
        {
            anim.SetBool("isFalling", true);
        }

        if (Input.GetKey(KeyCode.X))
        {
            Attack();
        }

        if (Input.GetKey(KeyCode.Z) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Jump();
        }

        anim.SetFloat("yVelocity", rb.velocity.y);
    }
    
    // Move //
    void PlayerMove()
    {
        Vector3 movePosition = Vector3.zero;

        if (anim.GetBool("isDash") != true)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                movePosition = Vector3.right;
                sr.flipX = false;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                movePosition = Vector3.left;
                sr.flipX = true;
            }
        }
        transform.position += movePosition * MoveSpeed * Time.deltaTime;
    }

    // Collision //
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = true;
            isJump = false;
            DoubleJump = false;
            anim.SetBool("isJump", false);
            anim.SetBool("isFalling", false);
        }
        if (collision.gameObject.tag == "Spike") //&& !isHit)
        {
            StartCoroutine(TakeHit(collision.transform.position));
        }
        if (collision.gameObject.tag == "MainCamera")
        {
            Die(collision.transform.position);
        }
    }

    // Attack //
    private void Attack()
    {
        if (anim.GetBool("isAttack") != true && anim.GetBool("isDash") != true)
        {
            anim.SetBool("isAttack", true);
        }
    }

    // Jump //
    private void Jump()
    {
        if (!anim.GetBool("isJump") && !anim.GetBool("isDash") && isGrounded && !DoubleJump)
        {
            isGrounded = false;
            isJump = true;
            anim.SetBool("isJump", true);
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            
        }
        else if(isJump && !DoubleJump)
        {
            DoubleJump = true;
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    // Dash //
    private IEnumerator Dash()
    {
        float Dir = Input.GetAxisRaw("Horizontal");
        if(Dir != 0)
        {
            canDash = false;
            anim.SetBool("isDash", true);
            gameObject.layer = 14;
            float OriginalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(transform.localScale.x * DashPower * Dir, 0f);
            yield return new WaitForSeconds(DashTime);
            gameObject.layer = 6;
            rb.gravityScale = OriginalGravity;
            anim.SetBool("isDash", false);
            yield return new WaitForSeconds(DashCooldown);
            canDash = true;
        }
    }

    // animtion Control // 
    void animtionUpdate()
    {
        if (Hmove == 0)
        {
            anim.SetBool("isMove", false);
        }
        else anim.SetBool("isMove", true);
    }

    public void HitcollisionOn()
    {
        isAttack = true;
    }

    public void StopAttack() // Attack 애니메이션에 사용중
    {
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    // Hit //
    private IEnumerator TakeHit(Vector2 targetPos)
    {
        currentHP -= 1;
        if(currentHP > 0)
        {
            canDash = true;
            gameObject.layer = 14;
            anim.SetTrigger("PlayerDamaged");
            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rb.AddForce(new Vector2(dirc, 1) * 15, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.3f);
            gameObject.layer = 6;
        }
        else
        {
            Die(targetPos);
        }
    }

    // Die //
    void Die(Vector2 targetPos)
    {
        gameObject.layer = 14;
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rb.AddForce(new Vector2(dirc, 1) * 15, ForceMode2D.Impulse);
        anim.SetTrigger("Death");
        cc.enabled = false;
    }

}


