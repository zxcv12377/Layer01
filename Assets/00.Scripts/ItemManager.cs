using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    APPLE,
    KEY,
    PIXIE
}

public class ItemManager : MonoBehaviour
{
    private PlayerMovement player;
    private Animator anim;
    public ItemType itemType;

    [SerializeField] private GameObject particle;

    private void Start()
    {
        if(itemType == ItemType.PIXIE)
        {
            anim = GetComponent<Animator>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (itemType)
        {
            #region DASH RESET
            case ItemType.APPLE:
                if (collision.gameObject.CompareTag("Player"))
                {
                    player = collision.gameObject.GetComponent<PlayerMovement>();
                    player.ItemApple();
                    StartCoroutine(ObjectActive());
                }
                break;
            #endregion

            case ItemType.KEY:
                if (collision.gameObject.CompareTag("Player"))
                {

                }
                break;
            #region BOSS PATTERN BREAKER
            case ItemType.PIXIE:
                if (collision.gameObject.CompareTag("Player"))
                {
                    GetPixie();
                }
                break;
            #endregion
        }
    }
    #region PIXIE EVENT
    private void GetPixie()
    {
        anim.SetTrigger("Get");
    }


    public void TwinkleParticle()
    {
        particle.SetActive(true);
    }
    #endregion

    #region OBJECT ACTVIE
    private IEnumerator ObjectActive()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(true);
    }
    #endregion
}
