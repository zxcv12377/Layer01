using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterController : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    //private

    float Hmove;
    //public
    public float MoveSpeed;
    public float jumpPower;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Hmove = Input.GetAxisRaw("Horizontal");
        PlayerMove();
    }
    // Update is called once per frame
    void Update()
    {
        AnimationUpdate();
    }

    void PlayerMove()
    {
        Vector3 movePosition = Vector3.zero;

        if(Input.GetAxisRaw("Horizontal") > 0)
        {
            movePosition = Vector3.right;
            spriteRenderer.flipX = false;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            movePosition = Vector3.left;
            spriteRenderer.flipX = true;
        }

        transform.position += movePosition * MoveSpeed * Time.deltaTime;
                
        if (Input.GetKey(KeyCode.Space))
        {
            if (anim.GetBool("isJump") != true)
            {
                anim.SetBool("isJump", true);
                rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Platform")
        {
            anim.SetBool("isJump", false);
        }
    }

    void AnimationUpdate()
    {
        if(Hmove == 0)
        {
            anim.SetBool("isMove", false);
        }
        else anim.SetBool("isMove", true);
    }
}


