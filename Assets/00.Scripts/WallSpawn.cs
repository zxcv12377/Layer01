using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallType
{
    Normal, Bullet
}
public class WallSpawn : MonoBehaviour
{
    public WallType type;

    [SerializeField] private WallData wallData;
    [SerializeField] private GameObject wallPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "SpawnTrigger")
        {
            var wall = SpawnWall(type);
        }
    }

    public WallState SpawnWall(WallType type)
    {
        var newWall = Instantiate(wallPrefab, transform.position, Quaternion.identity);
        var NewWall = newWall.GetComponent<WallState>();
        NewWall.WallData = wallData;
        WallInfo();
        return NewWall;
    }

    public void WallInfo()
    {
        Debug.Log("벽 이름 : " + wallData.WallName);
        Debug.Log("벽 현재체력 : " + wallData.currentHp);
        Debug.Log("벽 최대체력 : " + wallData.maxHp);
        Debug.Log("벽 방어력 : " + wallData.Defence);
        Debug.Log("벽 스프라이트 : " + wallData.Sprite);
    }
}
