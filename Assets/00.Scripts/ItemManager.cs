using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    APPLE,
    KEY
}

public class ItemManager : MonoBehaviour
{
    private PlayerMovement player;
    public ItemType itemType;
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
        }
    }

    #region OBJECT ACTVIE
    private IEnumerator ObjectActive()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(true);
    }
    #endregion
}
