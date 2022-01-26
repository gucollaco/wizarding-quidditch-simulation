using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveSpeed = 0.1f;
    private float rotateSpeed = 2f;

    private void LateUpdate () {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            transform.position -= moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
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
            transform.eulerAngles += rotateSpeed * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        }
    }
}
