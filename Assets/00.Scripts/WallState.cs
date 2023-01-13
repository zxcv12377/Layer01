using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WallState : MonoBehaviour
{
    // Component
    private CharacterController CC;
    private GameObject player;
    private WorldManagerController WMC;
    [SerializeField] private GameObject WMCobj;
    private SpriteRenderer[] sr;
    private SpriteRenderer[] piecesRenderer;

    // Shake
    [SerializeField] [Range(0.01f,0.1f)] private float ShakeRange = 0.05f;
    [SerializeField] private float ShakeDuration = 0.2f;
    private Vector3 mainPos;

    // Destructible
    private IObjectPool<DestructiblePieces> pool;
    [SerializeField] private GameObject destructiblePieces;
    private bool isDestroyed = false;

    // State
    [Header("State")]
    [SerializeField] private WallData wallData;
    public WallData WallData { set { wallData = value; } }
    private float currentHP;
    private float maxHP;
    private float Defence;
    private Sprite sprite;

    private void Awake()
    {
        pool = new ObjectPool<DestructiblePieces>(CreatPieces, OnGetPieces, OnReleassPieces, OnDestroyPieces, maxSize:1);
    }

    private void Start()
    {
        StartCoroutine(FindPlayer());
        sr = GetComponentsInChildren<SpriteRenderer>();
        piecesRenderer = destructiblePieces.GetComponentsInChildren<SpriteRenderer>();
        currentHP = wallData.currentHp;
        maxHP = wallData.maxHp;
        for(int i = 0; i < sr.Length; i++)
        {
            sr[i].sprite = wallData.Sprite;
        }
        for(int i = 0; i <piecesRenderer.Length; i++)
        {
            piecesRenderer[i].sprite = wallData.Sprite;
        }
    }

    IEnumerator FindPlayer()
    {
        while(player == null && WMCobj == null)
        {
            yield return new WaitForSeconds(0.2f);
            player = GameObject.FindWithTag("Player");
            WMCobj = GameObject.FindWithTag("WorldManager");
            CC = player.GetComponent<CharacterController>();
            WMC = WMCobj.GetComponent<WorldManagerController>();
        }
        
    }
    //      충돌
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MeleeAttack")
        {
            ShakeBody();
            TakeHit();
        }
    }
    //      지진
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
            Destroyobj();
        }
    }
    //      파괴
    private void Destroyobj()
    {
        //mainPos = new Vector3(transform.position.x, transform.position.y, 2);
        var pieces = pool.Get();
        gameObject.SetActive(false);
        WMC.IsPause = true;
        WMC.SelectCardVisible();
    }
    //      Destructible
    private DestructiblePieces CreatPieces()
    {
        var pieces = Instantiate(destructiblePieces, new Vector3(transform.position.x, 0, 4), Quaternion.identity);
        DestructiblePieces piecesComp = pieces.GetComponent<DestructiblePieces>();
        piecesComp.destroytime();
        piecesComp.SetManagedPool(pool);
        return piecesComp;
    }

    private void OnGetPieces(DestructiblePieces pieces)
    {
        pieces.gameObject.SetActive(true);
    }

    private void OnReleassPieces(DestructiblePieces pieces)
    {
        pieces.gameObject.SetActive(false);
    }
    
    private void OnDestroyPieces(DestructiblePieces pieces)
    {
        Destroy(pieces.gameObject);
    }
}
