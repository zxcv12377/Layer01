using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixie : MonoBehaviour
{
    [HideInInspector] public Transform target;

    public Vector3 followPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;

    [HideInInspector] public bool release;

    private Animator anim;
    // ����Ż Follow�����
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Watch();
        //Follow();
    }


    private void Watch()
    {
        followPos = parent.position;
    }

    private void Follow()
    {
        transform.position = followPos;
    }
}
