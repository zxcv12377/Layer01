using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallState : MonoBehaviour
{

    private CharacterController CC;

    [SerializeField] [Range(0.01f,0.1f)] float ShakeRange = 0.05f;
    [SerializeField] float ShakeDuration = 0.2f;
    Vector3 Pos;

    private GameObject player;

    public float currentHP;
    public float maxHP;
    public float Defence;

    private void Start()
    {
        StartCoroutine(FindPlayer());
    }

    IEnumerator FindPlayer()
    {
        while(player == null)
        {
            yield return new WaitForSeconds(0.2f);
            player = GameObject.FindWithTag("Player");
            CC = player.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MeleeAttack")
        {
            
            ShakeBody();
            currentHP -= CC.Damage;
        }
    }

    private void ShakeBody()
    {
        Pos = transform.position;
        InvokeRepeating("StartShake", 0f, 0.005f);
        Invoke("StopShake", ShakeDuration);
    }

    private void StartShake()
    {
        float PosX = Random.value * ShakeRange * 2 - ShakeRange;
        float PosY = Random.value * ShakeRange * 2 - ShakeRange;
        Vector3 Pos = transform.position;
        Pos.x += PosX;
        Pos.y = PosY;
        transform.position = Pos;
    }
    
    private void StopShake()
    {
        CancelInvoke("StartShake");
        transform.position = Pos;
    }
}
