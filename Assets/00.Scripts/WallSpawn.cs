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
        Debug.Log("�� �̸� : " + wallData.WallName);
        Debug.Log("�� ����ü�� : " + wallData.currentHp);
        Debug.Log("�� �ִ�ü�� : " + wallData.maxHp);
        Debug.Log("�� ���� : " + wallData.Defence);
        Debug.Log("�� ��������Ʈ : " + wallData.Sprite);
    }
}
