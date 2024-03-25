using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private PlayerMovement mov;
    private Animator anim;
    private SpriteRenderer spriteRend;

    public bool isMove { private get; set; }
    public bool startedJumping { private get; set; }
    public bool justLanded { private get; set; }
    public bool startedDash { private get; set; }
    public bool isGround { private get; set; }
    public bool isWallSlide { private get; set; }
    public bool isWallJump { private get; set; }

    public bool isJump { private get; set; }
    // Start is called before the first frame update
    void Start()
    {
        mov = GetComponent<PlayerMovement>();
        spriteRend = GetComponent<SpriteRenderer>();
        anim = spriteRend.GetComponent<Animator>();
    }
    private void LateUpdate()
    {
        CheckAnimationState();
    }

    private void CheckAnimationState()
    {
        
        if (startedJumping)
        {
            anim.SetTrigger("Jump");
            startedJumping = false;
            return;
        }
        if (justLanded)
        {
            anim.SetTrigger("Land");
            justLanded = false;
            return;
        }

        if (startedDash)
        {
            anim.SetTrigger("Dash");
            startedDash = false;
            return;
        }
        anim.SetBool("isWallJump", isWallJump);
        anim.SetBool("isWallSlide", isWallSlide);
        anim.SetBool("isMove", isMove);
        anim.SetBool("isJump", isJump);
        anim.SetBool("isGround", isGround);
        anim.SetFloat("yVelocity", mov.RB.velocity.y);
    }
}
