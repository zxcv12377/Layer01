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
        switch (itemtype)
        {
            case ItemType.Apple:
                    if (collision.gameObject.CompareTag("Player"))
                {
                    player = collision.gameObject.GetComponent<PlayerMovement>();
                    player.ItemApple();
                    StartCoroutine(ObjectActive());
                }
                break;
            case ItemType.Key:
                break;
        }
    }

    private IEnumerator ObjectActive()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(true);
    }
}


