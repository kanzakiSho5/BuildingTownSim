using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseButtonType
{
    left,
    right,
    wheel
}


public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private MouseButtonType rotateMouseKey = MouseButtonType.wheel;
    [SerializeField] private MouseButtonType moveMouseKey = MouseButtonType.right;
    [SerializeField] private Transform cameraRoot;

    private Vector2 firstCursorPosition = Vector2.zero;
    private Camera camera;

    private void Awake()
    {
        camera = gameObject.GetComponent<Camera>();
    }

    private void Update()
    {
        float zoomAxis = Input.mouseScrollDelta.y;
        //Debug.Log(zoomAxis);
        if (zoomAxis != 0f)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            float zoomDistance = zoomSpeed * zoomAxis * Time.deltaTime;
            camera.transform.Translate(ray.direction * zoomDistance, Space.World);
        }

        // カメラの回転
        Vector3 mousePosition = Input.mousePosition;

        if (Input.GetMouseButtonDown((int) moveMouseKey))
        {
            //Debug.Log("Init MousePosition");
            firstCursorPosition = mousePosition;
        }
        
        if (Input.GetMouseButton((int) moveMouseKey))
        {
            Move(mousePosition);
        }

        if (Input.GetMouseButton((int) rotateMouseKey))
        {
            Rotate();
        }
    }

    private void Move(Vector3 mousePosition)
    {
        float v = mousePosition.y - firstCursorPosition.y;
        float h = mousePosition.x - firstCursorPosition.x;
        // Debug.LogFormat("\nMouseMove: x:{0} y:{1} \nCursorPos: x:{2} y:{3}\nfirstpos: x:{4} y:{5}", h, v,mousePosition.x,mousePosition.y,firstCursorPosition.x,firstCursorPosition.y);

        cameraRoot.position += (transform.right * h + transform.up * v) * Time.deltaTime * moveSpeed;
    }

    private void Rotate()
    {
        float v = Input.GetAxis("Mouse X") * rotateSpeed;
        float h = Input.GetAxis("Mouse Y") * rotateSpeed;
        
        
        cameraRoot.localEulerAngles += new Vector3(h, v, 0);
    }
}
