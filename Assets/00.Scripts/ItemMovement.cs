using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    private Vector3 pos;
    private Vector3 origin;
    public float posSpeed = 1.0f;
    public float rotSpeed = 100.0f;
    private void Start()
    {
        origin = transform.position;
        pos = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(origin, pos, Mathf.PingPong(Time.time * posSpeed, 1.0f));
        transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0f));
    }
}
