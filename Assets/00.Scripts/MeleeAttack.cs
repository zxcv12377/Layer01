using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField] private CharacterController CC;
    BoxCollider2D col;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CC.isAttack)
        {
            col.enabled = true;
        }
        else
        {
            col.enabled = false;
        }
    }
}
