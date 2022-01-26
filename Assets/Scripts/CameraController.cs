using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float moveSpeed = 0.2f;

    private void Update () {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            transform.position -= moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += moveSpeed * new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += moveSpeed * new Vector3(0, -1, 0);
        }
    }
}
