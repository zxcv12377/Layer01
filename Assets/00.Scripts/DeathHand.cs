using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHand : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D col;
    private float animLength;

    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        animLength = curAnimStateInfo.length;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            playerMovement.currentHp--;
        }
    }

    public void InvokeDestroy()
    {
        Invoke("DestroyMissile", animLength);
    }

    private void DestroyMissile()
    {
        ObjectPooling.ReturnObject(this);
    }

    public void AttackStart()
    {
        col.enabled = true;
    }

    public void AttackEnd()
    {
        col.enabled = false;
    }
}
