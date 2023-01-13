using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // private
    [SerializeField] private float cameraSpeed = 1;

    // Update is called once per frame
    void Update()
    {
        Vector3 movePosition = Vector3.right;
        transform.position += movePosition * cameraSpeed * Time.deltaTime;
    }
}
