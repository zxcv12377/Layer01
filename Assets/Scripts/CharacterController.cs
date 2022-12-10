using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1. ���� ����� �뽬 ���, �״� ��, ü��
// 2. ���������� ���� ��������
// 3. �÷����� �����ϰ� ī�޶� �����Ӱ� ��ī�̹ڽ� �����ӵ� �����ؾ���.
// (�߰�)
// 4. �뽬 ������ �����ϱ� (�ذ�)
public class CharacterController : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    //TrailRenderer tr;

    float Hmove;

    //private
    private float MoveSpeed = 5f;
    [SerializeField] private float jumpPower = 25f;
    private float DashPower = 10f;

    private bool canDash = true;
    private float DashTime = 0.2f;
    private float DashCooldown = 1f;

    //public
    
    public int maxHP;
    public int currentHP;



    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (anim.GetBool("isDash"))
        {
            return;
        }

        Hmove = Input.GetAxisRaw("Horizontal");
        PlayerMove();
        if (Input.GetKey(KeyCode.X))
        {
            Attack();
        }
        if (Input.GetKey(KeyCode.LeftShift) && canDash)
        {
            Debug.Log("Dash");
            StartCoroutine(Dash());
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (anim.GetBool("isDash"))
        {
            return;
        }
        AnimationUpdate();

        if(rigid.velocity.y < 0)
        {
            anim.SetBool("isFalling", true);
        }

        anim.SetFloat("yVelocity", rigid.velocity.y);
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
                spriteRenderer.flipX = false;
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                movePosition = Vector3.left;
                spriteRenderer.flipX = true;
            }
        }
        

        transform.position += movePosition * MoveSpeed * Time.deltaTime;
                
        
    }

    // Collision //

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            anim.SetBool("isJump", false);
            anim.SetBool("isFalling", false);
        }
    }

    // Attack //
    public void Attack()
    {
        if (anim.GetBool("isAttack") != true && anim.GetBool("isDash") != true)
        {
            anim.SetBool("isAttack", true);
        }
    }

    public void Jump()
    {
        if (anim.GetBool("isJump") != true && anim.GetBool("isDash") != true)
        {
            anim.SetBool("isJump", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
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
            float OriginalGravity = rigid.gravityScale;
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(transform.localScale.x * DashPower * Dir, 0f);
            yield return new WaitForSeconds(DashTime);
            rigid.gravityScale = OriginalGravity;
            anim.SetBool("isDash", false);
            yield return new WaitForSeconds(DashCooldown);
            canDash = true;
        }
    }

    // Animation Control //
    void AnimationUpdate()
    {
        if (Hmove == 0)
        {
            anim.SetBool("isMove", false);
        }
        else anim.SetBool("isMove", true);
    }

    public void StopAttack() // Attack �ִϸ��̼ǿ� �����
    {
        anim.SetBool("isAttack", false);
    }

    // Die //
    void Die()
    {
        anim.SetTrigger("Death");
    }
}


