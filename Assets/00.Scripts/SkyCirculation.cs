using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCirculation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float PosValue;
    [SerializeField] private GameObject camPos;

    Vector2 StartPos;
    float newPos;

    // Update is called once per frame
    void Update()
    {
        StartPos = camPos.transform.position;
        newPos = Mathf.Repeat(Time.time * speed, PosValue);
        transform.position = StartPos + Vector2.right * newPos;
    }
}
