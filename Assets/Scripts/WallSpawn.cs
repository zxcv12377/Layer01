using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawn : MonoBehaviour
{
    private WallStore wallStore;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "SpawnTrigger")
        {
            wallStore = collision.GetComponent<WallStore>();
            GameObject Wall = wallStore.Wall;
            EnemySpawn(Wall);
        }
    }

    private void EnemySpawn(GameObject w)
    {
        Vector3 pos = new Vector3(w.transform.position.x, w.transform.position.y, 0);
        Instantiate(w, pos, Quaternion.identity);
    }
}
