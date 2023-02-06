using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    public float width = 1f;
    public float height = 1f;

    public Color color = Color.white;

    public GameObject[] prefabsList;

    void OnDrawGizmos()
    {
        Vector3 pos = Camera.current.transform.position;    // 현재 scene view 에서 보고 있는 카메라 좌표를 가져옵니다.
        Gizmos.color = color;

        if (width <= 0 || height <= 0)
            return;

        for (float y = pos.y - 40f; y < pos.y + 40f; y += height)
        {
            Gizmos.DrawLine(new Vector3(10000000.0f, Mathf.Floor(y / height) * height, 0f),
            new Vector3(-10000000, Mathf.Floor(y / height) * height, 0f));
        }

        for (float x = pos.x - 40f; x < pos.x + 40f; x += width)
        {
            Gizmos.DrawLine(new Vector3(Mathf.Floor(x / width) * width, 10000000, 0f),
            new Vector3(Mathf.Floor(x / width) * width, -10000000, 0f));
        }
    }
}
