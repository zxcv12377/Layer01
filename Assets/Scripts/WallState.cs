using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallState : MonoBehaviour
{
    private CharacterController CC;

    private GameObject player;

    [SerializeField] [Range(0.01f,0.1f)] float ShakeRange = 0.05f;
    [SerializeField] float ShakeDuration = 0.2f;
    Vector3 mainPos;

    [SerializeField] private GameObject destructiblePieces;
    private bool isDestroyed = false;

    [Header("State")]
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
            TakeHit();
        }
    }

    private void ShakeBody()
    {
        mainPos = transform.position;
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
        transform.position = mainPos;
    }

    private void TakeHit()
    {
        currentHP -= CC.Damage;
        if(currentHP <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            mainPos = new Vector3(transform.position.x, transform.position.y, 2);
            GameObject debris = Instantiate(destructiblePieces, mainPos, transform.rotation);
            StartCoroutine(Destructobj(debris));
            Destroy(gameObject);
        }
    }

    private IEnumerator Destructobj(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
