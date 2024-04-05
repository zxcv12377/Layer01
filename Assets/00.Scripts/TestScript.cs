using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class TestScript : MonoBehaviour
{
    private Vector3 pos;
    private Vector3 origin;
    public float speed = 1.0f;
    private void Start()
    {
        origin = transform.position;
        pos = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(origin, pos, Mathf.PingPong(Time.time * speed, 1.0f));
        transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, -3f, 3f), transform.localScale.y, transform.localScale.z);
    }
}


