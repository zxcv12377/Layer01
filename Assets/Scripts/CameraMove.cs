using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // private
    [SerializeField] private float cameraSpeed = 1;
    [SerializeField] private CharacterController CC;

    // Update is called once per frame
    void Update()
    {
        Vector3 movePosition = Vector3.right;
        transform.position += movePosition * cameraSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            CC.currentHP -= 1;
        }
    }
}
