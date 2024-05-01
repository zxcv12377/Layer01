using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathHand : MonoBehaviour
{
    public Animator anim;
    public float animLength;

    // Start is called before the first frame update
    private void Awake()
    {
        anim = GetComponent<Animator>();
        var curAnimStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        animLength = curAnimStateInfo.length;
    }

    public void InvokeDestroy()
    {
        Invoke("DestroyMissile", animLength);
    }

    private void DestroyMissile()
    {
        ObjectPooling.ReturnObject(this);
    }
}
