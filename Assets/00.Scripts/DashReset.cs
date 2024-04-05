using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashReset : MonoBehaviour
{
    private PlayerMovement player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject.GetComponent<PlayerMovement>();
            player.ItemApple();
            StartCoroutine(ObjectActive());
        }
    }


    private IEnumerator ObjectActive()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5.0f);
        gameObject.SetActive(true);
    }
}
