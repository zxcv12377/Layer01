using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public enum ItemType
{
    Key,
    Apple
}

public class TestScript : MonoBehaviour
{
    private PlayerMovement player;
    public ItemType itemtype;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(ItemType.Apple == itemtype)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                player = collision.gameObject.GetComponent<PlayerMovement>();
                player.ItemApple();
                StartCoroutine(ObjectActive());
            }
        }
    }

    private IEnumerator ObjectActive()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(true);
    }
}


