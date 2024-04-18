using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{

    public Transform target;
    public float speed = 2f;
    public Vector3 moveDirection;
    public Vector3 dir;
    public float disappearTime = 5f;
    public float rotSpeed = 3f;

    private void Update()
    {
        SetDestination(target);
        Chase();
        //SetRotate();
        Invoke("DestroyMissile", disappearTime);
    }

    public void Chase()
    {
        transform.position += moveDirection * speed * Time.deltaTime; // 오브젝트를 이동시키기 위해서 사용
        transform.right = Vector3.Slerp(transform.right.normalized, moveDirection, speed * Time.deltaTime); // 오브젝트를 이동할때 급격하게 커브를 도는것을 방지하기 위해서 사용
        
    }

    private void DestroyMissile()
    {
        target = null;
        ObjectPooling.ReturnObject(this);
    }

    public void SetDestination(Transform target)
    {
        if (target)
        {
            Vector3 direction = target.position - transform.position;
            moveDirection = direction.normalized;
            dir = direction;
        }
    }

    public void SetRotate()
    {
        // 오브젝트가 부드럽게 회전할 수 있도록 만들어줌 :O
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var rot = Quaternion.Euler(0, 0, rotZ);
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
        //transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
    }
}
