using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{

    [HideInInspector]public Transform target;
    private float speed = 2f;
    private Vector3 moveDirection;
    private Vector3 dir;
    private float disappearTime = 2.0f;

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
            var playerMovement = collision.GetComponent<PlayerMovement>();
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
            transform.position += moveDirection * speed * Time.deltaTime; // ������Ʈ�� �̵���Ű�� ���ؼ� ���
            transform.right = Vector3.Slerp(transform.right.normalized, moveDirection, speed * Time.deltaTime); // ������Ʈ�� �̵��Ҷ� �ް��ϰ� Ŀ�긦 ���°��� �����ϱ� ���ؼ� ���
        }
    }

    public void SetRotate()
    {
        // ������Ʈ�� �ε巴�� ȸ���� �� �ֵ��� ������� :O
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }
}
