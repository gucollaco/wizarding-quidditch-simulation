using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private float moveSpeed = 0.1f;
    private float rotateSpeed = 2f;

    private void LateUpdate () {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

        if (horizontalAxis != 0 || verticalAxis != 0) {
            transform.position -= moveSpeed * new Vector3(horizontalAxis, 0, verticalAxis);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += moveSpeed * transform.up;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position -= moveSpeed * transform.up;
        }
        if (Input.GetMouseButton(1))
        {
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseX = Input.GetAxis("Mouse X");
            transform.eulerAngles += rotateSpeed * new Vector3(-mouseY, mouseX, 0);
        }
    }
}
