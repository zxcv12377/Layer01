using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{

    [HideInInspector]public Transform target;
    private float speed = 3f;
    private Vector3 moveDirection;
    private Vector3 dir;
    private float disappearTime = 5.0f;

    private void Start()
    {
        Invoke("DestroyMissile", disappearTime);
    }
    private void Update()
    {
        SetDestination(target);
        SetRotate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            playerMovement.currentHp--;
            DestroyMissile();
        }
    }

    private void DestroyMissile()
    {
        target = null;
        gameObject.SetActive(false);
    }

    public void SetDestination(Transform target)
    {
        if (target)
        {
            Vector3 direction = target.position - transform.position;
            moveDirection = direction.normalized;
            dir = direction;
            transform.position += moveDirection * speed * Time.deltaTime; // 오브젝트를 이동시키기 위해서 사용
            transform.right = Vector3.Slerp(transform.right.normalized, moveDirection, speed * Time.deltaTime); // 오브젝트를 이동할때 급격하게 커브를 도는것을 방지하기 위해서 사용
        }
    }

    public void SetRotate()
    {
        // 오브젝트가 부드럽게 회전할 수 있도록 만들어줌 :O
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
