using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCirculation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float PosValue = 24f;

    Vector2 StartPos;
    float newPos;

    // Start is called before the first frame update
    void Start()
    {
        StartPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        newPos = Mathf.Repeat(Time.time * speed, PosValue);
        transform.position = StartPos + Vector2.right * newPos;
    }
}
