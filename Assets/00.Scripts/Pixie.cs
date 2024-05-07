using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixie : MonoBehaviour
{
    private ItemManager itemManager;
    private Transform target;
    private float speed = 1;

    private Queue<Vector3> targetPos;

    private Animator anim;
    private void Awake()
    {
        targetPos = new Queue<Vector3>();
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        itemManager = GetComponent<ItemManager>();
    }

    void Update()
    {
        target = itemManager.target;
        if(target != null && anim.GetCurrentAnimatorStateInfo(0).IsName("pixieflying"))
        {
            Follow();
            if (!transform.parent)
                Watch();

            if (!targetPos.Contains(target.position))
                transform.SetParent(target);
            
        }
    }

    private void Watch()
    {
        targetPos.Enqueue(target.position);
    }

    private void Follow()
    {
        transform.position = Vector2.Lerp(transform.position, target.transform.position, Time.deltaTime * speed);
    }
}
