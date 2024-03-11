using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // private
    [SerializeField] private float cameraSpeed = 5;
    [SerializeField] private GameObject target;
    [SerializeField] Vector2 center;
    [SerializeField] Vector2 mapSize;
    private float height;
    private float width;
    // public

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        height = Camera.main.orthographicSize;
        width = height * Screen.width / Screen.height;
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        LimitCameraArea();
    }

    private void LimitCameraArea()
    {
        transform.position = Vector2.Lerp(transform.position, target.transform.position, Time.deltaTime * cameraSpeed);

        float lx = mapSize.x - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + center.x, lx + center.x);

        float ly = mapSize.y - height;
        float clampY = Mathf.Clamp(transform.position.y, - ly + center.y, ly + center.y);

        transform.position = new Vector3(clampX, clampY, -10f);
    }
}
