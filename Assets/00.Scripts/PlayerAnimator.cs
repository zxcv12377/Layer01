using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    #region COMPONENT
    private PlayerMovement mov;
    public Animator anim { get; private set; }
    #endregion

    #region STRING TO HASH
    public readonly int hashJumpAnimation = Animator.StringToHash("Jump");
    public readonly int hashLandAnimation = Animator.StringToHash("Land");
    public readonly int hashDashAnimation = Animator.StringToHash("Dash");
    public readonly int hashIsWallJumpAnimation = Animator.StringToHash("isWallJump");
    public readonly int hashIsWallSlideAnimation = Animator.StringToHash("isWallSlide");
    public readonly int hashIsMoveAnimation = Animator.StringToHash("isMove");
    public readonly int hashIsJumpAnimation = Animator.StringToHash("isJump");
    public readonly int hashIsGroundAnimation = Animator.StringToHash("isGround");
    public readonly int hashyVelocityAnimation = Animator.StringToHash("yVelocity");
    public readonly int hashCurrentHpAnimation = Animator.StringToHash("currentHp");
    public readonly int hashIsAttackAnimation = Animator.StringToHash("isAttack");
    public readonly int hashAttackComboAnimation = Animator.StringToHash("AttackCombo");
    public readonly int hashAttackSpeedAnimation = Animator.StringToHash("AttackSpeed");
    #endregion

    #region PARAMETER
    public bool isMove { private get; set; }
    public bool startedJumping { private get; set; }
    public bool justLanded { private get; set; }
    public bool startedDash { private get; set; }
    public bool isGround { private get; set; }
    public bool isWallSlide { private get; set; }
    public bool isWallJump { private get; set; }
    public bool isJump { private get; set; }
    public bool isAttack { private get; set; }
    #endregion

    public AnimatorStateInfo currentStateInfo { get; private set; }

    void Start()
    {
        mov = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();

        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
    }
    private void LateUpdate()
    {
        CheckAnimationState();
    }

    private void CheckAnimationState()
    {
        
        if (startedJumping)
        {
            anim.SetTrigger(hashJumpAnimation);
            startedJumping = false;
            return;
        }
        if (justLanded)
        {
            anim.SetTrigger(hashLandAnimation);
            justLanded = false;
            return;
        }

        if (startedDash)
        {
            anim.SetTrigger(hashDashAnimation);
            startedDash = false;
            return;
        }
        anim.SetBool(hashIsWallJumpAnimation, isWallJump);
        anim.SetBool(hashIsWallSlideAnimation, isWallSlide);
        anim.SetBool(hashIsMoveAnimation, isMove);
        anim.SetBool(hashIsJumpAnimation, isJump);
        anim.SetBool(hashIsGroundAnimation, isGround);
        anim.SetBool(hashIsAttackAnimation, isAttack);
        anim.SetFloat(hashyVelocityAnimation, mov.RB.velocity.y);
        anim.SetFloat(hashCurrentHpAnimation, mov.currentHp);
        anim.SetFloat(hashAttackSpeedAnimation, mov.attackSpeed);
        anim.SetInteger(hashAttackComboAnimation, mov.comboCount);
    }
}
